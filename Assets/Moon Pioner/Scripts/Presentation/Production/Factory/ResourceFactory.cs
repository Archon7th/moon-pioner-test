using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Presentation.Production
{
    public class ResourceFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly ResourceView[] _prefabs;
        private readonly Dictionary<ResourceType, ObjectPool<ResourceView>> _pools = new();

        [Inject]
        public ResourceFactory(IObjectResolver resolver, ResourceView[] prefabs)
        {
            _resolver = resolver;
            _prefabs = prefabs;

            var resourceTypes = System.Enum.GetValues(typeof(ResourceType));

            if (_prefabs.Length != resourceTypes.Length)
            {
                Debug.LogError("Critical: Prefabs array length does not match the number of ResourceTypes.");
                return;
            }

            foreach (ResourceType type in resourceTypes)
            {
                _pools[type] = new ObjectPool<ResourceView>(
                    createFunc: () => _resolver.Instantiate<ResourceView>(_prefabs[(int)type]),
                    actionOnGet: obj => obj.gameObject.SetActive(true),
                    actionOnRelease: obj => obj.gameObject.SetActive(false),
                    actionOnDestroy: Object.Destroy
                );
            }
        }

        public ResourceView Get(ResourceType type) => _pools[type].Get();

        public void Return(ResourceType type, ResourceView item) => _pools[type].Release(item);
    }
}