using GameInput;
using UnityEngine;

public class CameraInteraction : MonoBehaviour {

    public Camera camera; // Reference to the main camera
    public float maxRayDistance = 100f; // Maximum distance the ray should check
    public LayerMask layerMask; // Layer mask to specify which objects the ray should interact with

    public float sensitivityY = 1f; // Sensitivity for the Y axis rotation

    LeverController interactingController;

    private void OnEnable() {
        GameInputDelegator.OnLeftClick += GameInputDelegator_OnLeftClick;
        GameInputDelegator.OnMouseMove += GameInputDelegator_OnMouseMove;
    }

    private void OnDisable() {
        GameInputDelegator.OnLeftClick -= GameInputDelegator_OnLeftClick;
        GameInputDelegator.OnMouseMove -= GameInputDelegator_OnMouseMove;
    }

    private void GameInputDelegator_OnLeftClick(bool isPressed) {

        if (isPressed) {
            Debug.Log("mouse down");
            CheckForControlInteraction();
        }
        else {
            Debug.Log("release");
            CheckForReleaseInteraction();
        }

    }

    private void GameInputDelegator_OnMouseMove(Vector2 delta) {
        CheckForMoveControlInteraction(delta);
    }

    private void Start() {
        camera = GetComponent<Camera>();
    }

    private void CheckForControlInteraction() {

        if (interactingController != null) return;

        // Create a ray from the center of the camera's view
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Perform the raycast and check if it hits an object with a BoxCollider
        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask)) {
            // Check if the hit object has a PrinterInputContol
            if (hit.collider.GetComponent<LeverController>() != null) {
                interactingController = hit.collider.GetComponent<LeverController>();
                interactingController.SetIsInteracting(true);
            }
        }

    }

    private void CheckForReleaseInteraction() {
        if (interactingController == null) return;
        interactingController.SetIsInteracting(false);
        interactingController = null;
    }

    private void CheckForMoveControlInteraction(Vector2 delta) {
        // Check if moving interactableObject
        if (interactingController == null) return;

        float mouseY = delta.y * sensitivityY;
        interactingController.AdjustValue(mouseY);
    }


}
