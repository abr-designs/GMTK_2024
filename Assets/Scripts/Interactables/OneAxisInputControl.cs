using Interactables;
using UnityEngine;

namespace Printer {

    enum ControlTransformType {
        Position,
        Rotation
    }

    public class OneAxisInputControl : InteractableInputControl // should inherit from something like an interactableInputControl
    {
        public override float InputValue => inputControlValue;

        [Header("Object References")]
        [SerializeField] private Transform movingPartReference;
        [SerializeField] private GantryController connectedGantry;

        [Header("Motion Type")]
        [SerializeField] private ControlTransformType controlTransformType = ControlTransformType.Rotation;
        [SerializeField] private Vector3 transformAxis = Vector3.forward;
        [SerializeField] private float rangeOfMotion = 0.4f;

        [Header("Input Variables")]
        [SerializeField, Range(0f, 1f)] private float inputControlValue = 0.5f;
        [SerializeField] private float inputDampener = 125f;
        [SerializeField] private bool DEBUG_updateInEditor = false;

        private bool _isInteracting = false;


        private void SetMeshPositionFromValue(float value) {
            float rangeValue = value - 0.5f;
            switch (controlTransformType) {
                case ControlTransformType.Position:
                    SetPositionInRange(rangeValue);
                    break;
                case ControlTransformType.Rotation:
                    SetRotationInRange(rangeValue);
                    break;
            }
        }

        private void SetPositionInRange(float rangeValue) {
            movingPartReference.localPosition = transformAxis * (rangeOfMotion * rangeValue);
        }

        private void SetRotationInRange(float rangeValue) {
            Vector3 rotationEuler = transformAxis * (rangeOfMotion * rangeValue);
            Quaternion quaternion = Quaternion.Euler(rotationEuler);
            movingPartReference.localRotation = quaternion;
        }


        private void ValueChanged(float newValue) {
            SetMeshPositionFromValue(newValue);

            connectedGantry?.ValueChanged(newValue);
        }

        public override void SetIsInteracting(bool b) {
            _isInteracting = b;
        }

        public override void AdjustValue(float delta) {
            // dampen input
            float dampening = 1f / inputDampener;
            float dampenedDelta = delta * dampening;

            // clamp
            inputControlValue += dampenedDelta;
            inputControlValue = Mathf.Clamp(inputControlValue, 0, 1);

            ValueChanged(inputControlValue);
        }
        
        public override void SetValue(float f) {
            throw new System.NotImplementedException();
        }

#if UNITY_EDITOR
        void OnValidate() {
            // This method is called when any value in the Inspector is changed
            if(DEBUG_updateInEditor) ValueChanged(inputControlValue);
        }
#endif

    }
}