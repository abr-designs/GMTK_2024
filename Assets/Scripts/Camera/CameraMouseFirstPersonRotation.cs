using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraMouseFirstPersonRotation : MonoBehaviour {

    [SerializeField, Header("References")]
    private CinemachineVirtualCamera virtualCamera;

    [Header("Variables")]
    public float sensitivityX = 5f; // Sensitivity for the X axis rotation
    public float sensitivityY = 5f; // Sensitivity for the Y axis rotation

    private float rotationX = 0f;
    private float rotationY = 0f;

    public float minX = -90f; // Minimum X rotation
    public float maxX = 90f;  // Maximum X rotation

    public float minY = -80f; // Minimum Y rotation
    public float maxY = 80f;  // Maximum Y rotation

    private void Start()
    {
        Assert.IsNotNull(virtualCamera);
    }

    void Update() {
        // Get mouse delta input
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // Update rotation values based on mouse input
        rotationX -= mouseY; // Invert to rotate correctly with mouse movement
        rotationY += mouseX;

        // Clamp the X rotation to prevent flipping
        rotationX = Mathf.Clamp(rotationX, minX, maxX);
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        // Apply the rotation to the object
        virtualCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}