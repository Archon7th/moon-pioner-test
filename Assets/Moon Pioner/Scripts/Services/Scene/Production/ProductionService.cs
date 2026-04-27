using Presentation.Production;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Services.Scene
{
    public class ProductionService : ITickable, IProductionService, IDisposable
    {
        private readonly List<ProductionBuilding> _buildings = new();

        public void AddBuilding(ProductionBuilding building)
        {
            _buildings.Add(building);
            building.InitializeProduction();
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;

            foreach (var building in _buildings)
            {
                building.ProcessProduction(deltaTime);
            }
        }
        public void Dispose()
        {
            _buildings.Clear();
        }
    }
}