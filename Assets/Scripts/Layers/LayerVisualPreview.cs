using System;
using Interactables;
using Levels;
using Levels.Enums;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

namespace Layers
{
    public class LayerVisualPreview : MonoBehaviour
    {

        [SerializeField]
        private Vector3 previewPosition;

        [SerializeField]
        private Material previewMaterial;

        private bool _updatingPreview;
    
        private Transform _meshPreviewTransform;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private LayerData _currentLayer;
        private LevelDataContainer _levelDataContainer;
        private ControlPanelContainer _controlPanelContainer;

        //============================================================================================================//

        private void OnEnable()
        {
            GameManager.OnLayerSelected += OnLayerSelected;
            GameManager.OnLayerStarted += OnLayerStarted;
            GameManager.OnLayerFinished += OnLayerFinished;
        }

        // Start is called before the first frame update
        void Start()
        {
            var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _meshPreviewTransform = temp.transform;
            _meshFilter = temp.GetComponent<MeshFilter>();
            _meshRenderer = temp.GetComponent<MeshRenderer>();

            _meshRenderer.sharedMaterial = previewMaterial;
        
            Assert.IsNotNull(temp);
            Assert.IsNotNull(_meshPreviewTransform);
            Assert.IsNotNull(_meshFilter);
            Assert.IsNotNull(_meshRenderer);
        
            _meshPreviewTransform.gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (_updatingPreview == false)
                return;

            var (position, rotation, scale) = LayerMathHelper.GetAllTransformations(_currentLayer, _controlPanelContainer, _levelDataContainer);
        
            //TODO Consider animating the position & the rotation as well!
            _meshPreviewTransform.position = previewPosition + position;
            _meshPreviewTransform.rotation = Quaternion.Euler(rotation);

            _meshPreviewTransform.localScale = scale;

        }
    
        private void OnDisable()
        {
            GameManager.OnLayerSelected -= OnLayerSelected;
            GameManager.OnLayerStarted -= OnLayerStarted;
            GameManager.OnLayerFinished -= OnLayerFinished;
        }
        //============================================================================================================//
    
        //Callbacks
        //============================================================================================================//

        private void OnLayerSelected(int layerIndex)
        {
            _controlPanelContainer = GameManager.ActiveControlPanel;
        
            _levelDataContainer = LevelLoader.CurrentLevelDataContainer;
            _currentLayer = _levelDataContainer.layers[layerIndex];
            _meshFilter.sharedMesh = _currentLayer.printLayerShape.Mesh;
        }
    
        private void OnLayerStarted()
        {
            _updatingPreview = true;
            _meshPreviewTransform.gameObject.SetActive(true);
        }
    
        private void OnLayerFinished()
        {
            _updatingPreview = false;
            _meshPreviewTransform.position = previewPosition;
            _meshPreviewTransform.gameObject.SetActive(false);
        }
    
        //============================================================================================================//

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(previewPosition, Vector3.one);
        }


#endif
    }
}
