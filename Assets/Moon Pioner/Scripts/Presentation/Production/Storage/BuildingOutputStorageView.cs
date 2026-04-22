using System.Linq;

namespace Presentation.Production
{
    public class BuildingOutputStorageView : BuildingStorageView
    {
        internal void ProcessOutputStorage(float deltaTime)
        {
            if (_playersInArea.Count == 0 || IsEmpty)
                return;

            foreach (var player in _playersInArea)
            {
                if (player.PlayerStorage.IsLockedTime)
                    continue;

                var resourcePoint = storagePoints.LastOrDefault(point => !point.IsEmpty && !point.IsTranfered);
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
                RecalculateAmount();
            }
        }
    }
}