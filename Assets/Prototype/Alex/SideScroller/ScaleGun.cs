using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Prototype.Alex.SideScroller
{
    public class ScaleGun : MonoBehaviour
    {
        [SerializeField, Min(1)]
        public float scaleDif;

        [SerializeField, Min(0.1f)]
        public float scaleTime = 0.1f;

        [SerializeField]
        public LayerMask mask;
            
        public AnimationCurve curve;
        private Camera _camera;
        private Vector3 mouseWorldPosition;

        //============================================================================================================//
        
        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            mouseWorldPosition = GetMouseWorldPosition();

            var pos = transform.position;
            var dir = (mouseWorldPosition - pos).normalized;

            Debug.DrawLine(pos, pos + dir * 30f, Color.green);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var target = GetTransform(pos, dir);

                if (target == null)
                    return;
                
                ScaleTransform(target, true);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var target = GetTransform(pos, dir);
                
                if (target == null)
                    return;

                ScaleTransform(target, false);
            }
        }

        private void ScaleTransform(Transform target, bool isXAxis)
        {
            IEnumerator ScaleCoroutine(Vector3 targetScale, float time)
            {
                var currentScale = target.localScale;
                for (var t = 0f; t <= time; t += Time.deltaTime)
                {
                    var dt = t / time;

                    target.transform.localScale = Vector3.Lerp(currentScale, targetScale, curve.Evaluate(dt));
                    
                    yield return null;
                }
            }

            var targetScale = Vector3.one;
            if (isXAxis)
            {
                targetScale.x = scaleDif;
            }
            else
            {
                targetScale.y = scaleDif;
            }

            StartCoroutine(ScaleCoroutine(targetScale, scaleTime));
        }


        private Transform GetTransform(Vector2 pos, Vector2 dir)
        {
            var hit = Physics2D.RaycastAll(pos, dir, 30, mask.value);

            return hit.Length <= 0 ? null : hit.First().transform;
        }
        
        
        //============================================================================================================//
        
        private Vector3 GetMouseWorldPosition()
        {
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            return pos;
        }
    }
}