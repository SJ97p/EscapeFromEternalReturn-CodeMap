using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class DragFollowIcon : MonoBehaviour
    {
        [SerializeField] private Image IconImage;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Canvas parentCanvas;


        public void Show(Sprite Icon, Vector2 screenPosition)
        {
            IconImage.sprite = Icon;
            gameObject.SetActive(true);
            Move(screenPosition);
        }

        public void Move(Vector2 screenPosition)
        {
            Camera cam = null;

            if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = parentCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPosition,
                cam,
                out Vector2 localPoint
            );

            rectTransform.localPosition = localPoint;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            IconImage.sprite = null;
        }
    }



}