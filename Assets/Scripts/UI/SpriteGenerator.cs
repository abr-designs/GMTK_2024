using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Interfaces;
using Levels;
using UnityEngine;

public enum SPRITE_VIEW {
    SIDE_VIEW = 0,
    ISO_VIEW
}

public class SpriteGenerator : MonoBehaviour, IGenerateSilhouette
{
    [SerializeField]
    private LevelDataContainer _debugLevel;

    [SerializeField]
    private GameObject layerPrefab;

    [SerializeField]
    private Camera spriteCamera;
    [SerializeField]
    private Transform cameraTargetContainer;

    public void BuildSilhouette(LevelDataContainer levelDataContainer)
    {
        BuildLevel(levelDataContainer);
    }

    // Instantiate each layer
    private void BuildLevel(LevelDataContainer levelDataContainer) {

        // Clear existing prefabs from camera
        foreach(Transform t in cameraTargetContainer.transform) {
            t.gameObject.SetActive(false);
            Destroy(t.gameObject);
        }

        for(int i = 0; i < levelDataContainer.layers.Length; i++) {
            
            var layerObject = Instantiate(levelDataContainer.layers[i].printLayerShape.prefab, cameraTargetContainer);
            var layerData = levelDataContainer.layers[i];
            layerObject.gameObject.layer = spriteCamera.gameObject.layer;

            var pos = /*basePosition + */ new Vector3(layerData.localPosition.x, levelDataContainer.yScale * i, layerData.localPosition.y);
            var scale = new Vector3(layerData.localScale.x, levelDataContainer.yScale, layerData.localScale.y);
            //var color = layerData.Material == null ? Color.magenta : layerData.Material.color;

            layerObject.localScale = scale;
            layerObject.localPosition = pos;
            //layerObject.GetComponent<MeshRenderer>().material.color = color;
        }

        // TODO -- remove
        CaptureView(levelDataContainer, SPRITE_VIEW.ISO_VIEW);

    }

    public void CaptureView(LevelDataContainer levelDataContainer, SPRITE_VIEW viewType) {
        
        spriteCamera.enabled = true;

        if(viewType == SPRITE_VIEW.SIDE_VIEW)
        {
            spriteCamera.transform.localPosition = new Vector3(0,((levelDataContainer.layers.Length / 2f) - 0.5f),-5f);
            spriteCamera.transform.localRotation = Quaternion.identity;
            spriteCamera.orthographicSize = levelDataContainer.layers.Length * levelDataContainer.yScale;
        } else if (viewType == SPRITE_VIEW.ISO_VIEW)
        {

            spriteCamera.transform.localPosition = Vector3.zero;
            spriteCamera.transform.localPosition = new Vector3(-5,5 + ((levelDataContainer.layers.Length / 2f) - 0.5f),-5);
            spriteCamera.transform.localRotation = Quaternion.Euler(new Vector3(35.264f,45,0));
            spriteCamera.orthographicSize = (levelDataContainer.layers.Length + 1) * levelDataContainer.yScale;
        }

        // Take picture (renderTexture)
        spriteCamera.Render();

        // Turn off camera
        spriteCamera.enabled = false;

    }

    // Start is called before the first frame update
    void Start()
    {
    }

  

    [ContextMenu("Sprite Preview")]
    void SpritePreview()
    {
        BuildSilhouette(_debugLevel);
    }
}
