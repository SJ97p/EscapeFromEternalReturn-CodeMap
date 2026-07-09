using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftingService
    {
        private readonly CraftingStorageAdapter storageAdapter;

        public CraftingService(CraftingStorageAdapter storageAdapter)
        {
            this.storageAdapter = storageAdapter;
        }

        public bool CanCraft(CraftTreeNode rootNode)
        {
            if (rootNode == null || storageAdapter == null)
                return false;

            Dictionary<int, int> requiredItems = GetRequiredItems(rootNode);

            foreach (var pair in requiredItems)
            {
                if (!storageAdapter.HasItem(pair.Key, pair.Value))
                    return false;
            }

            return true;
        }

        public bool TryCraft(CraftTreeNode rootNode)
        {
            if (!CanCraft(rootNode))
                return false;

            Dictionary<int, int> requiredItems = GetRequiredItems(rootNode);

            foreach (var pair in requiredItems)
            {
                if (!storageAdapter.RemoveItem(pair.Key, pair.Value))
                    return false;
            }

            return storageAdapter.AddItem(rootNode.ItemId, rootNode.NeedAmount);
        }

        private void CollectDirectRequirements(CraftTreeNode rootNode, Dictionary<int, int> result)
        {
            if (rootNode == null)
                return;

            AddRequirement(rootNode.Left, result);
            AddRequirement(rootNode.Right, result);
        }
        private void AddRequirement(CraftTreeNode node, Dictionary<int, int> result)
        {
            if (node == null)
                return;

            if (result.ContainsKey(node.ItemId))
                result[node.ItemId] += node.NeedAmount;
            else
                result.Add(node.ItemId, node.NeedAmount);
        }
        public Dictionary<int, int> GetRequiredItems(CraftTreeNode rootNode)
        {
            Dictionary<int, int> requiredItems = new();

            if (rootNode == null)
                return requiredItems;

            CollectDirectRequirements(rootNode, requiredItems);
            return requiredItems;
        }

        public Dictionary<int, int> GetMissingItems(CraftTreeNode rootNode)
        {
            Dictionary<int, int> missingItems = new();

            if (rootNode == null || storageAdapter == null)
                return missingItems;

            Dictionary<int, int> requiredItems = GetRequiredItems(rootNode);

            foreach (var pair in requiredItems)
            {
                int itemId = pair.Key;
                int requiredAmount = pair.Value;
                int ownedAmount = storageAdapter.GetTotalItemCount(itemId);

                if (ownedAmount < requiredAmount)
                    missingItems.Add(itemId, requiredAmount - ownedAmount);
            }

            return missingItems;
        }
        public int GetTotalItemCount(int itemId)
        {
            if (storageAdapter == null)
                return 0;

            return storageAdapter.GetTotalItemCount(itemId);
        }
    }
}
