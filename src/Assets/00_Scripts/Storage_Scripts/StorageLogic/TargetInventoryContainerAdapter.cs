using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.InventoryView;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class TargetInventoryContainerAdapter : IItemContainer
    {
        private readonly Storage storage;
        private readonly TargetInventoryHud hud;

        public ItemContainerType ContainerType => ItemContainerType.Loot;
        public int Width => storage != null ? storage.Width : 0;
        public int Height => storage != null ? storage.Height : 0;

        public TargetInventoryContainerAdapter(Storage storage, TargetInventoryHud hud, int width, int height)
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

            var slot = storage.GetSlot(x, y);
            if (slot == null) return false;

            slot.ItemId = itemId;
            slot.Amount = amount;
            return true;
        }

        public bool ClearSlot(int x, int y)
        {
            if (!storage.IsValidPosition(x, y)) return false;

            var slot = storage.GetSlot(x, y);
            if (slot == null) return false;

            slot.ItemId = -1;
            slot.Amount = 0;
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
            return UIItemMoveManager.Instance.TryAutoMove(ContainerType, x, y);
        }

        public void RefreshUI()
        {
            hud.RefreshAll();
        }
    }
}