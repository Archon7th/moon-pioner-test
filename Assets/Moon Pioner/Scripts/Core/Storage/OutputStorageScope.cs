using Presentation.Production;
using Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class OutputStorageScope : LifetimeScope
{
    [SerializeField] private BuildingStorageView view;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(view);
        builder.RegisterEntryPoint<OutputStorageController>(Lifetime.Scoped);
    }
}
