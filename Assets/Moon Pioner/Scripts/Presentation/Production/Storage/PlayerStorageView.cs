using System.Linq;
using UnityEngine;
using VContainer;

namespace Presentation.Production
{
    public class PlayerStorageView : StorageViewBase
    {
        protected BuildingStoragePoint[] storagePoints;

        private int _cachedAmount = -1;

        public override bool PutIntoStorage(ResourceView resource)
        {
            var emptyPlace = FindEmptyPlace();
            if (emptyPlace == null)
            {
                Debug.LogError("No empty place in storage");
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