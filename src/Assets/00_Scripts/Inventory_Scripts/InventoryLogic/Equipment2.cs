using HBDinosaur_ER_Project.InventoryData;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.ItemData;
using UnityEngine;
namespace HBDinosaur_ER_Project.InventoryLogic
{
    public class Equipment2 : IInteractable
    {
        // 슬롯 순서: 0=Weapon, 1=Armor, 2=Helmet, 3=Accessory, 4=Shoes
        public Item[] items { get; private set; }

        private static readonly EquipmentSlotType[] SlotTypes = new[]
        {
        EquipmentSlotType.WEAPON,
        EquipmentSlotType.ARMOR,
        EquipmentSlotType.HELMET,
        EquipmentSlotType.ACCESSORY,
        EquipmentSlotType.SHOES
        };

        public Equipment2()
        {
            items = new Item[SlotTypes.Length];
        }

        public EquipmentSlotType GetSlotType(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotTypes.Length) return EquipmentSlotType.NONE;
            return SlotTypes[slotIndex];
        }

        // 해당 부위의 슬롯 인덱스 반환
        public int GetSlotIndex(EquipmentSlotType slotType)
        {
            for (int i = 0; i < SlotTypes.Length; i++)
                if (SlotTypes[i] == slotType) return i;
            return -1;
        }

        public bool Equip(Item item, int slotIndex)
        {
            if (!CanEquip(item, slotIndex)) return false;

            items[slotIndex] = item;
            InventoryEventBus.Instance.PublishSlotChanged(null, slotIndex); // Equipment는 별도 이벤트
            InventoryEventBus.Instance.PublishEquipmentChanged(slotIndex);
            return true;
        }

        public Item Unequip(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Length || items[slotIndex] == null) return null;

            var unequipped = items[slotIndex];
            items[slotIndex] = null;
            InventoryEventBus.Instance.PublishEquipmentChanged(slotIndex);
            return unequipped;
        }

        public bool CanEquip(Item item, int slotIndex)
        {
            if (item == null) return false;
            if (slotIndex < 0 || slotIndex >= items.Length) return false;
            return item.data.EquipmentSlotType == SlotTypes[slotIndex];
        }

        public bool CanInteract(InteractContext context)
        {
            return context.interactType switch
            {
                InventoryInteractType.EQUIPITEM => CanEquip(context.item, context.toSlotIndex),
                InventoryInteractType.UNEQUIPITEM => context.fromSlotIndex >= 0 && context.fromSlotIndex < items.Length && items[context.fromSlotIndex] != null,
                _ => false
            };
        }

        public void Interact(InteractContext context)
        {
            if (!CanInteract(context)) return;

            switch (context.interactType)
            {
                case InventoryInteractType.EQUIPITEM:
                    Equip(context.item, context.toSlotIndex);
                    break;
                case InventoryInteractType.UNEQUIPITEM:
                    Unequip(context.fromSlotIndex);
                    break;
            }

            InventoryEventBus.Instance.PublishInteracted(context.interactType);
        }
    }
}

