using UnityEngine;

namespace Printer
{
    public class
        LeverController : InteractableInputControl // should inherit from something like an interactableInputControl
    {
        public override float InputValue => inputControlValue;

        [SerializeField] public Transform movableTransform;

        [SerializeField] private float maxPositionRange = 0.4f;
        [SerializeField] private Vector3 offsetAxis = Vector3.forward;

        [SerializeField, Range(0f, 1f)] private float inputControlValue = 0.5f;

        [SerializeField] private float inputDampener = 5f;

        private bool _isInteracting = false;

        [SerializeField] private GantryController connectedGantry;

        private void SetMeshPositionFromValue(float value)
        {
            float rangeValue = value - 0.5f;
            movableTransform.localPosition = offsetAxis * (maxPositionRange * rangeValue);
        }


        private void ValueChanged(float newValue)
        {
            SetMeshPositionFromValue(newValue);

            connectedGantry?.ValueChanged(newValue);
        }

        public void SetIsInteracting(bool b)
        {
            _isInteracting = b;
        }

        public void AdjustValue(float delta)
        {
            // dampen input
            float dampening = 1f / inputDampener;
            delta *= dampening;

            // clamp
            inputControlValue += delta * maxPositionRange;
            inputControlValue = Mathf.Clamp(inputControlValue, 0, 1);

            ValueChanged(inputControlValue);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            // This method is called when any value in the Inspector is changed
            ValueChanged(inputControlValue);
        }
#endif

    }
}
