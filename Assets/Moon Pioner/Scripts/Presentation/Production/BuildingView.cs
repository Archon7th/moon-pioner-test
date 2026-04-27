using DG.Tweening;
using UnityEngine;

namespace Presentation.Production
{
    public sealed class BuildingView : MonoBehaviour
    {
        [SerializeField] private Transform pivot;

        private Sequence _bounceSequence;

        public void StartBounce(float duration)
        {
            if (_bounceSequence != null && _bounceSequence.IsActive())
                return;

            _bounceSequence = DOTween.Sequence();
            _bounceSequence.Append(pivot.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1f, vibrato: 4, elasticity: 1f)).SetLoops((int)duration - 1, LoopType.Yoyo);
            _bounceSequence.Append(pivot.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.5f));
            _bounceSequence.Append(pivot.DOScale(Vector3.one, 0.5f));
        }

        internal void StopBounce()
        {
            if (_bounceSequence != null && _bounceSequence.IsActive())
            {
                _bounceSequence.Kill();
                pivot.localScale = Vector3.one;
            }

        }
    }
}