using System;
using Interactables.Enums;
using Levels.Enums;
using UnityEngine;

namespace Interactables
{
    public class ControlPanelContainer : MonoBehaviour
    {
        [Serializable]
        private class ControlInputData
        {
            public CONTROLS control;
            public InteractableInputControl inputControl;
        }

        //------------------------------------------------//
    
        public CONTROL_PANEL_TYPE controlPanelType;

        [SerializeField]
        private ControlInputData[] controls;

        public (CONTROLS control, float value)[] GetControlValues()
        {
            var outData = new (CONTROLS, float)[controls.Length];

            for (int i = 0; i < controls.Length; i++)
            {
                outData[i] = (controls[i].control, controls[i].inputControl.InputValue);
            }

            return outData;
        }

    }
}



