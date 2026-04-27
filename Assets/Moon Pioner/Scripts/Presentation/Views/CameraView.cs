using Extensions;
using Services.Scene;
using UnityEngine;
using VContainer;

namespace Presentation.Views
{
    [RequireComponent(typeof(Camera))]
    public sealed class CameraView : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(-1.5f, 3, -1.5f);
        [SerializeField] private float smoothTime = 0.15f;

        [Header("Look Ahead (Prediction)")]
        [SerializeField] private float leadDistance = 2f;
        [SerializeField] private float leadSmoothTime = 0.4f;
        [SerializeField] private float leadCameraFlow = 0.5f;

        [Range(0f, 90f)]
        [SerializeField] private float deadzoneAngle = 25f;

        [Inject] private PlayerTransformVariable _playerTransformVar;

        public Camera Camera { get; private set; }
        public Vector3 ForwardAnchor { get; internal set; }
        public Vector3 RightAnchor { get; internal set; }

        private Vector3 _currentVelocity;
        private Vector3 _currentLeadVelocity;
        private Vector3 _smoothLead;
        private Vector3 _stableTargetLead;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            ForwardAnchor = transform.forward.Flat().normalized;
            RightAnchor = transform.right.Flat().normalized;
        }

        private void LateUpdate()
        {
            if (_playerTransformVar?.Value == null) return;

            Transform playerTransform = _playerTransformVar.Value;
            Vector3 playerForward = playerTransform.forward;

            if (_stableTargetLead == Vector3.zero) _stableTargetLead = playerForward * leadDistance;

            float angle = Vector3.Angle(_stableTargetLead, playerForward);

            if (angle > deadzoneAngle)
            {
                float lerpFactor = (angle - deadzoneAngle) / angle;
                Vector3 targetDirection = playerForward * leadDistance;
                _stableTargetLead = Vector3.Slerp(_stableTargetLead, targetDirection, lerpFactor * Time.deltaTime);
            }

            _smoothLead = Vector3.SmoothDamp(_smoothLead, _stableTargetLead, ref _currentLeadVelocity, leadSmoothTime);

            Vector3 targetPosition = playerTransform.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);

            transform.LookAt(playerTransform.position + _smoothLead * leadCameraFlow);
        }

        private void OnDrawGizmos()
        {
            if (_playerTransformVar?.Value == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_playerTransformVar.Value.position + _smoothLead, 0.3f);
            Gizmos.DrawLine(_playerTransformVar.Value.position, _playerTransformVar.Value.position + _smoothLead);
        }
    }
}