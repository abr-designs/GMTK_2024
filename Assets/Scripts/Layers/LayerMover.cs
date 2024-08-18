using System.Collections;
using Interfaces;
using UnityEngine;
using Utilities.Animations;

namespace Layers
{
    public class LayerMover : MonoBehaviour, IMoveLayers
    {
        [SerializeField]
        private WaitForAnimationBase doorAnimation;

        [SerializeField]
        private float animationTime;
        [SerializeField]
        private AnimationCurve animationCurve;

        //IMoveLayers Implementation
        //============================================================================================================//
        
        public Coroutine MoveLayer(int layerIndex, float yScale, Transform targetTransform, Transform containerTransform)
        {
            return StartCoroutine(MoveLayerCoroutine(layerIndex, yScale, targetTransform, containerTransform));
        }
        
        private IEnumerator MoveLayerCoroutine(int layerIndex, float yScale,Transform targetTransform, Transform containerTransform)
        {
            if(doorAnimation)
                yield return doorAnimation.DoAnimation(animationTime, ANIM_DIR.TO_END);

            //Move the Container down
            //------------------------------------------------//
            var objectCurrentPosition = targetTransform.position;
            var endPosition = containerTransform.position + Vector3.up * (layerIndex * yScale);
            
            yield return StartCoroutine(MoveToPositionCoroutine(
                targetTransform, 
                objectCurrentPosition,
                endPosition, 
                animationTime));
            
            if(doorAnimation)
                yield return doorAnimation.DoAnimation(animationTime, ANIM_DIR.TO_START);
            //------------------------------------------------//
        }

        //Coroutines
        //============================================================================================================//
        
        private IEnumerator MoveToPositionCoroutine(Transform target, Vector3 startPosition, Vector3 endPosition, float time)
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