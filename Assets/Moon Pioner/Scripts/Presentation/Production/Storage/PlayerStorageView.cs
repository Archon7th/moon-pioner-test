using System.Linq;
using UnityEngine;

namespace Presentation.Production
{
    public class PlayerStorageView : StorageViewBase
    {
        [SerializeField] protected BuildingStoragePoint pointPreab;

        protected BuildingStoragePoint[] storagePoints;

        private int _cachedAmount = -1;

        public void Initialize(int capacity)
        {
            storagePoints = new BuildingStoragePoint[capacity];
            for (int i = 0; i < capacity; i++)
            {
                var point = Instantiate(pointPreab, transform);
                point.transform.localPosition = Vector3.up * i * 0.1f;
                storagePoints[i] = point;
            }
        }

        internal BuildingStoragePoint RequireInputResourceFor(StorageViewBase inputStorage)
        {
            return storagePoints.LastOrDefault(point => !point.IsEmpty && inputStorage.IsAcceptedResource(point.ResourceType));
        }

        public override bool PutIntoStorage(ResourceView resource)
        {
            var emptyPlace = FindEmptyPlace();
            if (emptyPlace == null)
            {
                Debug.LogError("Critical: No empty place in storage but requested", this);
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