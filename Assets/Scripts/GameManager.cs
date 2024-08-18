using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Interactables;
using Interactables.Enums;
using Interfaces;
using Levels;
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
    public static event Action OnLayerFinished;
    public static event Action<string> DisplayText;
    public static event Action<string> DisplayResultText;
    public static event Action<float> OnCountdown;

    private Dictionary<string, GameObject> _generatedPlayerContent;

    [SerializeField]
    private Vector3 spawnPosition;
    private Transform _containerInstance;
    private static LevelDataContainer CurrentLevel => LevelLoader.CurrentLevelDataContainer;

    public static ControlPanelContainer ActiveControlPanel { get; private set; }

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

    [SerializeField, Min(0)]
    private float worldShakeTime;

    [SerializeField, Header("Animation Boys")]
    private WaitForAnimationBase controlPanel;
    [SerializeField]
    private WaitForAnimationBase startButton;
    [SerializeField]
    private WaitForAnimationBase tvScreenAnimation;

    private IDisplayDialog _displayDialog;
    private IGenerateSilhouette _silhouetteGenerator;
    private ICreateWorldReplacers _createWorldReplacers;
    private CinemachineImpulseSource _impulseSource;
    private ISpawnLayers _spawnLayers;
    private IMoveLayers _moveLayers;
    private IDisplayResults _resultsDisplay;

    //============================================================================================================//
    
    public void Start()
    {
        controlPanelContainers = FindObjectsOfType<ControlPanelContainer>();
        _generatedPlayerContent = new Dictionary<string, GameObject>();
        LevelLoader.LoadFirstLevel();

        //Find from Children
        //------------------------------------------------//
        _displayDialog = GetComponentInChildren<IDisplayDialog>();
        _silhouetteGenerator = GetComponentInChildren<IGenerateSilhouette>();
        _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        _spawnLayers = GetComponentInParent<ISpawnLayers>();
        _spawnLayers.SpawnLocation = spawnPosition;
        _moveLayers = GetComponentInParent<IMoveLayers>();
        _resultsDisplay = GetComponentInChildren<IDisplayResults>();
        
        Assert.IsNotNull(_startButton);
        Assert.IsNotNull(_spawnLayers);
        Assert.IsNotNull(_moveLayers);

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
            ActiveControlPanel = activeControlPanel;
            
            SetupLevel(); 
            
            OnLevelStarted?.Invoke();

            yield return StartCoroutine(ApplyLevelObstaclesCoroutine());
            
            if (_displayDialog != null)
                yield return StartCoroutine(_displayDialog.DisplayDialogCoroutine(CurrentLevel.levelScript));

            //TODO Wait for button press
            DisplayText?.Invoke("Press Button to Start");
            
            if (startButton)
                yield return startButton.DoAnimation(animationTime, ANIM_DIR.TO_START);

            yield return new WaitUntil(() => _startButton.InputValue >= 1f);
            
            if (startButton)
                startButton.DoAnimation(animationTime, ANIM_DIR.TO_END);
            
            if (tvScreenAnimation)
                tvScreenAnimation.DoAnimation(animationTime, ANIM_DIR.TO_START);

            DisplayText?.Invoke("Get Ready!");
            
            //Level
            yield return StartCoroutine(CountdownCoroutine(levelStartCountdownTime));
            
            if (controlPanel)
                yield return controlPanel.DoAnimation(animationTime, ANIM_DIR.TO_START);

            //------------------------------------------------//
            var levelWaitTime = CurrentLevel.levelTime;
            var layers = CurrentLevel.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                OnLayerSelected?.Invoke(i);
                DisplayText?.Invoke("Next Layer!");

                var layer = CurrentLevel.layers[i];

                OnLayerStarted?.Invoke();
                //Allow the player adjust controls
                yield return StartCoroutine(CountdownCoroutine(levelWaitTime));
                
                OnLayerFinished?.Invoke();

                yield return _spawnLayers.SpawnLayer(i, layer, CurrentLevel, activeControlPanel);

                //Reparent object to Container
                //------------------------------------------------//
                var newTransform = _spawnLayers.GeneratedTransform;
                newTransform.SetParent(_containerInstance);

                yield return _moveLayers.MoveLayer(i, CurrentLevel.yScale, newTransform, _containerInstance);


                _impulseSource?.GenerateImpulse();
                OnWorldShake?.Invoke(worldShakeTime);
            }

            //------------------------------------------------//

            yield return _resultsDisplay.Display(() =>
            {
                var currentLevel = CurrentLevel;

                _generatedPlayerContent.TryAdd(currentLevel.worldPlaceTag, _containerInstance.gameObject);
                _createWorldReplacers?.CreateWorldVersion(currentLevel.worldPlaceTag, currentLevel.outputScale, _containerInstance.gameObject);
                //------------------------------------------------//

                //Call to Display the UI to the player
                OnLevelComplete?.Invoke();
                DisplayResultText?.Invoke(CurrentLevel.levelCompleteScript);
            });

            CleanupLevel();

            //If we've just wrapped the last lever, exit, and begin finishing the game
            if(LevelLoader.OnLastLevel())
                break;
            
            LevelLoader.LoadNextLevel();
            
            if (controlPanel)
                yield return controlPanel.DoAnimation(animationTime, ANIM_DIR.TO_END);
            
            
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
    
    //Level End Functions
    //============================================================================================================//

    //This function should take the generated Object, and apply it to all of the equivalent tagged world objects
    
    private void CleanupLevel()
    {
        _containerInstance = null;
    }

    //Helper Functions
    //============================================================================================================//

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

}