using Presentation.Production;
using System.Linq;


namespace Systems
{
    public class OutputStorageController : BuildingStorageController
    {
        public OutputStorageController(BuildingStorageView view) : base(view) { }

        public override void Tick()
        {
            if (_playersInZone.Count == 0 || _view.IsEmpty)
                return;

            foreach (var player in _playersInZone)
            {
                if (player.PlayerStorage.IsLockedTime)
                    continue;

                var resourcePoint = _view.StoragePoints.LastOrDefault(point => !point.IsEmpty && !point.IsTranfered);
                if (resourcePoint == default)
                    break;

                var emptyPlace = player.PlayerStorage.FindEmptyPlace();
                if (emptyPlace == null)
                {
                    player.PlayerStorage.SetLockedTime(1f);
                    //Debug.LogWarning("No empty place in player storage");
                    player.ShowFloatingMessage(_soLocalizationCache.PlayerNotEnoughtSpace.GetLocalizedString());
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
                _view.RecalculateAmount();
            }
        }
    }
}