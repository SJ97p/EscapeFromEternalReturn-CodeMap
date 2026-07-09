# IItemContainer

## Role

Inventory, Storage, Equipment, Loot UI를 같은 아이템 이동 시스템에 연결하기 위한 공통 컨테이너 인터페이스입니다.

## Class Diagram

```mermaid
classDiagram
    class IItemContainer {
        <<interface>>
        +ItemContainerType ContainerType
        +int Width
        +int Height
        +bool IsValidSlot(int x, int y)
        +bool IsEmpty(int x, int y)
        +int GetItemId(int x, int y)
        +int GetAmount(int x, int y)
        +bool SetSlot(int x, int y, int itemId, int amount)
        +bool ClearSlot(int x, int y)
        +bool CanDrop(UIDragPayload payload, int toX, int toY)
        +bool HandleDrop(UIDragPayload payload, int toX, int toY)
        +bool HandleClick(int x, int y)
        +void RefreshUI()
    }

    class InventoryContainerAdapter
    class StorageContainerAdapter
    class TargetInventoryContainerAdapter
    class EquipmentAdapter

    IItemContainer <|.. InventoryContainerAdapter
    IItemContainer <|.. StorageContainerAdapter
    IItemContainer <|.. TargetInventoryContainerAdapter
    IItemContainer <|.. EquipmentAdapter
```

## Design Point

각 UI 창의 내부 구조는 다르지만, 이동 시스템은 `IItemContainer`만 보고 슬롯 조회, 검증, 설정, UI 갱신을 처리합니다. 이 구조가 Adapter Pattern의 핵심입니다.

## Source

- [IItemContainer.cs](../../src/Assets/00_Scripts/Storage_Scripts/StorageLogic/IItemContainer.cs)

