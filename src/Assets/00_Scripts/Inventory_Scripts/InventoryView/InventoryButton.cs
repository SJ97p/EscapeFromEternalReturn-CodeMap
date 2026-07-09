using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.StorageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.InventoryView
{
    [RequireComponent(typeof(Image))]
    public abstract class InventoryButton : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] protected Image IconImage;
        
        public Item CurrentItem { get; protected set; }
        public int SlotIndex { get; protected set; }
        public abstract InventoryOwnerType OwnerType { get; }

        protected DragFollowIcon dragFollowIcon;
        protected InventoryUI inventoryUI;

        private Color originColor;
        private Image image;


        protected virtual void Awake()
        {
            originColor = GetComponent<Image>().color;
            image = GetComponent<Image>();
            Image[] images = GetComponentsInChildren<Image>(includeInactive: true);
            IconImage = images.Length > 1 ? images[1] : null;
        }

        protected virtual void OnEnable()
        {
            dragFollowIcon = FindAnyObjectByType<DragFollowIcon>(FindObjectsInactive.Include);
            inventoryUI = GetComponentInParent<InventoryUI>();
        }

        public virtual void UpdateDisplay(Item item)
        {
            CurrentItem = item;
            

            if (item != null && item.data.Icon != null)
            {
                IconImage.sprite = item.data.Icon;
                var c = IconImage.color;
                c.a = 1f;
                IconImage.color = c;
                IconImage.enabled = true;
               
                image.color = item.data.Grade.ToColor(); // 아이템이 들어왔을 때
            }
            else
            {
                IconImage.sprite = null;
                var c = IconImage.color;
                c.a = 0f;
                IconImage.color = c;
                IconImage.enabled = false;

                image.color = originColor; // 아이템이 떠났을 때 인벤토리 기본 색상으로
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CurrentItem == null) return;
            dragFollowIcon.Show(CurrentItem.data.Icon, eventData.position);
            inventoryUI.RegisterDragStart(new DragPayload(CurrentItem, SlotIndex, OwnerType));

            IconImage.enabled = false; // 드래그를 시작할 떄 시작 위치 아이콘 비활성화
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CurrentItem == null) return;
            dragFollowIcon.Move(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragFollowIcon.Hide();
            inventoryUI.HandleDragEnd();

            if (CurrentItem != null)
                IconImage.enabled = true; // 드래그가 끝났을 때, 현재 위치의 아이템이 null이 아니라면 활성화
        }

        public void OnDrop(PointerEventData eventData)
        {
            inventoryUI.HandleDrop(this);
        }

        public abstract void OnReceiveDrop(DragPayload payload);
        public virtual bool CanAcceptDrop(DragPayload payload) => true;
    }
}