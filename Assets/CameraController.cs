using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;       // Speed of zooming
    public float minZ = -50f;           // Minimum z position
    public float maxZ = -10f;           // Maximum z position

    public Transform target;
    Vector3 offset = Vector3.zero;

    private void Start() {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f) {
            // Calculate new position
            Vector3 zoomPosition = transform.localPosition;
            zoomPosition.z += scrollInput * zoomSpeed * Time.deltaTime;

            // Clamp the z position to the specified range
            zoomPosition.z = Mathf.Clamp(zoomPosition.z, minZ, maxZ);
            offset.z = zoomPosition.z;
        }

        transform.position = target.position + offset;
    }
}
