using Presentation.Views;
using Services.Scene;
using UnityEngine;
using VContainer.Unity;

namespace Systems
{
    public sealed class PlayerController : IStartable, IFixedTickable
    {
        private readonly IMobileInputService _input;
        private readonly PlayerView _view;
        private readonly PlayerConfig _config;
        private readonly CameraView _camera;

        public PlayerController(IMobileInputService input, PlayerView view, PlayerConfig config, CameraView camera)
        {
            _input = input;
            _view = view;
            _config = config;
            _camera = camera;
        }

        public void Start()
        {
            _view.PlayerStorage.Initialize(_config.StorageLimit);
        }

        public void FixedTick()
        {
            Vector2 inputDir = _input.GetMovement();

            if (inputDir.sqrMagnitude > 0.01f)
            {
                Vector3 forward = _camera.ForwardAnchor;
                Vector3 right = _camera.RightAnchor;
                Vector3 moveDirection = (forward * inputDir.y + right * inputDir.x).normalized;

                float speed = _config.MoveSpeed * inputDir.magnitude;
                Vector3 offset = moveDirection * speed * Time.fixedDeltaTime;

                _view.FixedMovePlayer(offset);
                _view.RotatePlayer(moveDirection);
            }
            else
            {
                _view.FixedMovePlayer(Vector3.zero);
            }
        }


    }
}