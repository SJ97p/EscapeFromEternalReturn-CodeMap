using System.Collections.Generic;
using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.Player;
using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI.DragDrop;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class UIItemUseManager : Singleton<UIItemUseManager>
    {
        public bool TryUse(ItemContainerType containerType, int x, int y)
        {

            Debug.Log("1");
            if (containerType != ItemContainerType.Inventory)
                return false;

            var container = UIItemMoveManager.Instance.GetContainer(containerType);
            if (container == null || container.IsEmpty(x, y))
                return false;

            int itemId = container.GetItemId(x, y);
            var itemData = ItemDatabase.Instance.GetItemByID(itemId);

            if (!itemData.HasValue)
                return false;

            if (itemData.Value.ItemType != ItemType.Consumable)
                return false;

            Debug.Log("2");
            var consumable = itemData.Value.ConsumableData;

            // ¿¹: HP È¸º¹
            if (PlayerStat.Instance.HP >= PlayerStat.Instance.MaxHP)
                return false;

            PlayerStat.Instance.HP += consumable.HealAmount;

            int amount = container.GetAmount(x, y);
            if (amount <= 1)
                container.ClearSlot(x, y);
            else
                container.SetSlot(x, y, itemId, amount - 1);

            container.RefreshUI();
            return true;
        }
    }
}

