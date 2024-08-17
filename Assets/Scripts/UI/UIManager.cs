using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static event Action OnContinuePressed;

        [SerializeField]
        private TMP_Text displayText;
        [SerializeField]
        private TextMeshPro timerWorldText;

        [SerializeField]
        private Button continueButton;

        //============================================================================================================//
        private void OnEnable()
        {
            GameManager.OnCountdown += OnCountdown;
            GameManager.DisplayText += OnDisplayText;
            GameManager.OnLayerStarted += OnLayerStarted;
            GameManager.OnLevelComplete += OnLevelComplete;

        }

        private void Start()
        {
            Assert.IsNotNull(displayText);
            Assert.IsNotNull(timerWorldText);
            Assert.IsNotNull(continueButton);
            
            continueButton.onClick.AddListener(() =>
            {
                OnContinuePressed?.Invoke();
            });

            continueButton.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GameManager.OnCountdown -= OnCountdown;
            GameManager.DisplayText -= OnDisplayText;
            GameManager.OnLayerStarted -= OnLayerStarted;   
        }
        
        //============================================================================================================//
        
        private void OnLayerStarted()
        {
            displayText.text = string.Empty;
        }
        
        private void OnCountdown(float time)
        {
            timerWorldText.text = $"{time:00.00}";
        }
        
        private void OnDisplayText(string textToDisplay)
        {
            displayText.text = textToDisplay;
        }
        private void OnLevelComplete()
        {
            continueButton.gameObject.SetActive(true);
        }
    }
}