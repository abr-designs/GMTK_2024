using UnityEngine;

namespace Interactables
{
    public abstract class InteractableInputControl : MonoBehaviour
    {
        public abstract float InputValue { get; }
        public abstract Transform InteractionTransform { get; }
        public abstract void SetIsInteracting(bool b);
        public abstract void AdjustValue(float delta);
        public abstract void AdjustValue(Vector2 delta);
        public virtual void SetValue(float f) { }
        public abstract Vector3[] GetTransformAxis();
    }
}
