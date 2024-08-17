using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Levels;
using UnityEngine;

public class SpriteGenerator : MonoBehaviour, IGenerateSilhouette
{
    private const int MAX_SPRITES = 10;

    private SpriteRenderer[] sideSpriteRenderers = new SpriteRenderer[MAX_SPRITES];

    [SerializeField]
    private LevelDataContainer _debugLevel;

    [SerializeField]
    private GameObject layerPrefab;

    public void BuildSilhouette(LevelDataContainer levelDataContainer)
    {
        for(int i = 0; i < sideSpriteRenderers.Length; i++)
        {
            if(i + 1 <= levelDataContainer.layers.Length)
            {
                sideSpriteRenderers[i].enabled = true;
                var layer = levelDataContainer.layers[i];
                var sideViewSprite = layer.printLayerShape.sideViewSprite;
                var isoViewSprite = layer.printLayerShape.isoViewSprite;

                sideSpriteRenderers[i].sprite = sideViewSprite;
                sideSpriteRenderers[i].transform.localPosition = new Vector3(0,i);
                sideSpriteRenderers[i].transform.localScale = new Vector3(layer.localScale.x,1,1);

            } else {
                sideSpriteRenderers[i].enabled = false;
            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i< MAX_SPRITES; i++) 
        {
            var layerSprite = Instantiate(layerPrefab,this.transform);
            layerSprite.name = $"Layer{i}";
            sideSpriteRenderers[i] = layerSprite.GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Sprite Preview")]
    void SpritePreview()
    {
        BuildSilhouette(_debugLevel);
    }
}
