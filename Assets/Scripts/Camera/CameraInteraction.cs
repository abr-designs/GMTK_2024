using System;
using GameInput;
using UnityEngine;
using Printer;
using Interactables;
using UnityEngine.UIElements;

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
    public float sensitivityX = 1f; // Sensitivity for the X axis rotation

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

        // get interacting control local rotation
        Vector3 controlRotation = interactingController.transform.localRotation.eulerAngles;

        float mouseY = delta.y * sensitivityY;
        float mouseX = delta.x * sensitivityX;

        //float dotValue = Vector3.Dot(new Vector3(mouseX, mouseY, 0), controlRotation);

        Vector3[] controlRotationAxis = interactingController.GetTransformAxis();

        if (controlRotationAxis.Length == 1) {
            float sinY = Mathf.Sin((controlRotation.y + controlRotationAxis[0].y * 90f) * Mathf.Deg2Rad);
            float cosY = Mathf.Cos((controlRotation.y + controlRotationAxis[0].y * 90f) * Mathf.Deg2Rad);

            float adjustment = mouseY * cosY + mouseX * sinY;

            interactingController.AdjustValue(adjustment);
        }
        else if (controlRotationAxis.Length == 2) {

            float sinY = Mathf.Sin((controlRotation.y + controlRotationAxis[0].y * 90f) * Mathf.Deg2Rad);
            float cosY = Mathf.Cos((controlRotation.y + controlRotationAxis[0].y * 90f) * Mathf.Deg2Rad);

            float sinX = Mathf.Sin((controlRotation.y + controlRotationAxis[0].y * 90f) * Mathf.Deg2Rad);
            float cosX = Mathf.Cos((controlRotation.y + controlRotationAxis[0].y * 90f) * Mathf.Deg2Rad);

            float adjustment1 = mouseY * cosY + mouseX * sinY;
            float adjustment2 =  mouseY * sinX + mouseX * cosX;

            Vector2 adjustment = new Vector2 (adjustment1, adjustment2);

            interactingController.AdjustValue(adjustment);
        }
    }


}
