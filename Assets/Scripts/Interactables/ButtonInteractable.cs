using System;

namespace Interactables
{
    public class ButtonInteractable : InteractableInputControl
    {
        public override float InputValue => _inputValue;

        private float _inputValue;

        public override void SetIsInteracting(bool b)
        {
            _inputValue = b ? 1f : 0f;

        }

        public override void AdjustValue(float delta) {
            throw new System.NotImplementedException();
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
    }
}