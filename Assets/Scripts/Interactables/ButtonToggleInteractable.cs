using System;

namespace Interactables
{
    public class ButtonToggleInteractable : ButtonInteractable
    {
        public override float InputValue => _inputValue;

        private float _inputValue;

        public override void SetIsInteracting(bool b)
        {
            if (!b)
                _inputValue = UnityEngine.Mathf.Abs(_inputValue - 1f);

        }

        public override void SetValue(float f) {
            _inputValue = f;
        }
    }
}