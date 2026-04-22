using System;
using System.Linq;
using UnityEngine;

namespace Presentation.Production
{
    public class BuildingInputStorageView : BuildingStorageView
    {
        internal BuildingStoragePoint[] RequireInputResources(ResourceType[] inputResources)
        {
            int[] ids = inputResources.Select(res => (int)res).ToArray();
            BuildingStoragePoint[] resources = new BuildingStoragePoint[inputResources.Length];
            for (int i = 0; i < storagePoints.Length; i++)
            {
                if (storagePoints[i].IsEmpty || storagePoints[i].IsTranfered)
                    continue;

                int idx = Array.IndexOf(ids, (int) storagePoints[i].ResourceType);
                if (idx >= 0)
                {
                    ids[idx] = -1;
                    resources[idx] = storagePoints[i];
                }
            }
            return resources;
        }

        internal void ProcessInputStorage(float deltaTime)
        {
            if (_playersInArea.Count == 0 || IsFull)
                return;

            foreach (var player in _playersInArea)
            {
                if (player.PlayerStorage.IsLockedTime)
                    continue;

                var emptyPlace = FindEmptyPlace();
                if (emptyPlace == null)
                {
                    player.PlayerStorage.SetLockedTime(1f);
                    //TODO: message about full player storage
                    Debug.LogWarning("No empty place in storage");
                    continue;
                }
                

                var resourcePoint = player.PlayerStorage.RequireInputResourceFor(this);
                if (resourcePoint == default)
                {
                    player.PlayerStorage.SetLockedTime(1f);
                    break;
                }

                player.PlayerStorage.SetLockedTime(0.5f);
                var resource = resourcePoint.ReleaseStore();
                emptyPlace.BeforeStore(resource);
                resource.PlayMoveAnimation(() =>
                {
                    emptyPlace.CompleteStore();
                    RecalculateAmount();
                });
                player.PlayerStorage.RecalculateAmount();
            }
        }
    }
}