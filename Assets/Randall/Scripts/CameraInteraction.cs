using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInteraction : MonoBehaviour {

    public Camera camera; // Reference to the main camera
    public float maxRayDistance = 100f; // Maximum distance the ray should check
    public LayerMask layerMask; // Layer mask to specify which objects the ray should interact with

    public float sensitivityY = 5f; // Sensitivity for the Y axis rotation
    public float inputDampener = 5f;

    LeverController interactingController;

    private void Start() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0)) {
            // Create a ray from the center of the camera's view
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            // Perform the raycast and check if it hits an object with a BoxCollider
            if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask)) {
                // Check if the hit object has a BoxCollider
                if (hit.collider.GetComponent<LeverController>() != null) {
                    Debug.Log("Hit an object with a BoxCollider: " + hit.collider.gameObject.name);
                    interactingController = hit.collider.GetComponent<LeverController>();
                    interactingController.SetIsInteracting(true);
                }
                else {
                    Debug.Log("Hit an object, but it doesn't have a BoxCollider.");
                }
            }
            else {
                Debug.Log("No hit detected.");
            }
        }

        // Check if moving interactableObject
        if (interactingController != null && Input.GetMouseButton(0)) {

            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

            float dampening = 1f / inputDampener;

            interactingController.AdjustValue(mouseY * dampening);
        }

        // Check if the left mouse button is unclicked
        if (interactingController != null && Input.GetMouseButtonUp(0)) {
            interactingController.SetIsInteracting(false);
        }
    }
}
