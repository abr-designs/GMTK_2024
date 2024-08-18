using System.Collections;
using Interfaces;
using UnityEngine;

namespace UI
{
    public class DialogSystem : MonoBehaviour, IDisplayDialog
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        public IEnumerator DisplayDialogCoroutine(string text)
        {
            yield break;
        }
    }
}
