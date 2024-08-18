using System;
using System.Collections;
using Interfaces;
using TMPro;
using UnityEngine;
using Audio.SoundFX;
using Audio;
namespace UI
{
    public class DialogSystem : MonoBehaviour, IDisplayDialog
    {

        public TMP_Text container;
        // Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(DisplayDialogCoroutine("this is a test"));
        }

        int tick = 0;
        public IEnumerator DisplayDialogCoroutine(string text)
        {

            container.text = text;
            int text_length = text.Length;
            for (int i = 0; i < text_length + 2; i++)
            {
                container.maxVisibleCharacters = i;
                SFX.Text.PlaySound();
                yield return new WaitForSeconds(0.05f);
            }
            //tick++;
            yield break;

        }


    }
}
