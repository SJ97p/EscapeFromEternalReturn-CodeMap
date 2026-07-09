# Class Diagram

```mermaid
flowchart TD
    SceneController --> BootstrapSceneController
    SceneController --> LobbySceneController
    SceneController --> InGameSceneController
    GameSceneManager --> SceneController
    GameSceneManager --> SceneEnterContext

    NewUIManager --> UIPanel
    UIPanel --> UIPanelId
    UIPanelButton --> NewUIManager
    PlayerInventoryHud --> UIPanel
    EquipmentHud --> UIPanel
    StoragePanelUI --> UIPanel
    CraftingWorkbenchUI --> UIPanel

    CraftRecipeDatabase --> CraftTreeBuilder
    CraftTreeBuilder --> CraftTreeNode
    CraftTreeNode --> CraftTreeRenderer
    CraftTreeNode --> CraftingService
    CraftingService --> CraftingStorageAdapter
    CraftingStorageAdapter --> Storage

    IItemContainer --> InventoryContainerAdapter
    IItemContainer --> StorageContainerAdapter
    IItemContainer --> TargetInventoryContainerAdapter
    IItemContainer --> EquipmentAdapter
    UIItemMoveManager --> IItemContainer
    UIDragDropManager --> IItemContainer
    InventoryContainerAdapter --> Storage
    StorageContainerAdapter --> Storage
    EquipmentAdapter --> Storage

    Storage --> StorageSlot
    Storage --> StorageData
    StorageRepository --> StorageData
    StorageRepository --> DBLoader
    GameRepositories --> StorageRepository
    DBLoader --> SQLiteConnection

    PlayerRegionTracker --> ZoneController
    RegionGraph --> ZoneController
    ZoneController --> Zone
    ZoneController --> ZoneState
```

## Link Map

- [SceneController](classes/SceneController.md)
- [GameSceneManager](classes/GameSceneManager.md)
- [NewUIManager](classes/NewUIManager.md)
- [UIPanel](classes/UIPanel.md)
- [CraftTreeBuilder](classes/CraftTreeBuilder.md)
- [CraftingService](classes/CraftingService.md)
- [IItemContainer](classes/IItemContainer.md)
- [UIItemMoveManager](classes/UIItemMoveManager.md)
- [Storage](classes/Storage.md)
- [StorageRepository](classes/StorageRepository.md)
- [ZoneController](classes/ZoneController.md)
- [RegionGraph](classes/RegionGraph.md)
