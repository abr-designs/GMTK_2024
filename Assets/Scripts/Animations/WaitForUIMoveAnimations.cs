using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Utilities.Animations;
using Utilities.Debugging;

namespace Animations
{
    public class WaitForUIMoveAnimations : WaitForAnimationBase
    {
        [Serializable]
        private class MoveData
        {
            public RectTransform rectTransform;
            public Vector2 startPosition;
            public Vector2 endPosition;
        }

        [SerializeField] private AnimationCurve openCurve;
        [SerializeField] private AnimationCurve closeCurve;

        [SerializeField]
        private MoveData[] objects;

        //============================================================================================================//

        protected override void Start()
        {
            Assert.IsNotNull(openCurve);
            Assert.IsNotNull(closeCurve);
            
            for (int i = 0; i < objects.Length; i++)
            {
                var data = objects[i];
                data.rectTransform.anchoredPosition = Vector3.Lerp(data.startPosition, data.endPosition, startingValue);
            }
        }


        //============================================================================================================//
        
        public override Coroutine DoAnimation(float time, ANIM_DIR animDir)
        {
            return StartCoroutine(DoAnimationCoroutine(time, animDir));
        }

        private IEnumerator DoAnimationCoroutine(float time, ANIM_DIR animDir)
        {
            
            for (int i = 0; i < objects.Length; i++)
            {
                var moveData = objects[i];
                var startPosition = animDir == ANIM_DIR.TO_END ? moveData.startPosition : moveData.endPosition;
                var endPosition = animDir == ANIM_DIR.TO_END ? moveData.endPosition : moveData.startPosition;
                var curve = animDir == ANIM_DIR.TO_END ? closeCurve : openCurve;

                StartCoroutine(MoveUIToPositionCoroutine(moveData.rectTransform, startPosition, endPosition, time, curve));
            }

            yield return new WaitForSeconds(time);
        }
        
        private IEnumerator MoveUIToPositionCoroutine(RectTransform target, Vector3 startPosition, Vector3 endPosition, float time, AnimationCurve animationCurve)
        {
            if (Vector2.Distance(target.anchoredPosition, endPosition) < 0.05f)
            {
                target.anchoredPosition = endPosition;
                yield break;
            }
            
            for (var t = 0f; t <= time; t += Time.deltaTime)
            {
                var dt = t / time;
            

                target.anchoredPosition = Vector2.Lerp(startPosition, endPosition, animationCurve.Evaluate(dt));
                    
                yield return null;
            }

            target.anchoredPosition = endPosition;
        }

        //============================================================================================================//
        
    }
}
