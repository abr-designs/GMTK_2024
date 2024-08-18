using System.Collections;

namespace Interfaces
{
    public interface IDisplayDialog
    {
        public IEnumerator DisplayDialogCoroutine(string text);
    }
}