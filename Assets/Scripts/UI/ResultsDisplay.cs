using System;
using System.Collections;
using GameInput;
using Interfaces;
using UnityEngine;
using Utilities.Animations;

namespace UI
{
    public class ResultsDisplay : MonoBehaviour, IDisplayResults
    {
        [SerializeField, Min(0f)]
        private float animationTime;

        [SerializeField, Header("Optional Animators")]
        private TransformAnimator checkMarkAnimation;
        [SerializeField]
        private WaitForAnimationBase tvScreenAnimation;

        private void Start()
        {
            if (checkMarkAnimation)
                checkMarkAnimation.gameObject.SetActive(false);
        }


        //IDisplayResults Implementation
        //============================================================================================================//
        
        public Coroutine Display(Action uiDisplayReady)
        {
            return StartCoroutine(DisplayResultsCoroutine(uiDisplayReady));
        }

        private IEnumerator DisplayResultsCoroutine(Action uiDisplayReady)
        {
                        
            var continuePressed = false;
            void OnContinuePressed()
            {
                continuePressed = true;
            }
            
            if (checkMarkAnimation)
            {
                checkMarkAnimation.gameObject.SetActive(true);
                checkMarkAnimation.Loop();
            }

            if (tvScreenAnimation)
                yield return tvScreenAnimation.DoAnimation(animationTime, ANIM_DIR.TO_END);
            
            if (checkMarkAnimation)
            {
                checkMarkAnimation.gameObject.SetActive(false);
                checkMarkAnimation.Stop();
            }
            
            uiDisplayReady?.Invoke();


            GameInputDelegator.SetInputLock(true);
            
            

            //Wait for Continue to be pressed
            //------------------------------------------------//
            UIManager.OnContinuePressed += OnContinuePressed;
            yield return new WaitUntil(() => continuePressed);
        
            GameInputDelegator.SetInputLock(false);
        }

        //============================================================================================================//
    }
}