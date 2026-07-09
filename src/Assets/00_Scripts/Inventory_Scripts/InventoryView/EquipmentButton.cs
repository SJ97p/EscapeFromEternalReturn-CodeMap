using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.InventoryLogic;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryView
{
    public class EquipmentButton : InventoryButton
    {
        [SerializeField] private EquipmentSlotType slotType;

        public override InventoryOwnerType OwnerType => InventoryOwnerType.EQUIPMENT_INVEN;
        public EquipmentSlotType SlotType => slotType;

        private Equipment2 equipment;

        public void Init(int slotIndex, Equipment2 eq)
        {
            SlotIndex = slotIndex;
            equipment = eq;
            inventoryUI = GetComponentInParent<InventoryUI>();
        }

        public override bool CanAcceptDrop(DragPayload payload)
        {
            if (payload.ownerType == InventoryOwnerType.TARGET_INVEN) return false;
            if (payload.item == null || !payload.item.data.IsEquippable) return false;
            return payload.item.data.EquipmentData.EquipmentSlotType == slotType;
        }

        public override void OnReceiveDrop(DragPayload payload)
        {
            if (!CanAcceptDrop(payload)) return;

            var playerInventory = inventoryUI.PlayerInventory; // ★ Instance 대신 inventoryUI 필드

            switch (payload.ownerType)
            {
                case InventoryOwnerType.PLAYER_INVEN:
                    if (CurrentItem != null)
                    {
                        var prev = equipment.Unequip(SlotIndex);
                        if (prev != null) playerInventory.AddItem(prev);
                    }
                    var toEquip = playerInventory.TakeItem(payload.slotIndex);
                    if (toEquip != null) equipment.Equip(toEquip, SlotIndex);
                    break;

                case InventoryOwnerType.EQUIPMENT_INVEN:
                    break;
            }
        }
    }
}