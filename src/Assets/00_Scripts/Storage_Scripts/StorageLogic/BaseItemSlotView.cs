using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI.DragDrop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.UI.Slots
{
    public abstract class BaseItemSlotView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
    {
        [SerializeField] protected Image iconImage;

        protected int x;
        protected int y;

        private bool wasDragging;

        private UIDragDropManager dragDropManager;

        protected virtual void Awake()
        {
            dragDropManager = GetComponentInParent<Canvas>().GetComponentInChildren<UIDragDropManager>(true);

            if (dragDropManager == null)
            {
                Debug.LogError($"[{nameof(BaseItemSlotView)}] UIDragDropManager not found in parents.");
            }
        }

        public void Initialize(int x, int y)
        {
            this.x = x;
            this.y = y;

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        protected abstract IItemContainer GetContainer();
        protected abstract Sprite GetSlotIcon();
        protected abstract bool HasItem();

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (dragDropManager == null)
                return;

            var container = GetContainer();

            if (container == null || !HasItem())
                return;

            wasDragging = true;

            UIDragPayload payload = new UIDragPayload(
                container.ContainerType,
                x,
                y,
                container.GetItemId(x, y),
                container.GetAmount(x, y)
            );

            dragDropManager.BeginDrag(
                container,
                payload,
                GetSlotIcon(),
                eventData.position
            );

            if (iconImage != null)
                iconImage.enabled = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (dragDropManager == null)
                return;

            if (!dragDropManager.IsDragging)
                return;

            dragDropManager.UpdateDrag(eventData.position);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (dragDropManager == null)
                return;

            if (iconImage != null)
                iconImage.enabled = true;

            dragDropManager.EndDrag();

            GetContainer()?.RefreshUI();
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            Debug.Log("Drop");
            if (dragDropManager == null)
                return;

            if (!dragDropManager.IsDragging)
                return;

            var targetContainer = GetContainer();

            if (targetContainer == null)
                return;

            UIDragPayload payload = dragDropManager.CurrentPayload;

            if (targetContainer.CanDrop(payload, x, y))
            {
                bool success = targetContainer.HandleDrop(payload, x, y);

                if (success)
                {
                    dragDropManager.SourceContainer?.RefreshUI();
                    targetContainer.RefreshUI();
                }
            }

            dragDropManager.EndDrag();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (wasDragging)
            {
                wasDragging = false;
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!HasItem())
                return;

            var container = GetContainer();

            if (container == null)
                return;

            bool moved = container.HandleClick(x, y);

            if (moved)
                container.RefreshUI();
        }

        public abstract void Refresh();
    }
}