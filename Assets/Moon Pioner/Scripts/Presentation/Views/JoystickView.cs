using Services.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using VContainer;

namespace Presentation.Views
{
    public sealed class JoystickView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Components")]
        [SerializeField] private RectTransform containerRect;
        [SerializeField] private RectTransform secondaryRect;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private OnScreenStick stick;

        [Header("Settings")]
        [SerializeField] private float visualRange = 100f;
        [SerializeField] private float returnSmoothTime = 10f;
        [SerializeField] private float fadeSpeed = 5f;

        [Inject] private JoystickInputVariable _joystickInputVar;

        private bool _isPressed;
        private RectTransform _stickRect;

        private void Start()
        {
            if (containerRect == null || secondaryRect == null || canvasGroup == null || stick == null)
            {
                Debug.LogError("JoystickView: Missing component references.");
                enabled = false;
                return;
            }
            _stickRect = stick.GetComponent<RectTransform>();
            canvasGroup.alpha = 0;
        }

        private void Update()
        {
            UpdateVisuals(_joystickInputVar.Value);
        }

        private void UpdateVisuals(Vector2 input)
        {
            float targetAlpha = _isPressed ? 0.5f : 0f;
            if (!_isPressed && _stickRect != null)
            {
                if (_joystickInputVar.Value.magnitude > float.Epsilon)
                {
                    targetAlpha = 0.5f;
                    _stickRect.anchoredPosition = input * visualRange;
                }
                else
                {
                    _stickRect.anchoredPosition = Vector2.Lerp(
                        _stickRect.anchoredPosition,
                        _joystickInputVar.Value * visualRange,
                        Time.deltaTime * returnSmoothTime
                    );
                }
            }
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isPressed)
            {
                _isPressed = true;
                containerRect.position = eventData.position;
                secondaryRect.anchoredPosition = Vector2.zero;
                stick.OnPointerDown(eventData);
            }
        }
        public void OnPointerUp(PointerEventData eventData) => _isPressed = false;

        public void Show() => containerRect.gameObject.SetActive(true);
        public void Hide() => containerRect.gameObject.SetActive(false);
    }
}