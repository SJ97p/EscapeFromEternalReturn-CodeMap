using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.InventoryLogic;
using UnityEngine.EventSystems;
using HBDinosaur_ER_Project.UI;

namespace HBDinosaur_ER_Project.InventoryView
{
    public class PlayerInventoryButton : InventoryButton/*, IPointerClickHandler*/
    {
        public override InventoryOwnerType OwnerType => InventoryOwnerType.PLAYER_INVEN;

        private Inventory playerInventory;

        public void Init(int slotIndex, Inventory inventory)
        {
            SlotIndex = slotIndex;
            playerInventory = inventory;
            inventoryUI = GetComponentInParent<InventoryUI>();
        }

        public override bool CanAcceptDrop(DragPayload payload) => true;

        public override void OnReceiveDrop(DragPayload payload)
        {
            //switch (payload.ownerType)
            //{
            //    case InventoryOwnerType.PLAYER_INVEN:
            //        playerInventory.SwapItems(payload.slotIndex, SlotIndex);
            //        break;

            //    case InventoryOwnerType.EQUIPMENT_INVEN:
            //        var equipment = inventoryUI.Equipment; // ★ Instance 대신 inventoryUI 필드
            //        var unequipped = equipment.Unequip(payload.slotIndex);
            //        if (unequipped == null) return;

            //        if (CurrentItem != null && CurrentItem.data.IsEquippable &&
            //            CurrentItem.data.EquipmentSlotType == equipment.GetSlotType(payload.slotIndex))
            //        {
            //            var toEquip = playerInventory.TakeItem(SlotIndex);
            //            playerInventory.items[SlotIndex] = unequipped;
            //            InventoryEventBus.Instance.PublishSlotChanged(playerInventory, SlotIndex);
            //            equipment.Equip(toEquip, payload.slotIndex);
            //        }
            //        else
            //        {
            //            if (CurrentItem != null)
            //                playerInventory.AddItem(CurrentItem);
            //            playerInventory.items[SlotIndex] = unequipped;
            //            InventoryEventBus.Instance.PublishSlotChanged(playerInventory, SlotIndex);
            //        }
            //        break;

            //    case InventoryOwnerType.TARGET_INVEN:
            //        var targetInventory = UIManager.Instance.TargetInventory;
            //        var taken = targetInventory.TakeItem(payload.slotIndex);
            //        if (taken == null) return;

            //        if (CurrentItem == null)
            //        {
            //            playerInventory.items[SlotIndex] = taken;
            //            InventoryEventBus.Instance.PublishSlotChanged(playerInventory, SlotIndex);
            //        }
            //        else
            //        {
            //            playerInventory.AddItem(taken);
            //        }
            //        break;
            //}
        }

        //public void OnPointerClick(PointerEventData eventData) // 클릭했을 때 장비아이템 장비칸으로 옮기기
        //{

        //    var equipment = inventoryUI.Equipment;
        //    var playerInventory = inventoryUI.PlayerInventory;

        //    if (eventData.button != PointerEventData.InputButton.Right) return; // 우클릭이면 무시
        //    if (CurrentItem == null) return; // 아이템이 없으면 무시
        //    if (!CurrentItem.data.IsEquippable) return; // 장착 가능 아이템이 아니면 무시

        //    // 장착할 장비의 인덱스 번호 반환
        //    int slotIndex = equipment.GetSlotIndex(CurrentItem.data.EquipmentSlotType);
        //    if (slotIndex == -1) return;

        //    // 장착된 장비가 있다면 탈착
        //    var alreadyEquipped = equipment.Unequip(slotIndex);
        //    if (alreadyEquipped != null) playerInventory.AddItem(alreadyEquipped);

        //    // 장착할 장비를 장착
        //    var taken = playerInventory.TakeItem(SlotIndex);
        //    if (taken != null) equipment.Equip(taken, slotIndex);
        //}
    }
}