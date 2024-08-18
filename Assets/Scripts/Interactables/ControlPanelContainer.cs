using System;
using Interactables.Enums;
using Levels.Enums;
using Printer;
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

        private void OnEnable() {
            GameManager.OnWorldShake += ShuffleControls;
        }
        private void OnDisable() {
            GameManager.OnWorldShake -= ShuffleControls;
        }

        public void SetActive(bool state) => gameObject.SetActive(state);

        private void ShuffleControls(float _) {
            //TODO Set all controls to be random values
            foreach(ControlInputData controlInputData in controls) {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                controlInputData.inputControl.SetValue(randomValue);
            }
        }
        

        public (CONTROLS control, float value, float value2)[] GetControlValues()
        {
            var outData = new (CONTROLS, float, float value2)[controls.Length];

            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].inputControl is SpiralAxisInputControl spiralAxisInputControl)
                {
                    var twoAxisValue = spiralAxisInputControl.InputValues;
                    outData[i] = (controls[i].control, twoAxisValue.x, twoAxisValue.y);
                    continue;
                }
                    
                
                outData[i] = (controls[i].control, controls[i].inputControl.InputValue, default);
            }

            return outData;
        }

    }
}



