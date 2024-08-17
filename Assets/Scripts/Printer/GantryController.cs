using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GantryController : MonoBehaviour
{
    public Transform movableTransform;

    public float maxPositionRange = 4.5f;
    public Vector3 offsetAxis = Vector3.right;

    [Range(0f, 1f)]
    public float inputValue = 0.5f;

    public void SetMeshPositionFromValue(float value) {

        float rangeValue = value - 0.5f;
        movableTransform.localPosition = (maxPositionRange * offsetAxis) * rangeValue;
    }

    void OnValidate() {
        // This method is called when any value in the Inspector is changed
        ValueChanged(inputValue);
    }

    public void ValueChanged(float newValue) {
        SetMeshPositionFromValue(newValue); // would like to lerp over time
    }
}
