# Storage

## Role

Inventory, Storage, Equipment, Loot이 공유하는 런타임 슬롯 데이터 모델입니다.

## Class Diagram

```mermaid
classDiagram
    class Storage {
        +int Width
        +int Height
        -StorageSlot[,] slots
        +StorageSlot[,] Slots
        +Storage(int width, int height)
        -void InitializeSlots()
        +void LoadFromDB(List~StorageData~ rows)
        +bool IsValidPosition(int x, int y)
        +StorageSlot GetSlot(int x, int y)
        +void Swap(int fromX, int fromY, int toX, int toY)
        +void SetItem(int x, int y, int itemId, int amount)
        +void ClearSlot(int x, int y)
        +int GetItemCount(int itemId)
        +bool HasItem(int itemId, int amount)
        +int RemoveItemUpTo(int itemId, int amount)
        +bool RemoveItem(int itemId, int amount)
        +bool AddItem(int itemId, int amount)
        +List~StorageData~ ExportToStorageData(StorageType storageType, int saveId)
    }

    class StorageSlot {
        +int X
        +int Y
        +int ItemId
        +int Amount
        +bool IsEmpty
        +void SetItem(int itemId, int amount)
        +void Clear()
    }

    class StorageData {
        +int SaveId
        +StorageType StorageType
        +int ItemId
        +int Quantity
        +int X
        +int Y
    }

    Storage *-- StorageSlot
    Storage --> StorageData : export
```

## Design Point

런타임에서는 2D 슬롯 배열로 빠르게 접근하고, 저장 시점에는 비어있지 않은 슬롯만 `StorageData`로 변환합니다.

## Source

- [Storage.cs](../../src/Assets/00_Scripts/Storage_Scripts/StorageLogic/Storage.cs)

