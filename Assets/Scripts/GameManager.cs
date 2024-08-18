using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using GameInput;
using Interactables;
using Interactables.Enums;
using Interfaces;
using Levels;
using Levels.Enums;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Animations;

public class GameManager : MonoBehaviour
{
    public static event Action OnLevelStarted;
    public static event Action OnLevelComplete;
    public static event Action<float> OnWorldShake;
    public static event Action<int> OnLayerSelected;
    public static event Action OnLayerStarted;
    public static event Action<string> DisplayText;
    public static event Action<float> OnCountdown;

    private Dictionary<string, GameObject> _generatedPlayerContent;

    [SerializeField]
    private Vector3 spawnPosition;
    private Transform _containerInstance;
    private static LevelDataContainer CurrentLevel => LevelLoader.CurrentLevelDataContainer;

    [SerializeField]
    private ControlPanelContainer[] controlPanelContainers;
    private Dictionary<CONTROL_PANEL_TYPE, List<ControlPanelContainer>> _controlPanelContainers;

    [SerializeField,Min(0f), Header("Level Setup")]
    private float levelStartCountdownTime;
    [SerializeField, Min(0f)]
    private float layerFinishedWaitTime;
    
    [SerializeField, Min(0), Space(10f)]
    private int worldTagSceneIndex;
    
    [SerializeField, Header("Static Interactables")]
    private ButtonInteractable _startButton;


    [SerializeField, Min(0f), Header("Animations")]
    private float animationTime;

    [SerializeField]
    private AnimationCurve scaleCurve;
    [SerializeField]
    private AnimationCurve moveCurve;
    [SerializeField, Min(0)]
    private float worldShakeTime;

    [SerializeField, Header("Animation Boys")]
    private WaitForAnimationBase doorAnimation;
    [SerializeField]
    private WaitForAnimationBase controlPanel;
    [SerializeField]
    private WaitForAnimationBase startButton;
    [SerializeField]
    private WaitForAnimationBase tvScreenAnimation;

    private IDisplayDialog _displayDialog;
    private IGenerateSilhouette _silhouetteGenerator;
    private ICreateWorldReplacers _createWorldReplacers;
    private CinemachineImpulseSource _impulseSource;



    //============================================================================================================//
    
