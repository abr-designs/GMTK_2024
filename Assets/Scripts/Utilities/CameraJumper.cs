using UnityEngine;
using UnityEngine.Assertions;

namespace Utilities
{
    [RequireComponent(typeof(Camera))]
    public class CameraJumper : MonoBehaviour
    {
        [SerializeField]
        private Camera camera;

        [SerializeField]
        private Transform[] cameraPositionTransforms;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            GameManager.OnLevelComplete += OnLevelComplete;
            GameManager.OnLevelStarted += OnLevelStarted;
        }

        // Start is called before the first frame update
        private void Start()
        {
            camera = GetComponent<Camera>();
            Assert.IsNotNull(cameraPositionTransforms);
            Assert.IsFalse(cameraPositionTransforms.Length == 0);
            Assert.IsNotNull(camera);
        }

        private void OnDisable()
        {
            GameManager.OnLevelComplete -= OnLevelComplete;
            GameManager.OnLevelStarted -= OnLevelStarted;
        }

        //Camera Jumper Functions
        //============================================================================================================//

        private void SetNewCameraPosition()
        {
            var targetTransform = cameraPositionTransforms.PickRandomElement();

            camera.transform.position = targetTransform.position;
            camera.transform.rotation = targetTransform.rotation;
        }


        //Callbacks
        //============================================================================================================//

        private void OnLevelStarted()
        {
            camera.enabled = false;
        }

        private void OnLevelComplete(int score)
        {
            SetNewCameraPosition();
            camera.enabled = true;
        }

        //============================================================================================================//
    }
}
