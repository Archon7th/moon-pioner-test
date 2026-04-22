using UnityEngine;
using VContainer;

namespace Presentation.Production
{
    public abstract class StorageViewBase : MonoBehaviour
    {
        protected float _lockedTime = 0;

        [Inject] protected LocalizationCache _soLocalizationCache;

        public bool IsFull => Amount >= Capacity;
        public bool IsEmpty => Amount <= 0;
        public bool IsLockedTime => Time.time < _lockedTime;

        public abstract float Capacity { get; }
        public abstract float Amount { get; }

        public abstract bool PutIntoStorage(ResourceView resource);

        public virtual bool IsAcceptedResource(ResourceType resourceType)
        {
            return true;
        }

        public void SetLockedTime(float time)
        {
            _lockedTime = Time.time + time;
        }

        private void OnDrawGizmos()
        {
            var points = GetComponentsInChildren<BuildingStoragePoint>();
            if (points != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in points)
                {
                    Gizmos.DrawWireSphere(point.StorePosition, 0.1f);
                }
            }
        }
    }
}