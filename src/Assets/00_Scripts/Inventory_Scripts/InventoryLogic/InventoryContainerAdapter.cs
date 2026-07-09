using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.UI.DragDrop;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class InventoryContainerAdapter : IItemContainer
    {
        private readonly PlayerInventoryHud hud;
        private readonly Storage storage;

        public ItemContainerType ContainerType => ItemContainerType.Inventory;
        public int Width => storage != null ? storage.Width : 0;
        public int Height => storage != null ? storage.Height : 0;

        public InventoryContainerAdapter(Storage storage, PlayerInventoryHud hud)
        {
            this.storage = storage;
            this.hud = hud;
        }

        public bool IsValidSlot(int x, int y)
        {
            return storage != null && storage.IsValidPosition(x, y);
        }

        public bool IsEmpty(int x, int y)
        {
            var slot = storage.GetSlot(x, y);
            return slot == null || slot.IsEmpty;
        }

        public int GetItemId(int x, int y)
        {
            var slot = storage.GetSlot(x, y);
            if (slot == null || slot.IsEmpty) return -1;
            return slot.ItemId;
        }

        public int GetAmount(int x, int y)
        {
            var slot = storage.GetSlot(x, y);
            if (slot == null || slot.IsEmpty) return 0;
            return slot.Amount;
        }

        public bool SetSlot(int x, int y, int itemId, int amount)
        {
            if (!storage.IsValidPosition(x, y)) return false;

            storage.SetItem(x, y, itemId, amount);
            return true;
        }

        public bool ClearSlot(int x, int y)
        {
            if (!storage.IsValidPosition(x, y)) return false;

            storage.ClearSlot(x, y);
            return true;
        }

        public bool CanDrop(UIDragPayload payload, int toX, int toY)
        {
            if (!storage.IsValidPosition(toX, toY)) return false;

            return UIItemMoveManager.Instance.CanMove(
                payload.ContainerType, payload.X, payload.Y,
                ContainerType, toX, toY);
        }

        public bool HandleDrop(UIDragPayload payload, int toX, int toY)
        {
            if (!storage.IsValidPosition(toX, toY)) return false;

            return UIItemMoveManager.Instance.TryMove(
                payload.ContainerType, payload.X, payload.Y,
                ContainerType, toX, toY);
        }

        public bool HandleClick(int x, int y)
        {
            if (IsConsumable(x, y) && UIItemUseManager.Instance.TryUse(ContainerType, x, y))
                return true;

            return UIItemMoveManager.Instance.TryAutoMove(ContainerType, x, y);
        }

        private bool IsConsumable(int x, int y)
        {
            if (storage == null || !storage.IsValidPosition(x, y))
                return false;

            var slot = storage.GetSlot(x, y);
            if (slot == null || slot.IsEmpty)
                return false;

            var itemData = ItemDatabase.Instance.GetItemByID(slot.ItemId);
            return itemData.HasValue && itemData.Value.ItemType == ItemType.Consumable;
        }

        public void RefreshUI()
        {
            hud.RefreshAll();
        }
    }
}
