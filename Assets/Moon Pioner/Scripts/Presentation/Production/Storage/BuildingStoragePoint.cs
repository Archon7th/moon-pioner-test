using DG.Tweening;
using System;
using UnityEngine;

namespace Presentation.Production
{
    public class BuildingStoragePoint : MonoBehaviour
    {
        private ResourceView _resourceView;

        public void BeforeStore(ResourceView resourceView)
        {
            _resourceView = resourceView;
            _resourceView.transform.SetParent(transform);
            IsTranfered = true;
        }
        public void CompleteStore()
        {
            _resourceView.transform.SetLocalPositionAndRotation(Vector3.zero, transform.rotation);
            IsTranfered = false;
        }

        public ResourceView ReleaseStore()
        {
            var released = _resourceView;
            released.transform.SetParent(null);
            _resourceView = null;
            return released;
        }

        internal ResourceType ResourceType => _resourceView.ResourceType;

        internal ResourceView ResourceView => _resourceView;

        public bool IsEmpty => _resourceView == null;

        public Vector3 StorePosition => transform.position;

        public bool IsTranfered { get; internal set; }
    }
}