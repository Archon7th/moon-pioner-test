using UnityEngine;

namespace Presentation.Production
{
    public interface IProductionState
    {
        void Enter(ProductionBuilding building);
        void Update(ProductionBuilding building, float deltaTime);
        bool ReadyToUpdate();
    }

    public class TimedState
    {
        protected float _waitTime = 0f;

        protected void ResetWaitTime(float time)
        {
            _waitTime = Time.time + time;
        }

        public bool ReadyToUpdate()
        {
            return Time.time > _waitTime;
        }
    }

    public class IdleState : TimedState, IProductionState
    {
        public void Enter(ProductionBuilding building)
        {
            building.HandleUpdateStorages();
            ResetWaitTime(1);
        }

        public void Update(ProductionBuilding building, float deltaTime)
        {
            if (building.Config.InputResources.Length > 0 && building.InputStorage != null)
            {
                if (building.OutputStorage.IsFull)
                {
                    //Debug.LogWarning($"Output storage is full {building.OutputStorage.Amount}/{building.OutputStorage.Capacity}", building.OutputStorage);
                    building.ShowFloatingMessage(building.LocalizationCache.ProductionStopNoSpace.GetLocalizedString());
                    ResetWaitTime(1);
                    return;
                }
                var inputPoints = building.InputStorage.RequireInputResources(building.Config.InputResources);
                foreach (var inputPoint in inputPoints)
                {
                    if (inputPoint == null)
                    {
                        ResetWaitTime(1);
                        //Debug.LogWarning("Missing resources for production", building.InputStorage);
                        building.ShowFloatingMessage(building.LocalizationCache.ProductionStopNoResouces.GetLocalizedString());
                        return;
                    }
                }

                foreach (var inputPoint in inputPoints)
                {
                    var emptyPlace = building.GetFreeInputPoint();
                    if (emptyPlace == null)
                    {
                        ResetWaitTime(1);
                        Debug.LogError("Critical: No free input point", building);
                        break;
                    }
                    else
                    {
                        var resource = inputPoint.ReleaseStore();
                        emptyPlace.BeforeStore(resource);
                        resource.PlayMoveAnimation(() =>
                        {
                            emptyPlace.CompleteStore();
                            building.InputStorage.RecalculateAmount();
                        });

                    }
                }

                building.ChangeState(new LoadState());
            }
            else if (building.OutputStorage != null)
            {
                if (building.OutputStorage.IsFull)
                {
                    //Debug.LogWarning($"Output storage is full {building.OutputStorage.Amount}/{building.OutputStorage.Capacity}", building.OutputStorage);
                    building.ShowFloatingMessage(building.LocalizationCache.ProductionStopNoSpace.GetLocalizedString());
                    ResetWaitTime(1);
                    return;
                }

                building.ChangeState(new ProductionState());
            }
            else
            {
                building.ChangeState(new DummyState());
            }
        }
    }

    public class LoadState : TimedState, IProductionState
    {
        public void Enter(ProductionBuilding building)
        {
            if (building.Config.InputResources.Length > 0)
            {
                ResetWaitTime(building.Config.ResourceTranferTime);
            }
        }

        public void Update(ProductionBuilding building, float deltaTime)
        {
            if (building.OutputStorage.IsFull)
            {
                //Debug.LogWarning("Output storage is full", building.OutputStorage);
                building.ShowFloatingMessage(building.LocalizationCache.ProductionStopNoSpace.GetLocalizedString());
                ResetWaitTime(1);
            }
            else if (building.CanStartProduction())
            {
                building.ChangeState(new ProductionState());
            }
        }
    }

    public class ProductionState : TimedState, IProductionState
    {
        public void Enter(ProductionBuilding building)
        {
            ResetWaitTime(building.Config.ProductionTime);
            building.HandleConsumeResources();
            building.BuildingView.StartBounce(building.Config.ProductionTime);
        }

        public void Update(ProductionBuilding building, float deltaTime)
        {
            building.BuildingView.StopBounce();
            building.HandleProduceResources();
            building.ChangeState(new CompleteState());
        }
    }

    public class CompleteState : TimedState, IProductionState
    {
        public void Enter(ProductionBuilding building)
        {
            if (building.Config.OutputResources.Length > 0)
            {
                ResetWaitTime(building.Config.ResourceTranferTime);
            }
            building.HandleOutputResources();
        }

        public void Update(ProductionBuilding building, float deltaTime)
        {
            building.ChangeState(new IdleState());
        }
    }

    public class DummyState : IProductionState
    {
        public void Enter(ProductionBuilding building)
        {
            Debug.LogWarning("Building entered to dummy state", building);
        }

        public bool ReadyToUpdate()
        {
            return false;
        }

        public void Update(ProductionBuilding building, float deltaTime)
        {
        }
    }
}