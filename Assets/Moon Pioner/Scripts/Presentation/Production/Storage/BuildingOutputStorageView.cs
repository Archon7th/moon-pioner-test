using System;
using System.Linq;
using UnityEngine;

namespace Presentation.Production
{
    public class BuildingOutputStorageView : BuildingStorageView
    {
        internal void ProcessOutputStorage(float deltaTime)
        {
            if (_playersInArea.Count == 0 || Amount == 0)
                return;

            foreach (var player in _playersInArea)
            {
                var resourcePoint = storagePoints.LastOrDefault(point => !point.IsEmpty && !point.IsTranfered);
                if (resourcePoint == default)
                    break;

                if (player.PlayerStorage.IsLockedTime)
                    continue;

                var emptyPlace = player.PlayerStorage.FindEmptyPlace();
                if (emptyPlace == null)
                {
                    player.PlayerStorage.SetLockedTime(1f);
                    //TODO: message about full player storage
                    Debug.LogWarning("No empty place in storage");
                    continue;
                }
                player.PlayerStorage.SetLockedTime(0.5f);
                var resource = resourcePoint.ReleaseStore();
                emptyPlace.BeforeStore(resource);
                resource.PlayMoveAnimation(() =>
                {
                    emptyPlace.CompleteStore();
                    player.PlayerStorage.RecalculateAmount();
                });
                RecalculateAmount();
            }
        }
    }
}