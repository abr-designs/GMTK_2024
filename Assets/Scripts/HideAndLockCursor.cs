using System;
using GameInput;
using UnityEngine;

public class HideAndLockCursor : MonoBehaviour
{
    public bool hideCursor = true;

    // Start is called before the first frame update
    private void OnEnable()
    {
        GameInputDelegator.InputLockChanged += OnInputLockChanged;
    }


    private void Start()
    {
        if(hideCursor)  
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        GameInputDelegator.InputLockChanged -= OnInputLockChanged;
    }
    
    //Callbacks
    //============================================================================================================//
    
    private void OnInputLockChanged(bool inputLockState)
    {
        if (hideCursor == false)
            return;
        
        Cursor.lockState = inputLockState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inputLockState;
    }
}
