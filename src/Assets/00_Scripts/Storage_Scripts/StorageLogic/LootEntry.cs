using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    [System.Serializable]
    public class LootEntry
    {
        public int itemId;

        [Range(0f, 100f)]
        public float DropChance;

        public int MinAmount = 1;
        public int MaxAmount = 1;

        public int amount;

        public LootEntry()
        {
        }

        public LootEntry(int itemId, int amount)
        {
            this.itemId = itemId;
            this.amount = amount;

            DropChance = 100f;
            MinAmount = amount;
            MaxAmount = amount;
        }

        public int GetRandomAmount()
        {
            return Random.Range(MinAmount, MaxAmount + 1);
        }
    }
}
