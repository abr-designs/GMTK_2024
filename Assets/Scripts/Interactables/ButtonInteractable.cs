﻿using System;
using System.Diagnostics;
using UnityEngine;

namespace Interactables
{
    public class ButtonInteractable : InteractableInputControl
    {
        public override float InputValue => inputValue;

        [SerializeField] private bool isToggleButton = false;

        private float _inputValue;

        public override void SetIsInteracting(bool b)
        {
            switch(isToggleButton) {
                case false:
                    _inputValue = b ? 1f : 0f;
                    break;
                case true:
                    if (!b) _inputValue = UnityEngine.Mathf.Abs(_inputValue - 1f);
                    break;
            }
            

        }

        public override void AdjustValue(float delta) {
            throw new System.NotImplementedException();
        }

        public override void SetValue(float f)
        {
            inputValue = f;
        }

        [Conditional("UNITY_EDITOR"), ContextMenu("Press Button")]
        private void SetToMax()
        {
            SetValue(1f);
        }
    }
}