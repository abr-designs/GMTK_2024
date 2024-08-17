using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour // should inherit from something like an interactableInputControl
{
    public Transform movableTransform;

    public float maxPositionRange = 0.4f;
    public Vector3 offsetAxis = Vector3.forward;

    [Range(0f, 1f)]
    public float inputControlValue = 0.5f;

    public float inputDampener = 5f;

    bool isInteracting = false;

    [SerializeField] private GantryController connectedGantry;

    public void SetMeshPositionFromValue(float value) {

        float rangeValue = value - 0.5f;
        movableTransform.localPosition = (maxPositionRange * offsetAxis) * rangeValue;
    }

    void OnValidate() {
        // This method is called when any value in the Inspector is changed
        ValueChanged(inputControlValue);
    }

    public void ValueChanged(float newValue) {
        SetMeshPositionFromValue(newValue);

        if(connectedGantry != null) {
            connectedGantry.ValueChanged(newValue);
        }
    }

    public void SetIsInteracting(bool b) {
        isInteracting = b;
    }

    public void AdjustValue(float delta) {

        // dampen input
        float dampening = 1f / inputDampener;
        delta *= dampening;

        // clamp
        inputControlValue += delta * maxPositionRange;
        inputControlValue = Mathf.Clamp(inputControlValue, 0, 1);

        ValueChanged(inputControlValue);
    }
}
