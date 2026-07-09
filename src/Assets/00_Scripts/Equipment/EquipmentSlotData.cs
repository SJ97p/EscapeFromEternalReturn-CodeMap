using HBDinosaur_ER_Project.ItemData;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    [System.Serializable]
    public class EquipmentSlotData
    {
        [SerializeField] private EquipmentSlotType slotType;
        [SerializeField] private int itemId = -1;

        public EquipmentSlotType SlotType => slotType;

        public int ItemId
        {
            get => itemId;
            set => itemId = value;
        }

        public bool IsEmpty => itemId < 0;
        public void Clear()
        {
            itemId = -1;
        }
    }
}