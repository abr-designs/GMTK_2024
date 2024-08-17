using UnityEngine;

public abstract class InteractableInputControl : MonoBehaviour
{
    public abstract float InputValue { get; }

    public abstract void SetIsInteracting(bool isInteracting);
    public abstract void AdjustValue(float delta);

}
