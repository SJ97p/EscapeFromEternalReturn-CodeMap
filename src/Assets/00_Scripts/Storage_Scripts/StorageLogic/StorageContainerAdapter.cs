using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.InventoryRewrite;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class StorageContainerAdapter : IItemContainer
    {
        private readonly Storage storage;
        private readonly StoragePanelUI panelUI;

        public ItemContainerType ContainerType => ItemContainerType.Storage;
        public int Width => storage != null ? storage.Width : 0;
        public int Height => storage != null ? storage.Height : 0;

        public StorageContainerAdapter(Storage storage, StoragePanelUI panelUI)
        {
            this.storage = storage;
            this.panelUI = panelUI;
        }

        public bool IsValidSlot(int x, int y) => storage.IsValidPosition(x, y);

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
            return UIItemMoveManager.Instance.CanMove(payload.ContainerType, payload.X, payload.Y, ContainerType, toX, toY);
        }

        public bool HandleDrop(UIDragPayload payload, int toX, int toY)
        {
            if (!storage.IsValidPosition(toX, toY)) return false;
            return UIItemMoveManager.Instance.TryMove(payload.ContainerType, payload.X, payload.Y, ContainerType, toX, toY);
        }

        public void RefreshUI()
        {
            panelUI.RefreshAll();
        }

        public bool HandleClick(int x, int y)
        {
            return UIItemMoveManager.Instance.TryAutoMove(ContainerType, x, y);
        }
    }
}