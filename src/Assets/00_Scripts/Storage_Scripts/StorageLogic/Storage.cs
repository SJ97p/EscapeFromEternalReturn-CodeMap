using System.Collections.Generic;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.ItemData;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class Storage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private StorageSlot[,] slots;

        public StorageSlot[,] Slots => slots;

        public Storage(int width, int height)
        {
            Width = width;
            Height = height;

            slots = new StorageSlot[width, height];
            InitializeSlots();
        }
        private void InitializeSlots()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    slots[x, y] = new StorageSlot
                    {
                        X = x,
                        Y = y,
                        ItemId = -1,
                        Amount = 0
                    };
                }
            }
        }

        public void LoadFromDB(List<StorageData> rows)
        {
            //LoadFromDB(GameDataStore.Instance.LoadStorage());
            foreach (var row in rows)
            {
                if (!IsValidPosition(row.X, row.Y))
                    continue;

                slots[row.X, row.Y].SetItem(row.ItemId, row.Quantity);
            }
        }
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public StorageSlot GetSlot(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return null;

            return slots[x, y];
        }
        public void Swap(int fromX, int fromY, int toX, int toY)
        {
            if (!IsValidPosition(fromX, fromY) || !IsValidPosition(toX, toY))
                return;

            StorageSlot a = slots[fromX, fromY];
            StorageSlot b = slots[toX, toY];

            int tempItemId = a.ItemId;
            int tempAmount = a.Amount;

            a.SetItem(b.ItemId, b.Amount);
            b.SetItem(tempItemId, tempAmount);
        }
        public void SetItem(int x, int y, int itemId, int amount)
        {
            if (!IsValidPosition(x, y))
                return;

            slots[x, y].SetItem(itemId, amount);
        }
        public void ClearSlot(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return;

            slots[x, y].Clear();
        }
        public int GetItemCount(int itemId)
        {
            int count = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    StorageSlot slot = slots[x, y];

                    if (slot.ItemId == itemId)
                    {
                        Debug.Log($"[Storage] Found itemId={itemId} at ({x},{y}), amount={slot.Amount}");
                        count += slot.Amount;
                    }
                }
            }

            Debug.Log($"[Storage] GetItemCount itemId={itemId}, total={count}");
            return count;
        }
        public bool HasItem(int itemId, int amount)
        {
            if (amount <= 0)
                return true;

            return GetItemCount(itemId) >= amount;
        }
        public int RemoveItemUpTo(int itemId, int amount)
        {
            if (itemId < 0 || amount <= 0)
                return 0;

            int remaining = amount;
            int removed = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (remaining <= 0)
                        return removed;

                    StorageSlot slot = slots[x, y];

                    if (slot.ItemId != itemId || slot.Amount <= 0)
                        continue;

                    int removeAmount = Mathf.Min(slot.Amount, remaining);
                    int nextAmount = slot.Amount - removeAmount;

                    if (nextAmount <= 0)
                        slot.Clear();
                    else
                        slot.SetItem(itemId, nextAmount);

                    remaining -= removeAmount;
                    removed += removeAmount;
                }
            }

            return removed;
        }
        public bool RemoveItem(int itemId, int amount)
        {
            if (!HasItem(itemId, amount))
                return false;

            return RemoveItemUpTo(itemId, amount) == amount;
        }

        public bool AddItem(int itemId, int amount)
        {
            if (itemId < 0 || amount <= 0)
                return false;

            ItemDataStruct? data = ItemDatabase.Instance.GetItemByID(itemId);
            if (!data.HasValue)
                return false;

            int maxStack = Mathf.Max(1, data.Value.MaxStack);
            int remaining = amount;

            // 1. 기존 같은 아이템 스택부터 채우기
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    StorageSlot slot = slots[x, y];

                    if (slot.ItemId != itemId)
                        continue;

                    if (slot.Amount >= maxStack)
                        continue;

                    int addAmount = Mathf.Min(maxStack - slot.Amount, remaining);
                    slot.SetItem(itemId, slot.Amount + addAmount);

                    remaining -= addAmount;

                    if (remaining <= 0)
                        return true;
                }
            }

            // 2. 빈 슬롯에 새로 넣기
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    StorageSlot slot = slots[x, y];

                    if (slot.ItemId != -1 || slot.Amount > 0)
                        continue;

                    int addAmount = Mathf.Min(maxStack, remaining);
                    slot.SetItem(itemId, addAmount);

                    remaining -= addAmount;

                    if (remaining <= 0)
                        return true;
                }
            }

            return false;
        }
        public List<StorageData> ExportToStorageData(StorageType storageType, int saveId)
        {
            List<StorageData> exportedData = new List<StorageData>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    StorageSlot slot = slots[x, y];

                    //  중요: 아이템이 들어있는 유효한 슬롯만 추출합니다.
                    if (slot != null && !slot.IsEmpty)
                    {
                        // StorageData 생성자의 시그니처에 맞게 객체를 생성합니다.
                        // (현재 가지고 계신 StorageData 생성자 구조에 맞춰 변수명을 대입하세요)
                        StorageData data = new StorageData(
                            (int)storageType, // 본인의 타입 (Inventory, Storage, EquipmentStorage 등)
                            saveId,      // 현재 저장할 세이브 슬롯 번호
                            slot.ItemId, // 아이템 ID
                            slot.Amount, // 수량
                            x,           // X 좌표
                            y            // Y 좌표
                        );

                        exportedData.Add(data);
                    }
                }
            }

            return exportedData;
        }

    }
}