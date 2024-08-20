using System;
using Audio;
using System.Diagnostics;
using UnityEngine;
using Audio.SoundFX;


namespace Interactables
{
    public class ButtonInteractable : InteractableInputControl
    {
        public event Action OnButtonPressed;

        public override float InputValue => _inputValue;
        public override Vector2 InputValues => throw new NotImplementedException();
        public override Transform InteractionTransform => transform;


        [SerializeField] private bool isToggleButton = false;

        [SerializeField] SFX interactionSFX;
        [SerializeField] private float sfxCooldown = 0.5f;
        private float sfxCountdown;

        private float _inputValue;

        private void TriggerInteractionSFX() {
            if (sfxCountdown > 0) return;

            interactionSFX.PlaySound();
            sfxCountdown = sfxCooldown;
        }
        private void Update() {
            if (sfxCountdown > 0f) { sfxCountdown -= Time.deltaTime; }
        }

        public override void SetIsInteracting(bool b)
        {
            
            
            switch (isToggleButton)
            {
                case false:
                    _inputValue = b ? 1f : 0f;
                    if (b)
                        OnButtonPressed?.Invoke();
                    break;
                case true:
                    if (!b) _inputValue = UnityEngine.Mathf.Abs(_inputValue - 1f);

                    break;
            }

            // on mouse release, trigger sfx
            if (!b) TriggerInteractionSFX();


        }

        public override void AdjustValue(float delta)
        {

        }

        public override void SetValue(float f)
        {
            _inputValue = f;
        }

        [Conditional("UNITY_EDITOR"), ContextMenu("Press Button")]
        private void SetToMax()
        {
            SetValue(1f);
        }

        public override Vector3[] GetTransformAxis() {
            return new Vector3[1];
            //throw new NotImplementedException();
        }

        public override void AdjustValue(Vector2 delta) {
            throw new NotImplementedException();
        }
    }
}