using UnityEngine;

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

        // private
        private float lastKnownValue = 0f;

        private void Update() {
            // check if button value changed
            if(buttonInteractable.InputValue != lastKnownValue) {

                float newValue = buttonInteractable.InputValue;

                if (shouldSwitchColor) {
                    if(newValue == 1f) buttonRenderer.material.color = onColor;
                    else buttonRenderer.material.color = offColor;
                } else {
                    if (hasDepressedColor) {
                        if (newValue == 1f) buttonRenderer.material.color = onColor;
                        else buttonRenderer.material.color = waitingColor;
                    }
                }

                lastKnownValue = newValue;
            }

        }
    }
}