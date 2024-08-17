using System;
using System.Collections;
using Interactables;
using Interactables.Enums;
using Interfaces;
using Levels;
using Levels.Enums;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnLevelComplete;
    public static event Action<string> DisplayText;
    public static event Action<float> OnCountdown; 

    [SerializeField]
    private Vector3 containerStartPosition;
    private Transform _containerInstance;
    private static LevelDataContainer CurrentLevel => LevelLoader.CurrentLevelDataContainer;

    [SerializeField]
    private ControlPanelContainer[] controlPanelContainers;

    [SerializeField, Min(0f), Header("Animations")]
    private float animationTime;

    [SerializeField]
    private AnimationCurve animationCurve;

    private IGenerateSilhouette _silhouetteGenerator;

    private ButtonInteractable _startButton;
    
    //============================================================================================================//
    
    public void Start()
    {
        LevelLoader.LoadFirstLevel();

        _silhouetteGenerator = GetComponent<IGenerateSilhouette>();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (LevelLoader.OnLastLevel() == false)
        {
            var activeControlPanel = GetControlContainerAndDisableOthers(CurrentLevel.controlPanelType);

            SetupLevel();

            yield return StartCoroutine(ApplyLevelObstaclesCoroutine());
        
            //TODO Wait for button press
            DisplayText?.Invoke("Press Button to Start");

            yield return new WaitUntil(() => _startButton.InputValue >= 1f);
        
            //TODO Should we have a countdown here to start??
            yield return StartCoroutine(CountdownCoroutine(0f));

            //------------------------------------------------//
            var levelWaitTime = CurrentLevel.levelTime;
            var layers = CurrentLevel.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = CurrentLevel.layers[i];
                //Allow the player adjust controls
                yield return StartCoroutine(CountdownCoroutine(levelWaitTime));
                
                //Get Inputs
                //------------------------------------------------//
                var (position, rotation, scale) = GetAllTransformations(layer, activeControlPanel);

                //Create New Object & Apply Transformations
                //------------------------------------------------//
                var newTransform = GetGeneratedLayerTransform(i, layer);
                //TODO Consider animating the position & the rotation as well!
                newTransform.position = position;
                newTransform.rotation = quaternion.Euler(rotation);

                yield return StartCoroutine(ScaleCoroutine(newTransform, Vector3.zero,  scale, 0.25f));
                
                yield return new WaitForSeconds(1f);
                
                //Reparent object to Container
                //------------------------------------------------//
                newTransform.SetParent(_containerInstance);
                
                //Move the Container down
                //------------------------------------------------//
                var containerCurrentPosition = _containerInstance.position;
                var endPosition = containerCurrentPosition + Vector3.down * CurrentLevel.yScale;
                yield return StartCoroutine(MoveToPositionCoroutine(_containerInstance, containerCurrentPosition, endPosition, 0.25f));
                //------------------------------------------------//
                
                yield return StartCoroutine(ShakeTheWorldCoroutine(activeControlPanel));
            }

            //------------------------------------------------//
            
            //TODO Calculate the score
            
            yield return StartCoroutine(DisplayResultCoroutine());

            CleanupLevel();
        
            LevelLoader.LoadNextLevel();
            
            //TODO Shake up the Level
            

        }

        //TODO Finish the Game
    }


    //Level Setup Functions
    //============================================================================================================//

    private void SetupLevel()
    {
        _silhouetteGenerator?.BuildSilhouette();
        _containerInstance = CreateNewContainer();
    }

    private Transform CreateNewContainer()
    {
        var newContainer = new GameObject($"---Level [{LevelLoader.CurrentLevelIndex.ToString()}] Container---").transform;
        newContainer.position = containerStartPosition;

        return newContainer;
    }

    private static (Vector3 position, Vector3 rotation, Vector3 scale) GetAllTransformations(LayerData layerData, ControlPanelContainer controlPanel)
    {
        var currentLevel = CurrentLevel;
        
        var outPosition = Vector3.zero;
        var outRotation = Vector3.zero;
        var outScale = new Vector3(0f, layerData.localScale.y, 0f);

        var levelMinPosition = CurrentLevel.MinPosition;
        var levelMaxPosition = CurrentLevel.MaxPosition;
        
        var controlValues = controlPanel.GetControlValues();

        //Go through each of the controls, then apply their values based on what they should be effecting
        for (int i = 0; i < controlValues.Length; i++)
        {
            var (control, value) = controlValues[i];

            switch (control)
            {
                case CONTROLS.SCALE:
                    var yScale = currentLevel.yScale;
                    outScale = new Vector3(layerData.localScale.x * value, yScale, layerData.localScale.y * value);
                    break;
                case CONTROLS.X_SCALE:
                    outScale.x = layerData.localScale.x * value;
                    break;
                case CONTROLS.Z_SCALE:
                    outScale.z = layerData.localScale.x * value;
                    break;
                case CONTROLS.X_POS:
                    outPosition.x = Mathf.Lerp(levelMinPosition.x, levelMaxPosition.x, value);
                    break;
                case CONTROLS.Z_POS:
                    outPosition.z = Mathf.Lerp(levelMinPosition.z, levelMaxPosition.z, value);
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
    private void ProcessPrintToWorld()
    {
        throw new NotImplementedException();
    }
    
    private void CleanupLevel()
    {
        //TODO Store Container?
        throw new NotImplementedException();
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
        var foundIndex = -1;
        for (int i = 0; i < controlPanelContainers.Length; i++)
        {
            var matches = controlPanelContainers[i].controlPanelType == controlPanelType;

            controlPanelContainers[i].SetActive(matches);
            if (matches)
                foundIndex = i;
        }

        if(foundIndex < 0)
            throw new Exception();

        return controlPanelContainers[foundIndex];
    }
    
    /*private ControlPanelContainer GetControlContainer(CONTROL_PANEL_TYPE controlPanelType)
    {
        for (int i = 0; i < controlPanelContainers.Length; i++)
        {
            if (controlPanelContainers[i].controlPanelType == controlPanelType)
                return controlPanelContainers[i];
        }

        throw new Exception();
    }*/

    //Coroutine
    //============================================================================================================//

    private IEnumerator ApplyLevelObstaclesCoroutine()
    {
        yield break;
    }

    private IEnumerator DisplayResultCoroutine()
    {
        //Replace world Objects
        ProcessPrintToWorld();
        //TODO Enable the RenderTexture/Grab screen capture
        //Call to Display the UI to the player
        OnLevelComplete?.Invoke();
        //TODO Wait for Continue to be pressed
        throw new NotImplementedException();
    }

    private IEnumerator ShakeTheWorldCoroutine(ControlPanelContainer controlPanel)
    {
        controlPanel.ShuffleControls();
        
        //TODO Apply a Camera Shake
        
        throw new NotImplementedException();
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

            target.transform.localScale = Vector3.Lerp(startScale, targetScale, animationCurve.Evaluate(dt));
                    
            yield return null;
        }
    }

    private IEnumerator MoveToPositionCoroutine(Transform target, Vector3 startPosition, Vector3 endPosition, float time)
    {
        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;
            

            target.transform.position = Vector3.Lerp(startPosition, endPosition, animationCurve.Evaluate(dt));
                    
            yield return null;
        }
    }

}