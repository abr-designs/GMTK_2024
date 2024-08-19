using Interactables;
using UnityEngine;

namespace Printer {

    public class SpiralAxisInputControl : InteractableInputControl
    {
        public override float InputValue => inputControlValue;
        public override Vector2 InputValues => twoAxisValue;
        public override Transform InteractionTransform => toggleButton.transform;

        [Header("Object References")]
        [SerializeField] private ButtonInteractable toggleButton;
        [SerializeField] private Transform rotationPartReference;
        [SerializeField] private Transform positionPartReference;
        [SerializeField] private GantryController connectedGantry;
        [SerializeField] private Renderer buttonRenderer;
        [SerializeField] private Color isChangingColor = Color.green;
        [SerializeField] private Color notChangingColor = Color.red;

        [Header("Motion Type")]
        //[SerializeField] private Vector3 rotationAxis = Vector3.up;
        //[SerializeField] private Vector3 positionAxis = Vector3.forward;
        [SerializeField] private Vector3[] transformAxis = new Vector3[] { Vector3.up, Vector3.forward };
        [SerializeField] private float rotationSpeed = 0.4f;
        [SerializeField] private float positionSpeed = 0.8f;
        [SerializeField] private float positionRangeOfMotion = 0.1f;

        [Header("Input Variables")]
        [SerializeField, Range(0f, 1f)] private float inputControlValue = 0.5f;
        [SerializeField] private float inputDampener = 1f;
        private bool DEBUG_updateInEditor = false;

        [Header("READ ONLY")]
        [SerializeField] private bool _isChangingValue;
        [SerializeField] private float timeValue = 0;
        [SerializeField] private Vector2 twoAxisValue = Vector2.zero;
        [SerializeField] private Vector2 dimensionSpeeds = new Vector2(0.75f, 1.5f);
        [SerializeField] private Vector3 targetRotOffset = Vector3.zero;
        [SerializeField] private Vector3 targetPosOffset = Vector3.zero;


        private void Awake() {
            toggleButton.SetValue(1f);
            SetIsChangingValue(true);
        }

        private void Update() {
            
            // check if button is on
            if (toggleButton.InputValue == 1f) _isChangingValue = true;
            else _isChangingValue = false;

            if (_isChangingValue) {
                timeValue += Time.deltaTime;
                ChangeValue();
            }
        }

        private void ChangeValue() {

            twoAxisValue = new Vector2(
                Mathf.Sin(timeValue * dimensionSpeeds.x),
                //(timeValue * dimensionSpeeds.x) % 1f,
                Mathf.Cos(timeValue * dimensionSpeeds.y));

            // normalize
            twoAxisValue = twoAxisValue * 0.5f + Vector2.one * 0.5f;

            inputControlValue = twoAxisValue.x;

            SetMeshTransformFromValue(twoAxisValue);
        }

        private void SetMeshTransformFromValue(Vector2 value) {
            Vector2 rangeValue = value - (Vector2.one * 0.5f);

            //private void SetRotationInRange(float rangeValue) {
            //Vector3 rotationEuler = rotationAxis * (360f * rangeValue.x);
            Vector3 rotationEuler = transformAxis[0] * (360f * rangeValue.x);
            targetRotOffset = rotationEuler;
            Quaternion quaternion = Quaternion.Euler(rotationEuler);
            rotationPartReference.localRotation = quaternion;
            //}

            //private void SetPositionInRange(float rangeValue) {
            //targetPosOffset = positionAxis * (positionRangeOfMotion * 2f * rangeValue.y);
            targetPosOffset = transformAxis[1] * (positionRangeOfMotion * 2f * rangeValue.y);
            positionPartReference.localPosition = targetPosOffset;
            //}
        }

        private void SetIsChangingValue(bool b) {
            // update color
            if(_isChangingValue)
                buttonRenderer.material.color = isChangingColor;
            else
                buttonRenderer.material.color = notChangingColor;
        }

        public override void AdjustValue(float delta) {
            // dampen input
            float dampening = 1f / inputDampener;
            float dampenedDelta = delta * dampening;

            // clamp
            inputControlValue += dampenedDelta;
            inputControlValue = Mathf.Clamp(inputControlValue, 0, 1);

            //ValueChanged(inputControlValue);
        }

        public override void SetValue(float f) {
            //SetIsChangingValue(true);
            toggleButton.SetValue(1f);
        }

        public override void SetIsInteracting(bool b) {
            throw new System.NotImplementedException();
        }

        public override Vector3[] GetTransformAxis() {
            return transformAxis;
        }

        public override void AdjustValue(Vector2 delta) {
            throw new System.NotImplementedException();
        }
    }
}