using System.Collections;

namespace Utilities.Animations
{
    public enum ANIM_DIR
    {
        TO_END,
        TO_START
    }
    public interface IWaitForAnimation
    {
        IEnumerator DoAnimationCoroutine(float time, ANIM_DIR animDir);
    }
}