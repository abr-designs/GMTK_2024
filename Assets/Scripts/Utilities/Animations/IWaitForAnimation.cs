using System.Collections;

namespace Utilities.Animations
{
    public interface IWaitForAnimation
    {
        IEnumerator DoAnimationCoroutine(float time, bool invert);
    }
}