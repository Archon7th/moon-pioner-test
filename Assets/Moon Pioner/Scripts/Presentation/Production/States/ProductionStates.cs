using UnityEngine;

namespace Presentation.Production
{
    public interface IProductionState
    {
        void Enter(ProductionBuilding building);
        void Update(ProductionBuilding building);
        bool ReadyToUpdate(float deltaTime);

    }

    public abstract class BaseProductionState : IProductionState
    {
        protected float _timer;
        public abstract void Enter(ProductionBuilding building);
        public abstract void Update(ProductionBuilding building);
        public bool ReadyToUpdate(float deltaTime) => (_timer -= deltaTime) <= 0;

        protected void SetTimer(float time = 1) => _timer = time;
    }

    public class IdleState : BaseProductionState
    {
        public override void Enter(ProductionBuilding building)
        {
            building.HandleUpdateStorages();
            SetTimer();
        }

        public override void Update(ProductionBuilding building)
        {
            if (building.Config.InputResources.Length > 0 && building.InputStorage != null)
            {
                if (building.CheckOutputStorageIsFull())
                {
                    SetTimer();
                    return;
                }
                var inputPoints = building.InputStorage.RequireInputResources(building.Config.InputResources);
                foreach (var inputPoint in inputPoints)
                {
                    if (inputPoint == null)
                    {
                        SetTimer();
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
                        SetTimer();
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
                if (building.CheckOutputStorageIsFull())
                {
                    SetTimer();
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

    public class LoadState : BaseProductionState
    {
        public override void Enter(ProductionBuilding building)
        {
            if (building.Config.InputResources.Length > 0)
            {
                SetTimer(building.Config.ResourceTranferTime);
            }
        }

        public override void Update(ProductionBuilding building)
        {
            if (building.CheckOutputStorageIsFull())
            {
                SetTimer();
                return;
            }
            if (building.CanStartProduction())
            {
                building.ChangeState(new ProductionState());
            }
        }
    }

    public class ProductionState : BaseProductionState
    {
        public override void Enter(ProductionBuilding building)
        {
            SetTimer(building.Config.ProductionTime);
            building.HandleConsumeResources();
            building.BuildingView.StartBounce(building.Config.ProductionTime);
        }

        public override void Update(ProductionBuilding building)
        {
            building.BuildingView.StopBounce();
            building.HandleProduceResources();
            building.ChangeState(new CompleteState());
        }
    }

    public class CompleteState : BaseProductionState
    {
        public override void Enter(ProductionBuilding building)
        {
            if (building.Config.OutputResources.Length > 0)
            {
                SetTimer(building.Config.ResourceTranferTime);
            }
            building.HandleOutputResources();
        }

        public override void Update(ProductionBuilding building)
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

        public bool ReadyToUpdate(float deltaTime)
        {
            return false;
        }

        public void Update(ProductionBuilding building)
        {
        }
    }
}