using DG.Tweening;
using System;
using UnityEngine;
using VContainer;

namespace Presentation.Production
{
    public class ResourceView : MonoBehaviour, IPooledItem
    {
        ResourceFactory _factory;

        [SerializeField] private ResourceType _type;

        [Inject]
        public void Construct(ResourceFactory factory)
        {
            _factory = factory;
        }

        public ResourceType ResourceType => _type;

        public void Awake()
        {
            var placePoint = transform.parent?.GetComponent<BuildingStoragePoint>();
            if (placePoint != null)
            {
                placePoint.BeforeStore(this);
                placePoint.CompleteStore();
            }
        }

        public void Release()
        {
            transform.SetParent(null);
            _factory.Return(_type, this);
        }

        private Sequence _moveSequence;

        public void PlayMoveAnimation(Action callback = null)
        {
            _moveSequence?.Kill();
            _moveSequence = DOTween.Sequence()
                .Append(transform.DOLocalJump(Vector3.zero, 1, 1, 1))
                .OnComplete(() =>
                {
                    if (callback != null)
                        callback?.Invoke();
                });
        }
    }
}