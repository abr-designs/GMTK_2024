using Levels;
using UnityEngine;

namespace World
{
    public class WorldTagObject : MonoBehaviour
    {
        [SerializeField]
        public string worldTag;

#if UNITY_EDITOR

        private static LevelLoader _levelLoader;
        private LevelDataContainer _levelDataContainer;
        
        private void OnDrawGizmos()
        {
            if (string.IsNullOrWhiteSpace(worldTag))
                return;
            
            if (_levelLoader == null)
                _levelLoader = FindObjectOfType<LevelLoader>();

            if (_levelDataContainer == null || _levelDataContainer.worldPlaceTag.Equals(worldTag) == false)
            {
                _levelDataContainer = null;
                //TODO Try to find the world tag
                var containers = _levelLoader.GetDataContainers();
                for (int i = 0; i < containers.Length; i++)
                {
                    if (containers[i].worldPlaceTag.Equals(worldTag) == false)
                        continue;

                    _levelDataContainer = containers[i];
                    break;
                }
            }

            if (_levelDataContainer == null)
                return;
            
            //var rotation = transform.rotation;
            //var position = transform.position;
            var layers = _levelDataContainer.layers;
            var LayerCount = layers.Length;
            var yScale = _levelDataContainer.yScale;

            transform.localScale = Vector3.one * _levelDataContainer.outputScale;
            
            Gizmos.matrix = transform.localToWorldMatrix;
            
            for (int i = 0; i < LayerCount; i++)
            {
                var layerData = layers[i];

                if (layerData == null || layerData.printLayerShape == null)
                    continue;

                var pos = new Vector3(layerData.localPosition.x, yScale * i, layerData.localPosition.y);
                var scale = new Vector3(layerData.localScale.x, yScale, layerData.localScale.y);
                var color = layerData.Material == null ? Color.magenta : layerData.Material.color;
                Gizmos.color = color;
                Gizmos.DrawMesh(layerData.printLayerShape.Mesh,
                    pos,
                    Quaternion.Euler(0f, layerData.yRotation, 0f),
                    scale);
                Gizmos.color =  new Color(color.r - 0.15f,color.g - 0.15f,color.b - 0.15f);
                Gizmos.DrawWireMesh(layerData.printLayerShape.Mesh,
                    pos,
                    Quaternion.Euler(0f, layerData.yRotation, 0f),
                    scale);
            }
        }
#endif
    }
}