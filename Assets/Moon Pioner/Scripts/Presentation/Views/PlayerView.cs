using Presentation.Production;
using Services.Scene;
using UnityEngine;
using VContainer;

namespace Presentation.Views
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerView : MonoBehaviour
    {
        [Inject] private PlayerTransformVariable _playerTransformVar;

        private Rigidbody _rigidbody;

        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 720f;

        [Header("References")]
        [SerializeField] private PlayerStorageView playerStorageView;

        public PlayerStorageView PlayerStorage => playerStorageView;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            playerStorageView?.Initialize(5);
        }

        public void FixedMovePlayer(Vector3 offset)
        {
            _rigidbody.MovePosition(_rigidbody.position + offset);
        }

        public void RotatePlayer(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude > float.Epsilon)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                float step = rotationSpeed * Time.fixedDeltaTime;

                Quaternion nextRotation = Quaternion.RotateTowards(
                    _rigidbody.rotation,
                    targetRotation,
                    step
                );

                _rigidbody.MoveRotation(nextRotation);
            }
        }

        private void OnEnable() => _playerTransformVar.Value = transform;
        private void OnDisable()
        {
            if (_playerTransformVar != null) _playerTransformVar.Value = null;
        }
    }
}