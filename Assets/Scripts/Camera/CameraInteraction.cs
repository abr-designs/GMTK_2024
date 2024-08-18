using System;
using GameInput;
using UnityEngine;
using Printer;
using Interactables;

public class CameraInteraction : MonoBehaviour
{
    public static event Action<Vector3, Transform> OnInteractionStarted;
    [Header("Object References")] 
    Camera cabCamera; // main camera
    [SerializeField] private CameraMouseFirstPersonRotation cameraMouseFirstPersonRotation;

    [Header("Variables")]
    public float maxRayDistance = 100f; // Maximum distance the ray should check
    public LayerMask layerMask; // Layer mask to specify which objects the ray should interact with

    public float sensitivityY = 1f; // Sensitivity for the Y axis rotation

    InteractableInputControl interactingController;

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
            CheckForControlInteraction();
            cameraMouseFirstPersonRotation.enabled = false;
        }
        else {
            CheckForReleaseInteraction();
            cameraMouseFirstPersonRotation.enabled = true;
        }

    }

    private void GameInputDelegator_OnMouseMove(Vector2 delta) {
        CheckForMoveControlInteraction(delta);
    }

    private void Start() {
        cabCamera = Camera.main;
    }

    private void CheckForControlInteraction() {

        if (interactingController != null) return;

        // Create a ray from the center of the camera's view
        Ray ray = cabCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Perform the raycast and check if it hits an object with a BoxCollider
        if (!Physics.Raycast(ray, out hit, maxRayDistance, layerMask)) 
            return;
        
        
        // Check if the hit object has a PrinterInputContol
        if (hit.collider.GetComponent<InteractableInputControl>() != null) {
            interactingController = hit.collider.GetComponent<InteractableInputControl>();
            interactingController.SetIsInteracting(true);
                
            OnInteractionStarted?.Invoke(hit.point, interactingController.InteractionTransform);
        }

    }

    private void CheckForReleaseInteraction() {
        if (interactingController == null) return;
        interactingController.SetIsInteracting(false);
        interactingController = null;
        
        OnInteractionStarted?.Invoke(default, null);
    }

    private void CheckForMoveControlInteraction(Vector2 delta) {
        // Check if moving interactableObject
        if (interactingController == null) return;

        float mouseY = delta.y * sensitivityY;
        interactingController.AdjustValue(mouseY);
    }


}
