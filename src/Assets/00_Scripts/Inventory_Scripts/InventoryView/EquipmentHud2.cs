using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.InventoryView;
using HBDinosaur_ER_Project.ItemData;
using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryView
{
    public class EquipmentHud2 : MonoBehaviour
    {
        // 인스펙터에서 5개 슬롯 버튼 연결 (순서: Weapon, Armor, Helmet, Accessory, Shoes)
        [SerializeField] private EquipmentButton[] slotButtons; // 5개

        private Equipment2 equipment;

        public void Init(Equipment2 equipment)
        {
            this.equipment = equipment;

            for (int i = 0; i < slotButtons.Length; i++)
                slotButtons[i].Init(i, equipment);

            InventoryEventBus.Instance.OnEquipmentChanged += OnEquipmentChanged;

            Refresh();
        }

        private void OnDestroy()
        {
            InventoryEventBus.Instance.OnEquipmentChanged -= OnEquipmentChanged;
        }

        private void OnEquipmentChanged(int slotIndex)
        {
            slotButtons[slotIndex].UpdateDisplay(equipment.items[slotIndex]);
        }

        private void Refresh()
        {
            for (int i = 0; i < slotButtons.Length; i++)
                slotButtons[i].UpdateDisplay(equipment.items[i]);
        }
    }
}