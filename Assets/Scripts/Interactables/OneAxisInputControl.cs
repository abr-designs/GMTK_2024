using Interactables;
using UnityEngine;
using Utilities.Animations;

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

        [SerializeField]
        private TransformAnimator transformAnimator;

        private bool _isInteracting = false;

        private void OnEnable() {
            // connect to Gantry through PrinterReferenceController
            //

        }

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

            // tell the gantry to move
            connectedGantry?.ValueChanged(newValue);
            // instead, broadcast a message that a control value has changed
            //
        }

        public override void SetIsInteracting(bool b) {
            _isInteracting = b;
            
            if(_isInteracting == false)
                transformAnimator?.Play();
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
            //throw new System.NotImplementedException();
            //Debug.Log($"Value changed for {name} to {f}");
            inputControlValue = f;
            ValueChanged(inputControlValue);
        }

#if UNITY_EDITOR
        void OnValidate() {
            // This method is called when any value in the Inspector is changed
            if(DEBUG_updateInEditor) ValueChanged(inputControlValue);
        }
#endif

    }
}