using System;
using System.Collections;
using Interactables;
using Interfaces;
using Levels;
using Levels.Enums;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

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
        var (position, rotation, scale) = LayerMathHelper.GetAllTransformations(layerData, controlPanelContainer, currentLevel);

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
