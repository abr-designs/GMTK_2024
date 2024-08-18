using System;

namespace Interactables
{
    public class ButtonToggleInteractable : ButtonInteractable
    {
        public override float InputValue => inputValue;

        public override void SetIsInteracting(bool b)
        {
            if (!b)
                inputValue = UnityEngine.Mathf.Abs(inputValue - 1f);

        }

        public override void SetValue(float f) {
            inputValue = f;
        }
    }
}