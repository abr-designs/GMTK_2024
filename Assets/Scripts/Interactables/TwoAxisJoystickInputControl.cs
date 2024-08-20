using Audio;
using Audio.SoundFX;
using Interactables;
using UnityEngine;
using Utilities.Animations;

namespace Printer {

    public class TwoAxisJoystickInputControl : InteractableInputControl
    {
        public override float InputValue => inputControlValue;
        public override Vector2 InputValues => twoAxisValue;
        public override Transform InteractionTransform => interactionTransform;

        [SerializeField]
        private Transform interactionTransform;

        [Header("Object References")]
        [SerializeField] private Transform movingPartReference;
        [SerializeField] private GantryController connectedGantry;

        [Header("Motion Type")]
        [SerializeField] private ControlTransformType controlTransformType = ControlTransformType.Rotation;
        //[SerializeField] private Vector3 transformAxis1 = Vector3.forward;
        //[SerializeField] private Vector3 transformAxis2 = Vector3.right;
        [SerializeField] private Vector3[] transformAxis = new Vector3[] { Vector3.forward , Vector3.left };

        [SerializeField] private float rangeOfMotion = 45f;

        [Header("Input Variables")]
        private float inputControlValue = 0.5f;
        [SerializeField] private Vector2 twoAxisValue = Vector2.one * 0.5f;
        [SerializeField] private float inputDampener = 125f;
        private bool DEBUG_updateInEditor = false;

        [SerializeField]
        private TransformAnimator transformAnimator;

        private bool _isInteracting = false;
        [SerializeField] SFX interactionSFX;
        [SerializeField] private float sfxCooldown = 0.5f;
        private float sfxCountdown;


        private void TriggerInteractionSFX() {
            if (sfxCountdown > 0) return;

            interactionSFX.PlaySound();
            sfxCountdown = sfxCooldown;
        }
        private void Update() {
            if (sfxCountdown > 0f) { sfxCountdown -= Time.deltaTime; }
        }

        private void SetMeshTransformFromValue(Vector2 value)
        {
            Vector2 rangeValue = value - (Vector2.one * 0.5f);
            switch (controlTransformType) {
                case ControlTransformType.Position:
                    SetPositionInRange(rangeValue);
                    break;
                case ControlTransformType.Rotation:
                    SetRotationInRange(rangeValue);
                    break;
            }
        }

        private void SetPositionInRange(Vector2 rangeValue) {
            movingPartReference.localPosition =
                transformAxis[0] * (rangeOfMotion * rangeValue.x) +
                transformAxis[1] * (rangeOfMotion * rangeValue.y);
        }

        private void SetRotationInRange(Vector2 rangeValue) {
            Vector3 rotationEuler =
                transformAxis[0] * (rangeOfMotion * rangeValue.x) +
                transformAxis[1] * (rangeOfMotion * rangeValue.y);
            Quaternion quaternion = Quaternion.Euler(rotationEuler);
            movingPartReference.localRotation = quaternion;
        }


        private void ValueChanged(Vector2 newValue) {
            SetMeshTransformFromValue(newValue);

            TriggerInteractionSFX();

            // tell the gantry to move
            //connectedGantry?.ValueChanged(newValue);
            // instead, broadcast a message that a control value has changed
            //
        }

        public override void AdjustValue(Vector2 delta) {
            // dampen input
            float dampening = 1f / inputDampener;
            Vector2 dampenedDelta = delta * dampening;

            // clamp
            twoAxisValue += dampenedDelta;
            twoAxisValue.x = Mathf.Clamp(twoAxisValue.x, 0, 1);
            twoAxisValue.y = Mathf.Clamp(twoAxisValue.y, 0, 1);

            ValueChanged(twoAxisValue);
        }

        public override void SetValue(float f) {
            ValueChanged(new Vector2(f, f));
        }

        public override void SetIsInteracting(bool b) {
            _isInteracting = b;

            if (_isInteracting == false)
            {
                transformAnimator?.Play();
                SFX.INTERACT.PlaySound();
            }
        }

        public override Vector3[] GetTransformAxis() {
            return transformAxis;
        }

        public override void AdjustValue(float delta) {
            throw new System.NotImplementedException();
        }
    }
}