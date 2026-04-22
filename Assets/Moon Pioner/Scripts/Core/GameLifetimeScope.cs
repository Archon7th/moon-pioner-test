using Infrastructure.Input;
using Presentation.Views;
using Services.Scene;
using Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private PlayerConfig soPlayerConfig;

    private PlayerTransformVariable _playerTransformVar = new PlayerTransformVariable();
    private JoystickInputVariable _joystickInputVar = new JoystickInputVariable();

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(soPlayerConfig);
        builder.RegisterInstance(_playerTransformVar);
        builder.RegisterInstance(_joystickInputVar);

        builder.Register<InputSystem_Actions>(Lifetime.Singleton);
        builder.RegisterEntryPoint<MobileInputService>().As<IMobileInputService>();

        builder.RegisterEntryPoint<PlayerController>();
        builder.RegisterEntryPoint<GamePresenter>();

        builder.RegisterComponentInHierarchy<CameraView>();
        builder.RegisterComponentInHierarchy<JoystickView>();
        builder.RegisterComponentInHierarchy<PlayerView>();
    }
}
