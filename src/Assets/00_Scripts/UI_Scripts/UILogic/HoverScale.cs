using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HBDinosaur_ER_Project.UI
{
    public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Vector3 normalScale = Vector3.one;
        [SerializeField] private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1f);
        [SerializeField] private Vector3 pressedScale = new Vector3(0.98f, 0.98f, 1f);

        [SerializeField] private float hoverDuration = 0.12f;
        [SerializeField] private float pressDuration = 0.06f;
        [SerializeField] private Ease hoverEase = Ease.OutQuad;
        [SerializeField] private Ease pressEase = Ease.OutQuad;

        private Tween scaleTween;
        private bool isHovering;

        private void Awake()
        {
            normalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
            ScaleTo(hoverScale, hoverDuration, hoverEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            ScaleTo(normalScale, hoverDuration, hoverEase);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ScaleTo(pressedScale, pressDuration, pressEase);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScaleTo(isHovering ? hoverScale : normalScale, hoverDuration, hoverEase);
        }

        private void ScaleTo(Vector3 targetScale, float duration, Ease ease)
        {
            scaleTween?.Kill();
            scaleTween = transform
                .DOScale(targetScale, duration)
                .SetEase(ease)
                .SetUpdate(true);
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
            transform.localScale = normalScale;
            isHovering = false;
        }

        private void OnDestroy()
        {
            scaleTween?.Kill();
        }
    }
}