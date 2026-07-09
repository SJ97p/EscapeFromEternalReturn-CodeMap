using System;
using System.Collections.Generic;
using System.IO;
using HBDinosaur_ER_Project.ItemData;
using NUnit.Framework.Interfaces;
using SQLite;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class ItemRepository : IReadOnlyRepository<Item>
    {
        private readonly SQLiteConnection db;

        public ItemRepository(SQLiteConnection db)
        {
            this.db = db;
        }

        private Item MapFromRow(ItemDB dbRow)
        {
            ItemDataStruct data = new ItemDataStruct
            {
                itemName = dbRow.name,
                Id = dbRow.id,
				EquipmentSlotType = ToEquipmentSlotType(dbRow.type),
                Grade = ToItemGrade(dbRow.Grade),
                IsExtractable = dbRow.IsExtractable,
                IsSellable = dbRow.IsSellable,
                IsBaseFurniture = dbRow.isBaseFurniture,
                IsFavorite = dbRow.IsFavorite
            };

            return new Item(data, 1);
        }
        private EquipmentSlotType ToEquipmentSlotType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return EquipmentSlotType.NONE;


			switch (value.Trim().ToUpper())
            {
                case "NONE":
                case "NOT_EQUIP":
                case "NOTEQUIP":
                    return EquipmentSlotType.NONE;

                case "WEAPON":
                    return EquipmentSlotType.WEAPON;

                case "ARMOR":
                    return EquipmentSlotType.ARMOR;

                case "HELMET":
                    return EquipmentSlotType.HELMET;

                case "ACCESSORY":
                    return EquipmentSlotType.ACCESSORY;

                case "SHOES":
                    return EquipmentSlotType.SHOES;

                default:
                    Debug.LogWarning($"알 수 없는 EquipmentSlotType 문자열: {value}");
                    return EquipmentSlotType.NONE;
            }
        }
        private ItemGrade ToItemGrade(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return ItemGrade.WHITE;

            switch (value.Trim().ToUpper())
            {
                case "WHITE":
                    return ItemGrade.WHITE;

                case "GREEN":
                    return ItemGrade.GREEN;

                case "PURPLE":
                    return ItemGrade.PURPLE;

                case "ORANGE":
                    return ItemGrade.ORANGE;

                case "RED":
                    return ItemGrade.RED;

                default:
                    Debug.LogWarning($"알 수 없는 EquipmentSlotType 문자열: {value}");
                    return ItemGrade.WHITE;
            }
        }
        public IEnumerable<Item> GetAll()
        {
            var rows = db.Table<ItemDB>().ToList();

            List<Item> items = new();
            foreach (var row in rows)
            {
                items.Add(MapFromRow(row));
            }

            return items;
        }

        public Item GetById(int id)
        {
            var row = db.Table<ItemDB>().Where(x => x.id == id).FirstOrDefault();
            return row == null ? null : MapFromRow(row);
        }
    }

}
