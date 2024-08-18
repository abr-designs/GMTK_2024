using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace DefaultNamespace
{
    public class ArmHand : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer handRenderer;
        private Transform _handTransform;
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private Sprite openSprite;
        [SerializeField]
        private Sprite closedSprite;

        [SerializeField]
        private float moveSpeed;

        [SerializeField]
        private float elbowDistance;

        [SerializeField, Header("Local Camera Offsets")]
        private Vector3 shoulderAnchor;
        [SerializeField]
        private Vector3 handRestAnchor;

        [SerializeField, Min(0f)]
        private float smoothTime;
        
        private Vector3[] _positions;
        private Vector3[] _targetPositions;
        private Vector3[] _velocities;

        private Transform _cameraTransform;
        private Transform _trackingTransform;
        private Vector3 _localOffset;
        
        

        private void OnEnable()
        {
            CameraInteraction.OnInteractionStarted += OnInteractionStarted;
        }

        private void Start()
        {
            const int POINT_COUNT = 3;
            
            
            
            _cameraTransform = Camera.main.transform;
            _handTransform = handRenderer.transform;

            handRenderer.sprite = openSprite;
            
            
            Assert.IsNotNull(_cameraTransform);
            Assert.IsNotNull(_handTransform);
            Assert.IsNotNull(lineRenderer);

            lineRenderer.positionCount = POINT_COUNT;
            lineRenderer.useWorldSpace = true;

            _positions = new Vector3[POINT_COUNT];
            _targetPositions = new Vector3[POINT_COUNT];
            _velocities = new Vector3[POINT_COUNT];

        }

        private void LateUpdate()
        {
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            Vector3 handTargetPosition;
            if (_trackingTransform == null)
            {
                //TODO Retract

                var target = _cameraTransform.TransformPoint(handRestAnchor);
                handTargetPosition = Vector3.MoveTowards(_targetPositions[2], target, moveSpeed * Time.deltaTime);
                
                handRenderer.sprite = openSprite;
            }
            else
            {
                var target = _trackingTransform.TransformPoint(_localOffset);
                handTargetPosition = Vector3.MoveTowards(_targetPositions[2], target, moveSpeed * Time.deltaTime);
                
                if(Vector3.Distance(target, _handTransform.position) < 0.5f)
                    handRenderer.sprite = closedSprite;
            }

            _targetPositions[0] = _cameraTransform.TransformPoint(shoulderAnchor);
            _targetPositions[2] = handTargetPosition;

            _targetPositions[1] = ((_targetPositions[0] + _targetPositions[2]) / 2f) + (_cameraTransform.right.normalized * elbowDistance);

            for (int i = 0; i < 3; i++)
            {
                _positions[i] = Vector3.SmoothDamp(_positions[i], _targetPositions[i], ref _velocities[i], smoothTime);
            }
            
            lineRenderer.SetPositions(_positions);

            _handTransform.position = _positions[2];
            _handTransform.forward = _cameraTransform.forward;
        }

        private void OnDisable()
        {
            CameraInteraction.OnInteractionStarted -= OnInteractionStarted;
        }
        //============================================================================================================//


        private void OnInteractionStarted(Vector3 worldHitPosition, Transform hitTransform)
        {
            
            _trackingTransform = hitTransform;

            if (_trackingTransform != null)
                _localOffset = _trackingTransform.InverseTransformPoint(worldHitPosition);
        }
    }
}