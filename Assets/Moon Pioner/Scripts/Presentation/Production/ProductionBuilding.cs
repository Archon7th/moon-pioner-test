using System;
using UnityEngine;
using VContainer;

namespace Presentation.Production
{
    public class ProductionBuilding : MonoBehaviour
    {
        protected ResourceFactory _resourceFactory;

        [SerializeField] protected ProductionConfig soProductionConfig;
        [SerializeField] protected BuildingInputStorageView inputStorage;
        [SerializeField] protected BuildingOutputStorageView outputStorage;

        [SerializeField] protected BuildingStoragePoint[] inputPoints;
        [SerializeField] protected BuildingStoragePoint[] outputPoints;

        [SerializeField] protected BuildingView buildingView;

        [Inject]
        public void Construct(ResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        private IProductionState _currentState;

        public ProductionConfig Config => soProductionConfig;
        public BuildingInputStorageView InputStorage => inputStorage;
        public BuildingOutputStorageView OutputStorage => outputStorage;
        public BuildingView BuildingView => buildingView;


        public void InitializeProduction()
        {
            ChangeState(new IdleState());

            if (inputStorage != null)
                inputStorage.InitializeStorage(soProductionConfig.InputResources);
            if (outputStorage != null)
                outputStorage.InitializeStorage(soProductionConfig.OutputResources);
        }

        public void ProcessProduction(float deltaTime)
        {
            if (inputStorage != null)
                inputStorage.ProcessInputStorage(deltaTime);
            if (outputStorage != null)
                outputStorage.ProcessOutputStorage(deltaTime);

            if (!soProductionConfig)
                return;

            if (_currentState.ReadyToUpdate())
                _currentState.Update(this, deltaTime);
            
        }
        public void ChangeState(IProductionState newState)
        {
            //_currentState?.Exit(this);
            _currentState = newState;
            _currentState.Enter(this);
        }

        internal BuildingStoragePoint GetFreeInputPoint()
        {
            foreach (var inputPoint in inputPoints)
            {
                if (inputPoint.IsEmpty)
                    return inputPoint;
            }
            return null;
        }

        internal bool CanStartProduction()
        {
            foreach (var inputPoint in inputPoints)
            {
                if (inputPoint.IsEmpty || inputPoint.IsTranfered)
                {
                    return false;
                }
            }
            return true;
        }

        internal void HandleConsumeResources()
        {
            foreach (var inputPoint in inputPoints)
            {
                if (inputPoint.IsEmpty)
                    continue;
                var released = inputPoint.ReleaseStore();
                released.Release();
            }
        }

        internal void HandleProduceResources()
        {
            if (outputPoints.Length < soProductionConfig.OutputResources.Length)
            {
                Debug.LogError("Critical: Not enough output points for production output resources");
                return;
            }
            for (int i = 0; i < soProductionConfig.OutputResources.Length; i++)
            {
                var resourceView = _resourceFactory.Get(soProductionConfig.OutputResources[i]);
                outputPoints[i].BeforeStore(resourceView);
                outputPoints[i].CompleteStore();
            }
        }

        internal void HandleOutputResources()
        {
            foreach (var outputPoint in outputPoints)
            {
                if (outputPoint.IsEmpty)
                    continue;

                if (!outputStorage.PutIntoStorage(outputPoint.ResourceView))
                {
                    var released = outputPoint.ReleaseStore();
                    released.Release();
                }
            }
        }


    }
}