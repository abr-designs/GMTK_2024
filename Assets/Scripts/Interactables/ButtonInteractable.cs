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
    }
}