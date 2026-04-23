using Presentation.Production;
using Presentation.Views;
using Services.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Systems
{
    public class BuildingStorageController : ITickable, IInitializable, IDisposable
    {
        [Inject] protected LocalizationCache _soLocalizationCache;

        protected readonly BuildingStorageView _view;

        protected readonly HashSet<PlayerView> _playersInZone = new();

        public BuildingStorageController(BuildingStorageView view) => _view = view;

        public void Initialize()
        {
            _view.OnPlayerEntered += HandleEnter;
            _view.OnPlayerExited += HandleExit;
        }

        private void HandleEnter(PlayerView player) => _playersInZone.Add(player);
        private void HandleExit(PlayerView player) => _playersInZone.Remove(player);

        public void Dispose()
        {
            _view.OnPlayerEntered -= HandleEnter;
            _view.OnPlayerExited -= HandleExit;
            _playersInZone.Clear();
        }

        public bool HasPlayers => _playersInZone.Count > 0;

        public virtual void Tick()
        {
            //_view.PerformUpdate(Time.deltaTime);
        }

    }
}