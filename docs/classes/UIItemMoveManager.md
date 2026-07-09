# UIItemMoveManager

## Role

서로 다른 아이템 컨테이너 사이의 이동, 병합, 스왑, 자동 이동을 중앙에서 조율합니다.

## Class Diagram

```mermaid
classDiagram
    class UIItemMoveManager {
        -Dictionary~ItemContainerType, IItemContainer~ containers
        -HashSet~ItemContainerType~ activeUIStates
        +void RegisterContainer(IItemContainer container)
        +void UnregisterContainer(ItemContainerType containerType)
        +void SetUIActive(ItemContainerType type, bool isActive)
        +IItemContainer GetContainer(ItemContainerType type)
        +bool CanMove(ItemContainerType fromType, int fromX, int fromY, ItemContainerType toType, int toX, int toY)
        +bool TryMove(ItemContainerType fromType, int fromX, int fromY, ItemContainerType toType, int toX, int toY)
        +bool TryAutoMove(ItemContainerType fromType, int fromX, int fromY)
        +bool IsUIOpen(ItemContainerType type)
        -int GetMaxStack(int itemId)
        -bool IsStackable(int itemId)
        -bool CanMerge(IItemContainer from, int fromX, int fromY, IItemContainer to, int toX, int toY)
        -bool TryMerge(IItemContainer from, int fromX, int fromY, IItemContainer to, int toX, int toY)
        -bool IsEquippableItem(int itemId)
    }

    UIItemMoveManager --> IItemContainer : register / query
    UIItemMoveManager --> ItemDatabase : stack / equip data
```

## Transaction Steps

1. Resolve source/target container.
2. Validate source and target slots.
3. Check merge, swap, equipment validation.
4. Commit slot changes.
5. Roll back if a partial write fails.
6. Refresh affected UI panels.

## Source

- [UIItemMoveManager.cs](../../src/Assets/00_Scripts/Storage_Scripts/StorageLogic/UIItemMoveManager.cs)

