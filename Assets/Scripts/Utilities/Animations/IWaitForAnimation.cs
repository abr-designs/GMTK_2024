using System.Collections;
using UnityEngine;

namespace Utilities.Animations
{
    public enum ANIM_DIR
    {
        TO_END,
        TO_START
    }
    public interface IWaitForAnimation
    {
        Coroutine DoAnimation(float time, ANIM_DIR animDir);
    }
}