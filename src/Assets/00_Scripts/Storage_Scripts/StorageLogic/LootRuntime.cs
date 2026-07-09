using System;
using UnityEngine;
using HBDinosaur_ER_Project.ItemLogic;
using HBDinosaur_ER_Project.ItemData;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class LootRuntime : MonoBehaviour
    {
        [SerializeField] private LootEntry[] lootEntries;
        private LootTableSO[] lootTables;

        public void InitializeLoot(ContainerType containerType)
        {
            lootTables = ItemDatabase.Instance.lootTables;

            LootTableSO lootTable = Array.Find(
                lootTables,
                table => table.ContainerType == containerType
            );

            if (lootTable == null)
            {
                lootEntries = Array.Empty<LootEntry>();
                return;
            }

            lootEntries = lootTable.Roll();
        }

        public LootEntry[] GetLootEntries()
        {
            return lootEntries;
        }

        public LootEntry[] GetLootEntries(ContainerType containerType)
        {
            InitializeLoot(containerType);
            return lootEntries;
        }
    }
}
