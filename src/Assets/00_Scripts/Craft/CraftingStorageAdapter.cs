using HBDinosaur_ER_Project.StorageSystem;

using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftingStorageAdapter
    {
        private readonly Storage inventory;
        private readonly Storage storage;

        public CraftingStorageAdapter(Storage inventory, Storage storage)
        {
            this.inventory = inventory;
            this.storage = storage;

            Debug.Log(
    $"[CraftingStorageAdapter] Created. " +
    $"inventory={(inventory != null ? "OK" : "NULL")}, " +
    $"storage={(storage != null ? "OK" : "NULL")}");
        }

        public int GetTotalItemCount(int itemId)
        {
            Debug.Log($"[CraftingStorageAdapter] GetTotalItemCount itemId={itemId}");

            int count = 0;

            if (inventory != null)
            {
                int inventoryCount = inventory.GetItemCount(itemId);
                Debug.Log($"[CraftingStorageAdapter] inventoryCount={inventoryCount}");
                count += inventoryCount;
            }
            else
            {
                Debug.LogWarning("[CraftingStorageAdapter] inventory is NULL.");
            }

            if (storage != null)
            {
                int storageCount = storage.GetItemCount(itemId);
                Debug.Log($"[CraftingStorageAdapter] storageCount={storageCount}");
                count += storageCount;
            }
            else
            {
                Debug.LogWarning("[CraftingStorageAdapter] storage is NULL.");
            }

            Debug.Log($"[CraftingStorageAdapter] totalCount={count}");
            return count;
        }
        public bool HasItem(int itemId, int amount)
        {
            return GetTotalItemCount(itemId) >= amount;
        }
        public bool RemoveItem(int itemId, int amount)
        {
            if (!HasItem(itemId, amount))
                return false;

            int remaining = amount;

            if (inventory != null)
                remaining -= inventory.RemoveItemUpTo(itemId, remaining);

            if (remaining > 0 && storage != null)
                remaining -= storage.RemoveItemUpTo(itemId, remaining);

            return remaining <= 0;
        }
        public bool AddItem(int itemId, int amount)
        {
            if (inventory != null && inventory.AddItem(itemId, amount))
                return true;

            if (storage != null && storage.AddItem(itemId, amount))
                return true;

            return false;
        }
    }

}