    public void Start()
    {
        _generatedPlayerContent = new Dictionary<string, GameObject>();
        LevelLoader.LoadFirstLevel();



        //Find from Children
        //------------------------------------------------//
        _displayDialog = GetComponentInChildren<IDisplayDialog>();
        _silhouetteGenerator = GetComponentInChildren<IGenerateSilhouette>();
        _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        
        Assert.IsNotNull(_startButton);

        //Sort the Panels
        //------------------------------------------------//
        _controlPanelContainers = new Dictionary<CONTROL_PANEL_TYPE, List<ControlPanelContainer>>();
        for (int i = 0; i < controlPanelContainers.Length; i++)
        {
            var container = controlPanelContainers[i];
            if (container == null)
                continue;
            
            if (_controlPanelContainers.ContainsKey(container.controlPanelType) == false)
            {
                _controlPanelContainers.Add(container.controlPanelType, new List<ControlPanelContainer>()
                {
                    container
                });
                continue;
            }
            
            _controlPanelContainers[container.controlPanelType].Add(container);
            
        }
        //============================================================================================================//
        
        
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        var operation = SceneManager.LoadSceneAsync(worldTagSceneIndex, LoadSceneMode.Additive);

        while (operation.isDone == false)
        {
            yield return null;
        }
        
        //Find object the World, once loaded
        //------------------------------------------------//
        _createWorldReplacers = FindObjectOfType<WorldReplaceManager>();
        
        do
        {
            var activeControlPanel = GetControlContainerAndDisableOthers(CurrentLevel.controlPanelType);

            SetupLevel(); 
            
            OnLevelStarted?.Invoke();

            yield return StartCoroutine(ApplyLevelObstaclesCoroutine());
            
            if (_displayDialog != null)
                yield return StartCoroutine(_displayDialog.DisplayDialogCoroutine(CurrentLevel.levelScript));

            //TODO Wait for button press
            DisplayText?.Invoke("Press Button to Start");

            yield return new WaitUntil(() => _startButton.InputValue >= 1f);

            DisplayText?.Invoke("Get Ready!");
            
            //Level
            yield return StartCoroutine(CountdownCoroutine(levelStartCountdownTime));

            //------------------------------------------------//
            var levelWaitTime = CurrentLevel.levelTime;
            var layers = CurrentLevel.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                OnLayerSelected?.Invoke(i);
                DisplayText?.Invoke("Next Layer!");

                var layer = CurrentLevel.layers[i];

                //Allow the player adjust controls
                yield return StartCoroutine(CountdownCoroutine(levelWaitTime));
                OnLayerStarted?.Invoke();

                //Get Inputs
                //------------------------------------------------//
                var (position, rotation, scale) = GetAllTransformations(layer, activeControlPanel);

                //Create New Object & Apply Transformations
                //------------------------------------------------//
                var newTransform = GetGeneratedLayerTransform(i, layer);
                //TODO Consider animating the position & the rotation as well!
                newTransform.position = position + spawnPosition;
                newTransform.rotation = quaternion.Euler(rotation);

                yield return StartCoroutine(ScaleCoroutine(newTransform, Vector3.zero, scale, animationTime));

                yield return new WaitForSeconds(1f);

                //Reparent object to Container
                //------------------------------------------------//
                newTransform.SetParent(_containerInstance);

                yield return StartCoroutine(doorAnimation.DoAnimationCoroutine(animationTime, false));

                //Move the Container down
                //------------------------------------------------//
                var objectCurrentPosition = newTransform.position;
                var endPosition = _containerInstance.position + Vector3.up * (i * CurrentLevel.yScale);
                yield return StartCoroutine(MoveToPositionCoroutine(
                    newTransform, 
                    objectCurrentPosition,
                    endPosition, 
                    animationTime));
                
                yield return StartCoroutine(doorAnimation.DoAnimationCoroutine(animationTime, true));
                //------------------------------------------------//

                _impulseSource?.GenerateImpulse();
                OnWorldShake?.Invoke(worldShakeTime);
            }

            //------------------------------------------------//

            //TODO Calculate the score

            yield return StartCoroutine(DisplayResultCoroutine());

            CleanupLevel();

            //If we've just wrapped the last lever, exit, and begin finishing the game
            if(LevelLoader.OnLastLevel())
                break;
            
            LevelLoader.LoadNextLevel();
        } while (true);

