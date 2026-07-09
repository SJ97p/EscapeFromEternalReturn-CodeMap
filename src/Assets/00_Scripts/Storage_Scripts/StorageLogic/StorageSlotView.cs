using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.UI.Slots;
using HBDinosaur_ER_Project.ItemData;
using TMPro;
using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class StorageSlotView : BaseItemSlotView
    {
        [SerializeField] private TMP_Text amountText;

        private StorageContainerAdapter containerAdapter;

        public void Init(int x, int y, StorageContainerAdapter adapter)
        {
            Initialize(x, y);
            containerAdapter = adapter;
        }

        protected override IItemContainer GetContainer() => containerAdapter;

        protected override bool HasItem()
        {
            return containerAdapter != null && !containerAdapter.IsEmpty(x, y);
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
            //Debug.Log($"[StorageSlotView] Refresh ({x},{y}) container={containerAdapter}");

            if (containerAdapter == null)
            {
                Debug.LogWarning($"[StorageSlotView] containerAdapter is null ({x},{y})");
                iconImage.sprite = null;
                iconImage.enabled = false;
                amountText.text = "";
                return;
            }

            int itemId = containerAdapter.GetItemId(x, y);
            int amount = containerAdapter.GetAmount(x, y);

            //Debug.Log($"[StorageSlotView] ({x},{y}) itemId={itemId}, amount={amount}, itemDb={ItemDatabase.Instance}");

            if (itemId < 0 || amount <= 0)
            {
                Debug.LogWarning($"[StorageSlotView] Empty slot ({x},{y}) itemId={itemId}, amount={amount}");
                iconImage.sprite = null;
                iconImage.enabled = false;
                amountText.text = "";
                return;
            }

            if (ItemDatabase.Instance == null)
            {
                Debug.LogError($"[StorageSlotView] ItemDatabase.Instance is null at ({x},{y})");
                return;
            }

            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            //Debug.Log($"[StorageSlotView] ({x},{y}) itemData={itemData}");

            if (itemData == null)
            {
                Debug.LogError($"[StorageSlotView] itemData is null for itemId={itemId} at ({x},{y})");
                iconImage.sprite = null;
                iconImage.enabled = false;
                amountText.text = "";
                return;
            }

            //Debug.Log($"[StorageSlotView] ({x},{y}) icon={itemData.Value.Icon}");

            iconImage.sprite = itemData.Value.Icon;
            iconImage.enabled = itemData.Value.Icon != null;
            amountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
}