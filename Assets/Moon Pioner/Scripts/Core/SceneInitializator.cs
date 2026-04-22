using UnityEngine;
using VContainer.Unity;
using VContainer;
using Presentation.Production;
using UnityEngine.Rendering.VirtualTexturing;

public class SceneInitializator : IStartable
{
    private readonly IObjectResolver _resolver;
    private readonly IProductionService _productionService;

    [Inject]
    public SceneInitializator(IObjectResolver resolver, IProductionService productionService)
    {
        _resolver = resolver;
        _productionService = productionService;
    }

    public void Start()
    {
        var resourcesOnScene = Object.FindObjectsByType<ResourceView>(FindObjectsSortMode.None);
        foreach (var obj in resourcesOnScene)
        {
            _resolver.Inject(obj);
        }

        var buildingsOnScene = Object.FindObjectsByType<ProductionBuilding>(FindObjectsSortMode.None);
        foreach (var building in buildingsOnScene)
        {
            _resolver.Inject(building);
            if (building.InputStorage != null)
                _resolver.Inject(building.InputStorage);
            if (building.OutputStorage != null)
                _resolver.Inject(building.OutputStorage);
            _productionService.AddBuilding(building);
        }

    }
}