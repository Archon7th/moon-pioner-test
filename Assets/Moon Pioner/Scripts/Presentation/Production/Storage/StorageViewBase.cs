using System.Linq;
using UnityEngine;

namespace Presentation.Production
{
    public abstract class StorageViewBase : MonoBehaviour
    {

        public virtual bool IsAcceptedResource(ResourceType resourceType)
        {
            return true;
        }


        public abstract bool PutIntoStorage(ResourceView resource);


        public abstract float Capacity { get; }
        public abstract float Amount { get; }

        public bool IsFull => Amount >= Capacity;
        public bool IsEmpty => Amount <= 0;
    }
}