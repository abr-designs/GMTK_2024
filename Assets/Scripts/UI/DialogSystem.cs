using System.Collections;
using Interfaces;
using TMPro;
using UnityEngine;
using Audio.SoundFX;
using Audio;
using Utilities.Animations;

namespace UI
{
    public class DialogSystem : MonoBehaviour, IDisplayDialog
    {
        [SerializeField, Min(0f)]
        private float waitTime;

        [SerializeField]
        private TMP_Text TMPText;

        [SerializeField]
        private TransformAnimator[] transformAnimators;

        [SerializeField]
        private TransformAnimator sourceAnimator;

        [SerializeField]
        private GameObject speechBubble;

        private void Start()
        {
            TMPText.text = string.Empty;
            speechBubble.SetActive(false);
        }

        public IEnumerator DisplayDialogCoroutine(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
                yield break;
            
            speechBubble.SetActive(true);
            PlayAnimations();
            yield return new WaitForSeconds(1f);
            
            TMPText.text = "Hey You!";
            PlayAnimations();
            yield return StartCoroutine(WriteText(TMPText.text, waitTime));
            yield return new WaitForSeconds(1f);
            
            TMPText.text = text;
            PlayAnimations();
            yield return StartCoroutine(WriteText(TMPText.text, waitTime));

            yield return new WaitForSeconds(2f);
            
            TMPText.text = string.Empty;
            PlayAnimations();

            yield return new WaitForSeconds(0.5f);
            
            speechBubble.SetActive(false);
        }

        private IEnumerator WriteText(string text, float waitTime)
        {
            var wait = new WaitForSeconds(waitTime);
            var textLength = text.Length;
            
            sourceAnimator?.Loop();
            for (int i = 0; i <= textLength; i++)
            {
                TMPText.maxVisibleCharacters = i;
                SFX.TEXT.PlaySound();
                yield return wait;
            }
            sourceAnimator?.Stop();
        }

        private void PlayAnimations()
        {
            for (int i = 0; i < transformAnimators.Length; i++)
            {
                transformAnimators[i].Play();
            }
        }
    }
}
