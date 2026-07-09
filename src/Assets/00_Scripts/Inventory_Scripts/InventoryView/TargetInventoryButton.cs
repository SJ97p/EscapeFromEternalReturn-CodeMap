using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.InventoryView;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.InventoryLogic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HBDinosaur_ER_Project.InventoryView
{
    public class TargetInventoryButton : InventoryButton, IPointerClickHandler
    {
        public override InventoryOwnerType OwnerType => InventoryOwnerType.TARGET_INVEN;

        private Inventory targetInventory;

        public void Init(int slotIndex, Inventory inventory)
        {
            SlotIndex = slotIndex;
            targetInventory = inventory;
            inventoryUI = GetComponentInParent<InventoryUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (CurrentItem == null) return;

            var playerInventory = inventoryUI.PlayerInventory; // Instance 대신 inventoryUI 필드
            if (playerInventory == null) return;

            var taken = targetInventory.TakeItem(SlotIndex);
            if (taken == null) return;

            bool added = playerInventory.AddItem(taken);
            if (!added)
            {
                targetInventory.items[SlotIndex] = taken;
                InventoryEventBus.Instance.PublishSlotChanged(targetInventory, SlotIndex);
                Debug.Log("플레이어 인벤토리가 가득 찼습니다.");
            }
        }

        public override bool CanAcceptDrop(DragPayload payload) => false;

        public override void OnReceiveDrop(DragPayload payload) { }
    }
}