using System;
using System.Diagnostics;
using UnityEngine;

namespace Interactables
{
    public class ButtonInteractable : InteractableInputControl
    {
        public override float InputValue => inputValue;

        protected float inputValue;

        public override void SetIsInteracting(bool b)
        {
            inputValue = b ? 1f : 0f;

        }

        public override void AdjustValue(float delta) {
            throw new System.NotImplementedException();
        }

        public override void SetValue(float f)
        {
            inputValue = f;
        }

        [Conditional("UNITY_EDITOR"), ContextMenu("Press Button")]
        private void SetToMax()
        {
            SetValue(1f);
        }
    }
}