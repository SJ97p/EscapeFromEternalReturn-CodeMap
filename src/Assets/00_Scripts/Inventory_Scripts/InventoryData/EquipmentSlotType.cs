using UnityEngine;

namespace HBDinosaur_ER_Project.ItemData
{
    // A 006 장비 아이템인지? 아닌지?, 장비라면 어떤 슬롯인지?
    public enum EquipmentSlotType
    {
        NONE,       // 장비가 아닌 아이템 (소모품 등)
        WEAPON,
        ARMOR,
        HELMET,
        ACCESSORY,
        SHOES
    }
}
