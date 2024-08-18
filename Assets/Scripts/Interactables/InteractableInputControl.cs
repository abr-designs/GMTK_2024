using UnityEngine;

namespace Interactables
{
    public abstract class InteractableInputControl : MonoBehaviour
    {
        public abstract float InputValue { get; }
        public abstract Transform InteractionTransform { get; }
        public abstract void SetIsInteracting(bool b);
        public abstract void AdjustValue(float delta);
        public virtual void SetValue(float f) { }
    }
}
