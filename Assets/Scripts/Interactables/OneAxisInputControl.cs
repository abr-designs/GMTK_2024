using Audio;
using Interactables;
using UnityEditor;
using UnityEngine;
using Utilities.Animations;
using Audio.SoundFX;
using System;
using System.Net.Http.Headers;
using static GantryController;
using World;

namespace Printer
{

    enum ControlTransformType
    {
        Position,
        Rotation
    }

    public class OneAxisInputControl : InteractableInputControl // should inherit from something like an interactableInputControl
    {
        public override float InputValue => inputControlValue;
        public override Vector2 InputValues => throw new NotImplementedException();


        public override Transform InteractionTransform => interactionTransform;

        [SerializeField] 
        private Transform interactionTransform;

        [Header("Object References")]
        [SerializeField] private Transform movingPartReference;
        [SerializeField] private GantryController.GantryControlAxis gantryControlAxis;
        [SerializeField] private GantryController connectedGantry;

        [Header("Motion Type")]
        [SerializeField] private ControlTransformType controlTransformType = ControlTransformType.Rotation;
        [SerializeField] private Vector3[] transformAxis = new Vector3[] { Vector3.forward };
        [SerializeField] private float rangeOfMotion = 2.5f;

        [Header("Input Variables")]
        //[SerializeField] private Vector3 positiveInputAxis = (enum)Vector3;
        [SerializeField, Range(0f, 1f)] private float inputControlValue = 0.5f;
        [SerializeField] private float inputDampener = 125f;
        [SerializeField] private bool DEBUG_updateInEditor = false;

        [SerializeField]
        private TransformAnimator transformAnimator;

        private bool _isInteracting = false;
        [SerializeField] SFX interactionSFX;
        [SerializeField] private float sfxCooldown = 0.5f;
        private float sfxCountdown;

        private void OnEnable()
        {
            // connect to Gantry through PrinterReferenceController
            if (gantryControlAxis == GantryControlAxis.NONE) return;

            GantryController[] gantryAxisObjectList = FindObjectsOfType<GantryController>();
            foreach(GantryController controller in gantryAxisObjectList) {
                if(controller.GetGantryControlAxis() == gantryControlAxis) {
                    connectedGantry = controller;
                    break;
                }
            }

        }

        private void TriggerInteractionSFX() {
            if (sfxCountdown > 0) return;

            interactionSFX.PlaySound();
            sfxCountdown = sfxCooldown;
        }
        private void Update() {
            if(sfxCountdown > 0f) { sfxCountdown -= Time.deltaTime; }
        }

        private void SetMeshPositionFromValue(float value)
        {
            float rangeValue = value - 0.5f;
            switch (controlTransformType)
            {
                case ControlTransformType.Position:
                    SetPositionInRange(rangeValue);
                    break;
                case ControlTransformType.Rotation:
                    SetRotationInRange(rangeValue);
                    break;
            }
        }

        private void SetPositionInRange(float rangeValue)
        {
            movingPartReference.localPosition = transformAxis[0] * (rangeOfMotion * rangeValue);
        }

        private void SetRotationInRange(float rangeValue)
        {
            Vector3 rotationEuler = transformAxis[0] * (rangeOfMotion * rangeValue);
            Quaternion quaternion = Quaternion.Euler(rotationEuler);
            movingPartReference.localRotation = quaternion;
        }


        private void ValueChanged(float newValue)
        {
            SetMeshPositionFromValue(newValue);

            TriggerInteractionSFX();

            // tell the gantry to move
            connectedGantry?.ValueChanged(newValue);
            // instead, broadcast a message that a control value has changed
            //
        }

        public override void SetIsInteracting(bool b)
        {
            _isInteracting = b;

            if (_isInteracting == false)
                transformAnimator?.Play();
        }

        public override void AdjustValue(float delta)
        {
            // dampen input
            float dampening = 1f / inputDampener;
            float dampenedDelta = delta * dampening;

            // clamp
            inputControlValue += dampenedDelta;
            inputControlValue = Mathf.Clamp(inputControlValue, 0, 1);
            ValueChanged(inputControlValue);
        }

        public override Vector3[] GetTransformAxis() {
            return transformAxis;
        }

        public override void SetValue(float f)
        {
            //throw new System.NotImplementedException();
            //Debug.Log($"Value changed for {name} to {f}");
            inputControlValue = f;
            ValueChanged(inputControlValue);
        }

        public override void AdjustValue(Vector2 delta) {
            throw new System.NotImplementedException();
        }

    }
}