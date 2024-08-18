using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities.Animations;
using Utilities.Debugging;

namespace Animations
{
    public class WaitForMoveAnimations : WaitForAnimationBase
    {
        [Serializable]
        private class MoveData
        {
            public Transform transform;
            public Vector3 startPosition;
            public Vector3 endPosition;
        }

        [SerializeField] private AnimationCurve openCurve;
        [SerializeField] private AnimationCurve closeCurve;

        [SerializeField]
        private MoveData[] doors;

        //============================================================================================================//

        protected override void Start()
        {
            Assert.IsNotNull(openCurve);
            Assert.IsNotNull(closeCurve);
            
            for (int i = 0; i < doors.Length; i++)
            {
                var data = doors[i];
                data.transform.position = Vector3.Lerp(data.startPosition, data.endPosition, startingValue);
            }
        }


        //============================================================================================================//
        
        public override Coroutine DoAnimation(float time, ANIM_DIR animDir)
        {
            return StartCoroutine(DoAnimationCoroutine(time, animDir));
        }

        private IEnumerator DoAnimationCoroutine(float time, ANIM_DIR animDir)
        {

            for (int i = 0; i < doors.Length; i++)
            {
                var door = doors[i];
                var startPosition = animDir == ANIM_DIR.TO_END ? door.startPosition : door.endPosition;
                var endPosition = animDir == ANIM_DIR.TO_END ? door.endPosition : door.startPosition;
                var curve = animDir == ANIM_DIR.TO_END ? closeCurve : openCurve;

                StartCoroutine(MoveToPositionCoroutine(door.transform, startPosition, endPosition, time, curve));
            }

            yield return new WaitForSeconds(time);
        }

        //============================================================================================================//

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (doors == null || doors.Length == 0)
                return;
            
            for (int i = 0; i < doors.Length; i++)
            {
                var door = doors[i];

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(door.startPosition, door.endPosition);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(door.startPosition, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(door.endPosition, 0.5f);
                
                var midPoint = (door.startPosition + door.endPosition) / 2f;
                Draw.Label(midPoint, door.transform.name);
            }
        }
#endif
    }
}
