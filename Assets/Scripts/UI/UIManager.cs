using System;
using System.Collections;
using Audio;
using Audio.SoundFX;
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
        private TMP_Text resultsText;

        [SerializeField]
        private GameObject resultStars;
        [SerializeField]
        private AnimationCurve scaleCurve;

        [SerializeField]
        private float starsAnimationTime;

        [SerializeField]
        private Button continueButton;

        //============================================================================================================//
        private void OnEnable()
        {
            GameManager.OnCountdown += OnCountdown;
            GameManager.DisplayText += OnDisplayText;
            GameManager.DisplayResultText += DisplayResultText;
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
            GameManager.DisplayResultText -= DisplayResultText;
            GameManager.OnLayerStarted -= OnLayerStarted;
            GameManager.OnLevelComplete -= OnLevelComplete;
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

        private void DisplayResultText(string textToDisplay)
        {
            resultsText.text = textToDisplay;
        }

        private void OnLevelComplete(int score)
        {
            resultsWindowObject.gameObject.SetActive(true);
            StartCoroutine(AnimateScore(score));
        }
        //============================================================================================================//

        private IEnumerator AnimateText(string textToDisplay)
        {
            yield return windowMoveAnimation.DoAnimation(animationTime, ANIM_DIR.TO_END);
            displayText.text = textToDisplay;

            if (string.IsNullOrWhiteSpace(textToDisplay))
                yield break;

            yield return windowMoveAnimation.DoAnimation(animationTime, ANIM_DIR.TO_START);
        }

        private IEnumerator AnimateScore(int value)
        {
            foreach (Transform t in resultStars.transform)
            {
                t.GetComponent<Image>().enabled = false;
            }

            for (int i = 0; i < value; i++)
            {
                var child = resultStars.transform.GetChild(i);

                child.GetComponent<Image>().enabled = true;
                SFX.STAR.PlaySound();
                yield return StartCoroutine(AnimateStar(child.transform, starsAnimationTime));

            }

        }

        private IEnumerator AnimateStar(Transform target, float time)
        {
            var startScale = Vector3.one * scaleCurve.Evaluate(0);
            var targetScale = Vector3.one * scaleCurve.Evaluate(1);
            for (var t = 0f; t <= time; t += Time.deltaTime)
            {
                var dt = t / time;
                target.transform.localScale = Vector3.one * scaleCurve.Evaluate(dt);

                yield return null;
            }
            target.transform.localScale = targetScale;
        }

    }
}