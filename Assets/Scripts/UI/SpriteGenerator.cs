using Interactables;
using Interfaces;
using Levels;
using UnityEngine;

namespace UI
{
    public class SpriteGenerator : MonoBehaviour, IGenerateSilhouette
    {
        private  enum SPRITE_VIEW 
        {
            SIDE_VIEW = 0,
            ISO_VIEW
        }
        [SerializeField] private LevelDataContainer _debugLevelDataContainer;

        [SerializeField] private GameObject layerPrefab;

        [SerializeField] private Camera spriteCamera;
        [SerializeField] private Transform cameraTargetContainer;

        private int _activeLayerIndex = 0;
        private SPRITE_VIEW _currentView = SPRITE_VIEW.SIDE_VIEW;
        private LevelDataContainer _currentLevel;

        [SerializeField] private ButtonInteractable[] toggleButtons;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            GameManager.OnLayerSelected += OnLayerSelected;
            for (int i = 0; i < toggleButtons.Length; i++)
            {
                if(toggleButtons[i] == null)
                    continue;
                toggleButtons[i].OnButtonPressed += ToggleView;
            }
        }

        private void OnDisable()
        {
            GameManager.OnLayerSelected -= OnLayerSelected;
            for (int i = 0; i < toggleButtons.Length; i++)
            {
                if(toggleButtons[i] == null)
                    continue;
                toggleButtons[i].OnButtonPressed -= ToggleView;
            }
        }
        //============================================================================================================//

        public void BuildSilhouette(LevelDataContainer levelDataContainer)
        {
            _currentLevel = levelDataContainer;
            BuildLevel();
        }

        // Instantiate each layer
        private void BuildLevel()
        {

            LevelDataContainer levelDataContainer = _currentLevel;

            // Clear existing prefabs from camera
            foreach (Transform t in cameraTargetContainer.transform)
            {
                t.gameObject.SetActive(false);
                Destroy(t.gameObject);
            }

            for (int i = 0; i < levelDataContainer.layers.Length; i++)
            {

                var layerObject = Instantiate(levelDataContainer.layers[i].printLayerShape.prefab, cameraTargetContainer);
                var layerData = levelDataContainer.layers[i];
                layerObject.gameObject.layer = spriteCamera.gameObject.layer;

                var pos = /*basePosition + */ new Vector3(layerData.localPosition.x, levelDataContainer.yScale * i,
                    layerData.localPosition.y);
                var scale = new Vector3(layerData.localScale.x, levelDataContainer.yScale, layerData.localScale.y);
                //var color = layerData.Material == null ? Color.magenta : layerData.Material.color;

                layerObject.localScale = scale;
                layerObject.localPosition = pos;
                //layerObject.GetComponent<MeshRenderer>().material.color = color;
            }

            UpdateView();

        }

        //Callbacks
        //============================================================================================================//
    
        private void ToggleView()
        {
            // why doesn't this work!!!!
            //_currentView = ((int)_currentView + 1) % (System.Enum.GetValues(typeof(SPRITE_VIEW)).Length);
            _currentView = (_currentView == SPRITE_VIEW.SIDE_VIEW) ? SPRITE_VIEW.ISO_VIEW : SPRITE_VIEW.SIDE_VIEW;
            UpdateView();
        }

        private void UpdateView()
        {
            if (!_currentLevel) return;

            // Alter materials based on current layer
            for (int i = 0; i < _currentLevel.layers.Length; i++)
            {
                var child = cameraTargetContainer.GetChild(i);
                var renderer = child.GetComponent<MeshRenderer>();
                if (_activeLayerIndex == i)
                {
                    renderer.enabled = true;
                    // Set to green
                    // TODO -- use custom material?
                    renderer.material.color = Color.green;
                }
                /*else if(_activeLayerIndex < i)
            {
                renderer.enabled = false;
            }*/ else
                {
                    renderer.enabled = true;
                    renderer.material.color = Color.gray;
                }

            }


            spriteCamera.enabled = true;

            if (_currentView == SPRITE_VIEW.SIDE_VIEW)
            {
                spriteCamera.transform.localPosition = new Vector3(0, ((_currentLevel.layers.Length / 2f) - 0.5f), -5f);
                spriteCamera.transform.localRotation = Quaternion.identity;
                spriteCamera.orthographicSize = _currentLevel.layers.Length * _currentLevel.yScale;
            }
            else if (_currentView == SPRITE_VIEW.ISO_VIEW)
            {

                spriteCamera.transform.localPosition = Vector3.zero;
                spriteCamera.transform.localPosition = new Vector3(-5, 5 + ((_currentLevel.layers.Length / 2f) - 0.5f), -5);
                spriteCamera.transform.localRotation = Quaternion.Euler(new Vector3(35.264f, 45, 0));
                spriteCamera.orthographicSize = (_currentLevel.layers.Length + 1) * _currentLevel.yScale;
            }

            // Take picture (renderTexture)
            spriteCamera.Render();

            // Turn off camera
            spriteCamera.enabled = false;

        }

        private void OnLayerSelected(int index)
        {
            _activeLayerIndex = index;
            UpdateView();
        }

        //Context Menu Debug Items
        //============================================================================================================//
    

        [ContextMenu("Sprite Preview")]
        private void SpritePreview()
        {
            BuildSilhouette(_debugLevelDataContainer);
        }

        [ContextMenu("Debug - Increase Layer")]
        private void IncreaseLayer()
        {
            OnLayerSelected(_activeLayerIndex + 1);
        }

        [ContextMenu("Debug - Toggle View")]
        private void ToggleViewDebug()
        {
            ToggleView();
        }

//============================================================================================================//
    }
}