        //TODO Finish the Game
    }


    //Level Setup Functions
    //============================================================================================================//

    private void SetupLevel()
    {
        _silhouetteGenerator?.BuildSilhouette(CurrentLevel);
        _containerInstance = CreateNewContainer();
    }

    private Transform CreateNewContainer()
    {
        var newContainer = new GameObject($"---Level [{LevelLoader.CurrentLevelIndex.ToString()}] Container---").transform;

        var dataContainer = LevelLoader.CurrentLevelDataContainer;
        var layerHeight = dataContainer.yScale;
        var layerCount = dataContainer.layers.Length;
        
        newContainer.position = Vector3.down * (layerHeight * layerCount);

        return newContainer;
    }

    private static (Vector3 position, Vector3 rotation, Vector3 scale) GetAllTransformations(LayerData layerData, ControlPanelContainer controlPanel)
    {
        var currentLevel = CurrentLevel;
        var maxScale = CurrentLevel.maxScale;
        
        var outPosition = Vector3.zero;
        var outRotation = Vector3.zero;
        var outScale = new Vector3(0f, layerData.localScale.y, 0f);

        var levelMinPosition = CurrentLevel.MinPosition;
        var levelMaxPosition = CurrentLevel.MaxPosition;
        
        var controlValues = controlPanel.GetControlValues();

        //Go through each of the controls, then apply their values based on what they should be effecting
        for (int i = 0; i < controlValues.Length; i++)
        {
            var (control, value, value2) = controlValues[i];

            switch (control)
            {
                case CONTROLS.SCALE:
                    var yScale = currentLevel.yScale;
                    outScale = new Vector3(maxScale * Mathf.Clamp(value, 0.1f, 1f), 
                        yScale,
                        maxScale * Mathf.Clamp(value, 0.1f, 1f));
                    break;
                case CONTROLS.X_SCALE:
                    outScale.x = maxScale * Mathf.Clamp(value, 0.1f, 1f);
                    break;
                case CONTROLS.Z_SCALE:
                    outScale.z = maxScale * Mathf.Clamp(value, 0.1f, 1f);
                    break;
                case CONTROLS.X_POS:
                    outPosition.x = Mathf.Lerp(levelMinPosition.x, levelMaxPosition.x, value);
                    break;
                case CONTROLS.Z_POS:
                    outPosition.z = Mathf.Lerp(levelMinPosition.z, levelMaxPosition.z, value2);
                    break;
                case CONTROLS.Y_ROT:
                    outRotation.y = value * 360f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }


        return (outPosition, outRotation, outScale);
    }
    
    //Level End Functions
    //============================================================================================================//

    //This function should take the generated Object, and apply it to all of the equivalent tagged world objects
    
    private void CleanupLevel()
    {
        _containerInstance = null;
    }

    //Helper Functions
    //============================================================================================================//

    private Transform GetGeneratedLayerTransform(int layer, LayerData layerData)
    {
        var newLayerTransform = Instantiate(layerData.printLayerShape.prefab);
        newLayerTransform.gameObject.name = $"Layer_[{layer.ToString()}] {layerData.printLayerShape.prefab.name}";

        newLayerTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterial = layerData.Material;
        
        return newLayerTransform;
    }

    private ControlPanelContainer GetControlContainerAndDisableOthers(CONTROL_PANEL_TYPE controlPanelType)
    {
        Assert.IsTrue(controlPanelType != CONTROL_PANEL_TYPE.NONE);
        
        for (var i = 0; i < controlPanelContainers.Length; i++)
        {
            if (controlPanelContainers[i] == null)
            {
                Debug.LogWarning($"{nameof(GameManager)}.{nameof(controlPanelContainers)}[{i}] IS EMPTY!!");
                continue;
            }
            
            controlPanelContainers[i].SetActive(false);
        }

        if (!_controlPanelContainers.TryGetValue(controlPanelType, out var list))
            throw new Exception($"No control panel of type: {controlPanelType}");
        
        
        var found = list.PickRandomElement();
        found.SetActive(true);

        return found;
    }

    //Coroutine
    //============================================================================================================//

    private IEnumerator ApplyLevelObstaclesCoroutine()
    {
        yield break;
    }

    private IEnumerator DisplayResultCoroutine()
    {
        var continuePressed = false;
        void OnContinuePressed()
        {
            continuePressed = true;
        }

        GameInputDelegator.LockInputs = true;
        //Replace world Objects
        //------------------------------------------------//
        var currentLevel = CurrentLevel;

        _generatedPlayerContent.TryAdd(currentLevel.worldPlaceTag, _containerInstance.gameObject);
        _createWorldReplacers?.CreateWorldVersion(currentLevel.worldPlaceTag, currentLevel.outputScale, _containerInstance.gameObject);
        //------------------------------------------------//
        
        //TODO Enable the RenderTexture/Grab screen capture
        //Call to Display the UI to the player
        OnLevelComplete?.Invoke();

        //Wait for Continue to be pressed
        //------------------------------------------------//
        UIManager.OnContinuePressed += OnContinuePressed;
        yield return new WaitUntil(() => continuePressed);
        
        GameInputDelegator.LockInputs = false;
    }
    
    //Static Coroutines
    //============================================================================================================//

    private static IEnumerator CountdownCoroutine(float time, bool countUp = false)
    {
        if(time <= 0f)
            yield break;
        
        for (var t = 0f; t < time; t+= Time.deltaTime)
        {
            var value = countUp ? t : time - t;
            OnCountdown?.Invoke(value);
            
            yield return null;
        }
    }
    
    private IEnumerator ScaleCoroutine(Transform target, Vector3 startScale, Vector3 targetScale, float time)
    {
        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;

            target.transform.localScale = Vector3.Lerp(startScale, targetScale, scaleCurve.Evaluate(dt));
                    
            yield return null;
        }
    }

    private IEnumerator MoveToPositionCoroutine(Transform target, Vector3 startPosition, Vector3 endPosition, float time)
    {
        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;
            

            target.transform.position = Vector3.Lerp(startPosition, endPosition, moveCurve.Evaluate(dt));
                    
            yield return null;
        }
    }

}