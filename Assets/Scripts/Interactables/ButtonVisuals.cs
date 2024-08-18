using UnityEngine;
using Utilities.Animations;

namespace Interactables
{
    public class ButtonVisuals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ButtonInteractable buttonInteractable;
        [SerializeField] private Renderer buttonRenderer;
        
        [Header("Variables")]
        [SerializeField] private bool shouldSwitchColor = false;
        [SerializeField] private bool hasDepressedColor = true;

        [SerializeField] private Color onColor = Color.green;
        [SerializeField] private Color offColor = Color.red;
        [SerializeField] private Color waitingColor = Color.yellow;

        [SerializeField]
        private TransformAnimator transformAnimator;

        // private
        private float lastKnownValue = 0f;


        private void Start() {
            if (shouldSwitchColor) buttonRenderer.material.color = offColor;
            if (!shouldSwitchColor) buttonRenderer.material.color = waitingColor;
        }

        private void Update()
        {
            // check if button value changed
            if (buttonInteractable.InputValue == lastKnownValue)
                return;

            float newValue = buttonInteractable.InputValue;

            if (shouldSwitchColor)
            {
                transformAnimator.Play();
                buttonRenderer.material.color = newValue == 1f ? onColor : offColor;
            }
            else
            {
                if (hasDepressedColor)
                {
                    if (newValue == 1f)
                    {
                        buttonRenderer.material.color = onColor;
                        transformAnimator.Play();
                    }
                    else buttonRenderer.material.color = waitingColor;
                }
            }

            lastKnownValue = newValue;


        }
    }
}