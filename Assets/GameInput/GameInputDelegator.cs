using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class GameInputDelegator : MonoBehaviour, InputActions.IGameplayActions
    {
        public static event Action<bool> InputLockChanged;
        public static event Action<Vector2> OnMouseMove;
        public static event Action<bool> OnLeftClick;
        public static event Action<bool> OnRightClick;

        public static bool LockInputs { get; private set; }

        private Vector2 _currentInput;
    
        //============================================================================================================//\

        private void OnEnable()
        {
            Inputs.Input.Gameplay.Enable();
            Inputs.Input.Gameplay.AddCallbacks(this);
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }
    
        private void OnDisable()
        {
            Inputs.Input.Gameplay.Disable();
            Inputs.Input.Gameplay.RemoveCallbacks(null);
        }
        //============================================================================================================//

        public static void SetInputLock(bool lockState)
        {
            LockInputs = lockState;
            
            InputLockChanged?.Invoke(lockState);
        }

        //============================================================================================================//

        public void OnMouseLeftClick(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;
            
            if (LockInputs)
            {
                OnLeftClick?.Invoke(false);
                return;
            }
            
            var pressed = context.ReadValueAsButton();
            OnLeftClick?.Invoke(pressed);
        }

        public void OnMouseRightClick(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;
            
            if (LockInputs)
            {
                OnRightClick?.Invoke(false);
                return;
            }
            
            var pressed = context.ReadValueAsButton();
            OnRightClick?.Invoke(pressed);
        }

        public void OnMouseLook(InputAction.CallbackContext context)
        {
            if (LockInputs)
            {
                OnMouseMove?.Invoke(Vector2.zero);
                return;
            }
            
            var delta = context.ReadValue<Vector2>();
            OnMouseMove?.Invoke(delta);
        }


        //============================================================================================================//
    }
}
