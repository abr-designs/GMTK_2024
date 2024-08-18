using Cinemachine;
using GameInput;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraMouseFirstPersonRotation : MonoBehaviour {

    [SerializeField, Header("References")]
    private CinemachineVirtualCamera virtualCamera;

    [Header("Variables")]
    public float sensitivityX = 0.25f; // Sensitivity for the X axis rotation
    public float sensitivityY = 0.25f; // Sensitivity for the Y axis rotation

    private float rotationX = 0f;
    private float rotationY = 0f;

    public float minX = -90f; // Minimum X rotation
    public float maxX = 90f;  // Maximum X rotation

    public float minY = -80f; // Minimum Y rotation
    public float maxY = 80f;  // Maximum Y rotation

    private void OnEnable() {
        GameInputDelegator.OnMouseMove += GameInputDelegator_OnMouseMove;
    }

    private void OnDisable() {
        GameInputDelegator.OnMouseMove -= GameInputDelegator_OnMouseMove;
    }

    private void Start()
    {
        Assert.IsNotNull(virtualCamera);
    }

    private void GameInputDelegator_OnMouseMove(Vector2 delta) {

        // Update rotation values based on mouse input
        rotationX -= delta.y * sensitivityX; // Invert to rotate correctly with mouse movement
        rotationY += delta.x * sensitivityY;

        // Clamp the X rotation to prevent flipping
        rotationX = Mathf.Clamp(rotationX, minX, maxX);
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        // Apply the rotation to the object
        virtualCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}