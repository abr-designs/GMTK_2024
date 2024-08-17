using UnityEngine;

namespace Interactables
{
    public abstract class InteractableInputControl : MonoBehaviour
    {
        public abstract float InputValue { get; }
        public abstract void SetIsInteracting(bool b);
        public abstract void AdjustValue(float delta);
        public abstract void SetValue(float f);
    }
}
