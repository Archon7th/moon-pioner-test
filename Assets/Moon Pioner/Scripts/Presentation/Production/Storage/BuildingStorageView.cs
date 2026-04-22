using Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using static UnityEngine.UI.Image;

namespace Presentation.Production
{
    public class BuildingStorageView : StorageViewBase
    {

        [SerializeField] protected BuildingStoragePoint[] storagePoints;

        [Inject] protected ProductionBuilding _building;
        
        protected HashSet<PlayerView> _playersInArea = new HashSet<PlayerView>();

        private int _cachedAmount = -1;

        protected ResourceType[] _acceptedTypes;

        internal void InitializeStorage(ResourceType[] acceptedTypes)
        {
            _acceptedTypes = acceptedTypes;
        }

        public override bool IsAcceptedResource(ResourceType resourceType)
        {
            return _acceptedTypes.Contains(resourceType);
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

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerView>();
            if (player)
            {
                _playersInArea.Add(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponent<PlayerView>();
            if (player)
            {
                _playersInArea.Remove(player);
            }
        }
    }
}