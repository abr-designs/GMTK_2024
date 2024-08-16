using UnityEngine;

public class CameraScrollZoom : MonoBehaviour {
    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;       // Speed of zooming
    public float minZ = -50f;           // Minimum z position
    public float maxZ = -10f;           // Maximum z position

    private Camera cam;


    public Transform target;
    Vector3 offset = Vector3.zero;

    void Start() {
        cam = GetComponent<Camera>();
        if (cam == null) {
            Debug.LogError("ScrollZoom script must be attached to a Camera.");
        }


        offset = transform.position - target.position;
    }

    void Update() {
        if (cam == null) return;

        // Get the scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f) {
            // Calculate new position
            Vector3 position = transform.localPosition;
            position.z += scrollInput * zoomSpeed * Time.deltaTime;

            // Clamp the z position to the specified range
            position.z = Mathf.Clamp(position.z, minZ, maxZ);


            //transform.position = target.position + offset;

            // Apply the new position
            transform.localPosition = position + target.transform.position + offset;
        }
    }
}
