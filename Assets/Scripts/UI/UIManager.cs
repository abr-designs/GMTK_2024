using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utilities.Animations;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static event Action OnContinuePressed;

        [SerializeField]
        private TMP_Text displayText;
        [SerializeField]
        private WaitForAnimationBase windowMoveAnimation;
        [SerializeField, Min(0f)]
        private float animationTime;
        
        [SerializeField]
        private TMP_Text timerWorldText;

        [SerializeField]
        private GameObject resultsWindowObject;

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
                resultsWindowObject.SetActive(false);
            });

            resultsWindowObject.gameObject.SetActive(false);
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
            OnDisplayText(string.Empty);
        }
        
        private void OnCountdown(float time)
        {
            timerWorldText.text = $"{time:00.00}";
        }
        
        private void OnDisplayText(string textToDisplay)
        {
            StartCoroutine(AnimateText(textToDisplay));
        }
        private void OnLevelComplete()
        {
            resultsWindowObject.gameObject.SetActive(true);
        }
        //============================================================================================================//

        private IEnumerator AnimateText(string textToDisplay)
        {
            yield return windowMoveAnimation.DoAnimation(animationTime, ANIM_DIR.TO_END);
            displayText.text = textToDisplay;
            
            if(string.IsNullOrWhiteSpace(textToDisplay))
                yield break;
            
            yield return windowMoveAnimation.DoAnimation(animationTime, ANIM_DIR.TO_START);
        }
    }
}