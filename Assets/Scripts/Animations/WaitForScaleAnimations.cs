using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities.Animations;

namespace Animations
{
    public class WaitForScaleAnimations: WaitForAnimationBase
    {
        [Serializable]
        private class ScaleData
        {
            public Transform transform;
            public Vector3 startScale;
            public Vector3 endScale;
        }
        
        [SerializeField] private AnimationCurve openCurve;
        [SerializeField] private AnimationCurve closeCurve;

        [SerializeField]
        private ScaleData[] objects;

        protected override void Start()
        {
            Assert.IsNotNull(openCurve);
            Assert.IsNotNull(closeCurve);

            for (int i = 0; i < objects.Length; i++)
            {
                var data = objects[i];
                data.transform.localScale = Vector3.Lerp(data.startScale, data.endScale, startingValue);
            }
            
        }

        public override IEnumerator DoAnimationCoroutine(float time, ANIM_DIR animDir)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                var startPosition = animDir == ANIM_DIR.TO_END ? obj.startScale : obj.endScale;
                var endScale = animDir == ANIM_DIR.TO_END ? obj.endScale : obj.startScale;
                var curve = animDir == ANIM_DIR.TO_END ? closeCurve : openCurve;

                StartCoroutine(ScaleCoroutine(obj.transform, startPosition, endScale, time, curve));
            }

            yield return new WaitForSeconds(time);
        }
    }
}