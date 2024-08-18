using System.Collections;
using UnityEngine;

namespace Utilities.Animations
{
    public abstract class WaitForAnimationBase : MonoBehaviour, IWaitForAnimation
    {
        public abstract IEnumerator DoAnimationCoroutine(float time, bool invert);
        
        protected IEnumerator ScaleCoroutine(Transform target, Vector3 startScale, Vector3 targetScale, float time, AnimationCurve animationCurve)
        {
            for (var t = 0f; t <= time; t += Time.deltaTime)
            {
                var dt = t / time;

                target.transform.localScale = Vector3.Lerp(startScale, targetScale, animationCurve.Evaluate(dt));
                    
                yield return null;
            }
        }

        protected IEnumerator MoveToPositionCoroutine(Transform target, Vector3 startPosition, Vector3 endPosition, float time, AnimationCurve animationCurve)
        {
            for (var t = 0f; t <= time; t += Time.deltaTime)
            {
                var dt = t / time;
            

                target.transform.position = Vector3.Lerp(startPosition, endPosition, animationCurve.Evaluate(dt));
                    
                yield return null;
            }
        }
    }
}