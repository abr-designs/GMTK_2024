using System.Collections;

namespace Utilities.Animations
{
    public interface IWaitForAnimation
    {
        IEnumerator DoAnimatioCoroutine(float time, bool invert);
    }
}