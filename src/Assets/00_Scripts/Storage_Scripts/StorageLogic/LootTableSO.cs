using System.Collections.Generic;
using HBDinosaur_ER_Project.ItemLogic;
using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    [CreateAssetMenu(
        fileName = "LootTable",
        menuName = "HBDinosaur/Loot/Loot Table"
    )]
    public class LootTableSO : ScriptableObject
    {
        [SerializeField] private ContainerType containerType;
        [SerializeField] private LootEntry[] entries;

        public ContainerType ContainerType => containerType;

        public LootEntry[] Roll()
        {
            List<LootEntry> drops = new();

            foreach (LootEntry entry in entries)
            {
                float roll = Random.Range(0f, 100f);

                if (roll > entry.DropChance)
                {
                    continue;
                }

                int amount = entry.GetRandomAmount();

                drops.Add(new LootEntry(entry.itemId, amount));
            }

            return drops.ToArray();
        }
    }
}
