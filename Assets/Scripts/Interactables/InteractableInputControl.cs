using UnityEngine;

namespace Interactables
{
    public abstract class InteractableInputControl : MonoBehaviour
    {
        public abstract float InputValue { get; }
        public abstract void SetIsInteracting(bool b);
    }
}
