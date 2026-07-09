using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.UI.Slots;
using HBDinosaur_ER_Project.InventoryView;
using HBDinosaur_ER_Project.ItemData;
using TMPro;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class InventorySlotView : BaseItemSlotView
    {
        [SerializeField] private TMP_Text amountText;

        private PlayerInventoryHud hud;
        private InventoryContainerAdapter containerAdapter;

        protected override void OnInitialized()
        {
            hud = GetComponentInParent<PlayerInventoryHud>();
            containerAdapter = hud.ContainerAdapter;
        }

        protected override IItemContainer GetContainer() => containerAdapter;

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
                amountText.text = "";
                return;
            }

            int itemId = containerAdapter.GetItemId(x, y);
            int amount = containerAdapter.GetAmount(x, y);
            //Debug.Log($"{name} Refresh x={x}, y={y}, itemId={itemId}, amount={amount}");

            if (itemId < 0 || amount <= 0)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                amountText.text = "";
                return;
            }

            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (itemData == null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                amountText.text = "";
                return;
            }

            iconImage.sprite = itemData.Value.Icon;
            iconImage.enabled = itemData.Value.Icon != null;
            amountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
}