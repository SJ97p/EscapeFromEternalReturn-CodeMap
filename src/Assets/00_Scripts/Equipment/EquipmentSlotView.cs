using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.UI.Slots;
using HBDinosaur_ER_Project.ItemData;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class EquipmentSlotView : BaseItemSlotView
    {
        private EquipmentAdapter containerAdapter;

        public void Init(int x, int y, EquipmentAdapter adapter)
        {
            Initialize(x, y);
            containerAdapter = adapter;
            Refresh();
        }

        protected override IItemContainer GetContainer()
        {
            return containerAdapter;
        }

        protected override bool HasItem()
        {
            if (containerAdapter == null) return false;
            return !containerAdapter.IsEmpty(x, y);
        }

        protected override Sprite GetSlotIcon()
        {
            if (containerAdapter == null) return null;

            int itemId = containerAdapter.GetItemId(x, y);
            if (itemId < 0) return null;

            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (itemData == null) return null;

            return itemData.Value.Icon;
        }

        public override void Refresh()
        {
            if (containerAdapter == null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                iconImage.color = Color.white;
                return;
            }

            int itemId = containerAdapter.GetItemId(x, y);
            if (itemId < 0)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                iconImage.color = Color.white;
                return;
            }

            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (itemData == null || itemData.Value.Icon == null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                iconImage.color = Color.white;
                return;
            }

            iconImage.sprite = itemData.Value.Icon;
            iconImage.enabled = true;
            iconImage.color = Color.white;
        }
    }
}