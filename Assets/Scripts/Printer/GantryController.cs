using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Audio;
using Audio.SoundFX;
using TMPro;
using UnityEngine;

public class GantryController : MonoBehaviour
{
    public enum GantryControlAxis {
        NONE = 0,
        X = 1,
        Z = 2,
    }

    [SerializeField] private GantryControlAxis gantryControlAxis = GantryControlAxis.X;
    public GantryControlAxis GetGantryControlAxis() { return gantryControlAxis; }
    public Transform movableTransform;

    public float maxPositionRange = 4.5f;
    public Vector3 offsetAxis = Vector3.right;

    [Range(0f, 1f)]
    public float inputValue = 0.5f;

    // lerp movement
    public Vector3 targetPosition; // The target position to move towards
    public float lerpDuration = 2f;  // The time it takes to complete the lerp
    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private bool isLerping = false;

    void Update()
    {
        if (isLerping)
        {
            // Calculate the current progress
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpDuration;

            // Ease in and ease out (using a sine wave for smooth start and end)
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Eases out
            //t = t * t * (3f - 2f * t); // Eases in and out (alternative method)

            // Lerp the position
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

            // Check if the lerp is complete
            if (elapsedTime >= lerpDuration)
            {
                isLerping = false;
            }
        }
    }

    void StartLerp(Vector3 targetPostion)
    {
        startPosition = transform.localPosition;
        //this.targetPosition = targetPostion;
        elapsedTime = 0f;
        isLerping = true;

        SFX.Gantry.PlaySound();
    }

    public void SetMeshPositionFromValue(float value)
    {

        float rangeValue = value - 0.5f;
        movableTransform.localPosition = (maxPositionRange * offsetAxis) * rangeValue;
    }

    public Vector3 GetTargetPositionFromValue(float value)
    {
        float rangeValue = value - 0.5f;
        Vector3 targetPosition = (maxPositionRange * offsetAxis) * rangeValue;
        return targetPosition;
    }

    //void OnValidate() {
    //    // This method is called when any value in the Inspector is changed
    //    ValueChanged(inputValue);
    //}

    public void ValueChanged(float newValue)
    {
        //SetMeshPositionFromValue(newValue); // would like to lerp over time
        targetPosition = GetTargetPositionFromValue(newValue);
        StartLerp(targetPosition);
    }
}
