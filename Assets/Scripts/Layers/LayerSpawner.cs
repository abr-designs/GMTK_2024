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

    // Bottom of the object will be here
    public Vector3 SpawnLocation;
    /* Filament effect */
    [SerializeField]
    private Transform nozzlePosition;
    [SerializeField]
    private GameObject filamentPrefab;
    private ParticleSystem _filamentStreamVFX;
    [SerializeField]
    private Material PLA_Fill_Material;


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
        GeneratedTransform.localScale = Vector3.zero; // hide when starting

        PLA_Fill_Material.SetColor("_BaseColor",layerData.Material.color);
        PLA_Fill_Material.SetFloat("_ObjectHeight", currentLevel.yScale);
        GeneratedTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterial = PLA_Fill_Material;
        // Start filament
        toggleFilament(true, layerData.Material.color);
        // Wait a bit for filament
        yield return new WaitForSeconds(0.7f);

        yield return StartCoroutine(ScaleCoroutine(GeneratedTransform, Vector3.zero, scale, animationTime));

        toggleFilament(false);
        // Wait a bit for filament
        yield return new WaitForSeconds(0.7f);


        // Restore material
        GeneratedTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterial = layerData.Material;

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
        Vector3 startPosition = target.transform.position;

        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;

            target.transform.localScale = Vector3.Lerp(startScale, targetScale, scaleCurve.Evaluate(dt));

            target.transform.position = startPosition + Vector3.up * (target.transform.localScale.y /2f);

            PLA_Fill_Material.SetFloat("_PercentFill", dt);
                    
            yield return null;
        }

        // Ensure final values are correct
        target.transform.localScale = targetScale;
        target.transform.position = startPosition + Vector3.up * (target.transform.localScale.y /2f);

    }

    private void toggleFilament(bool isOn, Color? fColor = null) {
        if(!_filamentStreamVFX)
        {
            var filament = Instantiate(filamentPrefab, nozzlePosition);
            _filamentStreamVFX = filament.GetComponent<ParticleSystem>();
        }
        
        if(isOn)
        {            
            _filamentStreamVFX.GetComponent<ParticleSystemRenderer>().sharedMaterial.color = fColor.GetValueOrDefault();
            _filamentStreamVFX.Play();
        }
        else
            _filamentStreamVFX.Stop();
    }
}
