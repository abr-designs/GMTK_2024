using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities.Animations;

namespace Animations
{
    public class WaitForDoorAnimations : WaitForAnimationBase
    {
        [Serializable]
        private class DoorData
        {
            public Transform transform;
            public Vector3 startPosition;
            public Vector3 endPosition;
        }

        [SerializeField] private AnimationCurve openCurve;
        [SerializeField] private AnimationCurve closeCurve;

        [SerializeField]
        private DoorData[] doors;

        //============================================================================================================//

        private void Start()
        {
            Assert.IsNotNull(openCurve);
            Assert.IsNotNull(closeCurve);
        }


        //============================================================================================================//

        public override IEnumerator DoAnimationCoroutine(float time, bool invert)
        {

            for (int i = 0; i < doors.Length; i++)
            {
                var door = doors[i];
                var startPosition = invert ? door.endPosition : door.startPosition;
                var endPosition = invert ? door.startPosition : door.endPosition;
                var curve = invert ? closeCurve : openCurve;

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
                

            }
        }
#endif
    }
}
