using Presentation.Production;
using Presentation.Views;
using Services.Scene;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Systems
{
    public class InputStorageController : BuildingStorageController
    {
        public InputStorageController(BuildingStorageView view) : base(view) { }

        public override void Tick()
        {
            if (_playersInZone.Count == 0 || _view.IsFull)
                return;

            foreach (var player in _playersInZone)
            {
                if (player.PlayerStorage.IsLockedTime)
                    continue;

                var resourcePoint = player.PlayerStorage.RequireInputResourceFor(_view);
                if (resourcePoint == default)
                {
                    player.PlayerStorage.SetLockedTime(1f);
                    continue;
                }

                var emptyPlace = _view.FindEmptyPlace();
                if (emptyPlace == default)
                {
                    player.PlayerStorage.SetLockedTime(1f);
                    //Debug.LogWarning("No empty place in storage");
                    player.ShowFloatingMessage(_soLocalizationCache.StorageNotEnoughtSpace.GetLocalizedString());
                    break;
                }

                player.PlayerStorage.SetLockedTime(0.5f);
                var resource = resourcePoint.ReleaseStore();
                emptyPlace.BeforeStore(resource);
                resource.PlayMoveAnimation(() =>
                {
                    emptyPlace.CompleteStore();
                    _view.RecalculateAmount();
                });
                player.PlayerStorage.RecalculateAmount();
            }
        }
    }
}