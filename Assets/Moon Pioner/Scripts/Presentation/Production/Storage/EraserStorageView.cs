using System.Linq;
using UnityEngine;

namespace Presentation.Production
{
    public class EraserStorageView : BuildingStorageView
    {
        public override bool IsAcceptedResource(ResourceType resourceType)
        {
            return true;
        }
    }
}
