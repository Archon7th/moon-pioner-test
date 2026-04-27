namespace Presentation.Production
{
    using UnityEngine;
    using TMPro;
    using DG.Tweening;
    
    public class FloatingMessageView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform messageRect;
        [SerializeField] private TMP_Text messageText;

        private Sequence _bounceSequence;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
        }

        public void ShowBounceMessage(string message)
        {
            if (_bounceSequence != null && _bounceSequence.IsActive())
                _bounceSequence.Kill();

            canvasGroup.alpha = canvasGroup.alpha * 0.25f;
            messageRect.localScale = messageRect.localScale * 0.25f;
            messageText.text = message;

            _bounceSequence = DOTween.Sequence();

            _bounceSequence.Append(canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.Linear));
            _bounceSequence.Join(messageRect.DOScale(1f, 0.6f).SetEase(Ease.OutBack));

            _bounceSequence.Append(canvasGroup.DOFade(0f, 3f).SetEase(Ease.InExpo));
            if (transform.parent && Camera.main != null)
            {
                Camera camera = Camera.main;
                messageRect.position = camera.WorldToScreenPoint(transform.parent.position + Vector3.up);
                _bounceSequence.SetUpdate(UpdateType.Late);
                _bounceSequence.OnUpdate(() => {
                    messageRect.position = camera.WorldToScreenPoint(transform.parent.position + Vector3.up);
                });
            }

                
        }
    }
}