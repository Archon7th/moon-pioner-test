using System.Linq;
using UnityEngine;
using VContainer;

namespace Presentation.Production
{
    public class BuildingStorageView : StorageViewBase
    {
        [SerializeField] protected ResourceType[] AcceptedTypes;

        [SerializeField] protected BuildingStoragePoint[] storagePoints;

        [Inject] protected ProductionBuilding _building;

        private int _cachedAmount = -1;

        public override bool IsAcceptedResource(ResourceType resourceType)
        {
            return AcceptedTypes.Contains(resourceType);
        }

        public override bool PutIntoStorage(ResourceView resource)
        {
            var emptyPlace = FindEmptyPlace();
            if (emptyPlace == null)
            {
                Debug.LogError("Critical: No empty place in storage", this);
                return false;
            }
            emptyPlace.BeforeStore(resource);
            resource.PlayMoveAnimation(() =>
            {
                emptyPlace.CompleteStore();
            });
            RecalculateAmount();
            return true;
        }

        public BuildingStoragePoint FindEmptyPlace()
        {
            foreach (var storagePoint in storagePoints)
            {
                if (storagePoint.IsEmpty)
                    return storagePoint;
            }
            return null;
        }

        public void RecalculateAmount()
        {
            _cachedAmount = 0;
            foreach (var storagePoint in storagePoints)
            {
                if (!storagePoint.IsEmpty)
                    _cachedAmount++;
            }
        }

        public override float Capacity => storagePoints.Length;
        public override float Amount => _cachedAmount;

    }
}