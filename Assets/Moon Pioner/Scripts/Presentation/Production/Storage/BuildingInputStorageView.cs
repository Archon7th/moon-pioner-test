using System;
using System.Linq;
using UnityEngine;

namespace Presentation.Production
{
    public class BuildingInputStorageView : BuildingStorageView
    {
        internal BuildingStoragePoint[] RequireInputResources(ResourceType[] inputResources)
        {
            int[] ids = inputResources.Select(res => (int)res).ToArray();
            BuildingStoragePoint[] resources = new BuildingStoragePoint[inputResources.Length];
            for (int i = 0; i < storagePoints.Length; i++)
            {
                if (storagePoints[i].IsEmpty || storagePoints[i].IsTranfered)
                    continue;

                int idx = Array.IndexOf(ids, (int) storagePoints[i].ResourceType);
                if (idx >= 0)
                {
                    ids[idx] = -1;
                    resources[idx] = storagePoints[i];
                }
            }
            return resources;
        }

        internal void ProcessInputStorage(float deltaTime)
        {

        }


    }
}