using System;
using System.Collections;
using Interactables;
using Interfaces;
using Levels;
using Levels.Enums;
using UnityEngine;
using UnityEngine.Assertions;

public class LayerSpawner : MonoBehaviour, ISpawnLayers
{
    public Transform GeneratedTransform { get; private set; }
    public Vector3 SpawnLocation { get; set; }

    [SerializeField, Min(0f), Header("Scale Animations")]
    private float animationTime;
    [SerializeField]
    private AnimationCurve scaleCurve;

    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(scaleCurve);
    }


    //ISpawnLayers Implementation
    //============================================================================================================//
    
    public Coroutine SpawnLayer(int layerIndex, LayerData layerData, LevelDataContainer currentLevel, ControlPanelContainer controlPanelContainer)
    {
        return StartCoroutine(SpawnLayerCoroutine(layerIndex, layerData, currentLevel, controlPanelContainer));
    }
    
    //============================================================================================================//

    private IEnumerator SpawnLayerCoroutine(int layerIndex, LayerData layerData, LevelDataContainer currentLevel, ControlPanelContainer controlPanelContainer)
    {
        //Get Inputs
        //------------------------------------------------//
        var (position, rotation, scale) = GetAllTransformations(layerData, controlPanelContainer, currentLevel);

        //Create New Object & Apply Transformations
        //------------------------------------------------//
        GeneratedTransform = GetGeneratedLayerTransform(layerIndex, layerData);
        //TODO Consider animating the position & the rotation as well!
        GeneratedTransform.position = position + SpawnLocation;
        GeneratedTransform.rotation = Quaternion.Euler(rotation);

        yield return StartCoroutine(ScaleCoroutine(GeneratedTransform, Vector3.zero, scale, animationTime));

        yield return new WaitForSeconds(1f);
    }

    //============================================================================================================//
    
    private Transform GetGeneratedLayerTransform(int layer, LayerData layerData)
    {
        var newLayerTransform = Instantiate(layerData.printLayerShape.prefab);
        newLayerTransform.gameObject.name = $"Layer_[{layer.ToString()}] {layerData.printLayerShape.prefab.name}";

        newLayerTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterial = layerData.Material;
        
        return newLayerTransform;
    }
    private static (Vector3 position, Vector3 rotation, Vector3 scale) GetAllTransformations(LayerData layerData, ControlPanelContainer controlPanel, LevelDataContainer currentLevel)
    {
        var maxScale = currentLevel.maxScale;
        
        var outPosition = Vector3.zero;
        var outRotation = Vector3.zero;
        var outScale = new Vector3(0f, layerData.localScale.y, 0f);

        var levelMinPosition = currentLevel.MinPosition;
        var levelMaxPosition = currentLevel.MaxPosition;
        
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

    //Coroutines
    //============================================================================================================//
    
    private IEnumerator ScaleCoroutine(Transform target, Vector3 startScale, Vector3 targetScale, float time)
    {
        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;

            target.transform.localScale = Vector3.Lerp(startScale, targetScale, scaleCurve.Evaluate(dt));
                    
            yield return null;
        }
    }
}
