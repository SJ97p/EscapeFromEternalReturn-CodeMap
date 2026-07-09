using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField] private EquipmentSlotData[] slots;

        public int SlotCount => slots.Length;

        public EquipmentSlotData GetSlot(int index)
        {
            if (index < 0 || index >= slots.Length) return null;
            return slots[index];
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < slots.Length;
        }
    }
}
