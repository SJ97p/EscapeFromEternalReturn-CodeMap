const repoBase = "https://github.com/sj97p/EscapeFromEternalReturn-CodeMap/blob/main/";

const nodes = {
  overview: systemNode({
    title: "런타임 시스템 구조",
    summary:
      "씬, UI, 제작, 아이템 컨테이너, 저장/로드, 지역 최적화를 공통 규격과 데이터 흐름으로 연결한 런타임 아키텍처입니다.",
    problem:
      "각 기능을 화면이나 씬마다 따로 구현하면 아이템 이동 규칙, UI 호출, 저장 데이터, 지역 상태가 서로 강하게 얽혀 유지보수가 어려워집니다.",
    solution:
      "SceneController, UIPanel Registry, IItemContainer Adapter, UIItemMoveManager, StorageRepository, RegionGraph/ZoneController로 책임 경계를 만들었습니다.",
    classes: [
      "SceneController",
      "GameSceneManager",
      "NewUIManager",
      "CraftTreeBuilder",
      "IItemContainer",
      "UIItemMoveManager",
      "StorageRepository",
      "ZoneController",
      "PlayerRegionTracker",
    ],
    graph: `flowchart TD
      scene["Scene Lifecycle"]
      ui["UI Registry"]
      crafting["Recursive Crafting Tree"]
      item["Item Container Transaction"]
      persistence["SQLite Storage Persistence"]
      zone["RegionGraph Zone Culling"]
      zoneapi["Zone State API"]

      scene --> ui
      ui --> crafting
      ui --> item
      crafting --> item
      item --> persistence
      zone --> zoneapi
      zone --> item

      click scene call selectNode("scene-ui-lifecycle")
      click ui call selectNode("scene-ui-lifecycle")
      click crafting call selectNode("recursive-crafting-tree")
      click item call selectNode("item-container-transaction")
      click persistence call selectNode("storage-persistence")
      click zone call selectNode("zone-culling")
      click zoneapi call selectNode("zone-state-api")`,
  }),

  "scene-ui-lifecycle": systemNode({
    title: "Scene Lifecycle & UI Registry",
    summary:
      "모든 씬은 공통 SceneController 라이프사이클을 따르고, UI 패널은 UIPanelId 기반 레지스트리로 등록/조회/제어됩니다.",
    problem:
      "씬마다 전환 로직과 UI 참조를 직접 들고 있으면 씬 추가나 패널 교체 때 호출 구조가 쉽게 무너집니다.",
    solution:
      "GameSceneManager가 씬 전환과 컨텍스트 전달을 맡고, NewUIManager가 UIPanelId 기반 Open/Close/Toggle을 제공합니다.",
    doc: "docs/systems/scene-ui-lifecycle.md",
    classes: ["SceneController", "GameSceneManager", "SceneEnterContext", "NewUIManager", "UIPanel", "UIPanelButton"],
    graph: `flowchart TD
      request["Scene Change Request"]
      manager["GameSceneManager"]
      context["SceneEnterContext"]
      controller["SceneController"]
      ui["NewUIManager"]
      panel["UIPanel / UIPanelId"]
      button["UIPanelButton"]

      request --> manager --> context
      manager --> controller
      button --> ui
      controller --> ui --> panel

      click manager call selectNode("GameSceneManager")
      click context call selectNode("SceneEnterContext")
      click controller call selectNode("SceneController")
      click ui call selectNode("NewUIManager")
      click panel call selectNode("UIPanel")
      click button call selectNode("UIPanelButton")`,
  }),

  "recursive-crafting-tree": systemNode({
    title: "Recursive Crafting Tree",
    summary:
      "재료가 다시 제작 아이템일 수 있는 다단계 조합식을 레시피 DB 기반 재귀 트리로 생성하고 런타임 UI로 렌더링합니다.",
    problem:
      "레시피가 깊어질수록 UI에서 직접 하위 재료를 따라가면 코드가 복잡해지고 데이터 변경에 취약해집니다.",
    solution:
      "CraftTreeBuilder가 CraftRecipeDatabase를 재귀 탐색해 CraftTreeNode를 만들고, 렌더러와 서비스가 표시/실행을 분리합니다.",
    doc: "docs/systems/recursive-crafting-tree.md",
    classes: ["CraftTreeBuilder", "CraftTreeNode", "CraftTreeRenderer", "CraftingService", "CraftingStorageAdapter"],
    graph: `flowchart TD
      db["CraftRecipeDatabase"]
      builder["CraftTreeBuilder"]
      node["CraftTreeNode"]
      child["Recursive Ingredients"]
      renderer["CraftTreeRenderer"]
      service["CraftingService"]
      adapter["CraftingStorageAdapter"]

      db --> builder --> node --> child
      child --> builder
      node --> renderer
      node --> service --> adapter

      click builder call selectNode("CraftTreeBuilder")
      click node call selectNode("CraftTreeNode")
      click renderer call selectNode("CraftTreeRenderer")
      click service call selectNode("CraftingService")
      click adapter call selectNode("CraftingStorageAdapter")`,
  }),

  "item-container-transaction": systemNode({
    title: "Item Container Transaction",
    summary:
      "인벤토리, 창고, 장비창, 루팅창을 IItemContainer와 Adapter로 통합하고, UIItemMoveManager가 이동 트랜잭션을 처리합니다.",
    problem:
      "창마다 이동 로직을 따로 구현하면 병합, 스왑, 장비 검증, 자동 루팅 규칙이 중복되고 아이템 복사/증발 위험이 커집니다.",
    solution:
      "컨테이너를 공통 인터페이스로 추상화하고 중앙 이동 루틴에서 Resolve, Validate, Merge/Swap, Commit, Refresh 순서로 처리합니다.",
    doc: "docs/systems/item-container-transaction.md",
    classes: [
      "IItemContainer",
      "InventoryContainerAdapter",
      "StorageContainerAdapter",
      "TargetInventoryContainerAdapter",
      "EquipmentAdapter",
      "UIItemMoveManager",
      "Storage",
      "StorageSlot",
    ],
    graph: `flowchart TD
      ui["Drag / Click"]
      iface["IItemContainer"]
      inv["InventoryContainerAdapter"]
      storageAdapter["StorageContainerAdapter"]
      loot["TargetInventoryContainerAdapter"]
      equip["EquipmentAdapter"]
      move["UIItemMoveManager"]
      validate["Validate / Merge / Swap"]
      commit["Commit or Rollback"]
      storage["Storage / StorageSlot"]

      inv --> iface
      storageAdapter --> iface
      loot --> iface
      equip --> iface
      ui --> move
      iface --> move --> validate --> commit --> storage

      click iface call selectNode("IItemContainer")
      click inv call selectNode("InventoryContainerAdapter")
      click storageAdapter call selectNode("StorageContainerAdapter")
      click loot call selectNode("TargetInventoryContainerAdapter")
      click equip call selectNode("EquipmentAdapter")
      click move call selectNode("UIItemMoveManager")
      click storage call selectNode("Storage")`,
  }),

  "storage-persistence": systemNode({
    title: "SQLite Storage Persistence",
    summary:
      "런타임 Storage 슬롯 데이터를 저장 시점에 StorageData DTO로 변환하고, SQLite Repository를 통해 세이브 슬롯별로 저장/로드합니다.",
    problem:
      "런타임 슬롯 배열을 DB 구조에 직접 묶으면 UI, 게임 로직, 저장 로직이 서로 강하게 결합됩니다.",
    solution:
      "Storage는 런타임 모델만 관리하고, 저장 시 비어있지 않은 슬롯만 StorageData로 추출해 StorageRepository가 SQLite row로 매핑합니다.",
    doc: "docs/systems/storage-persistence.md",
    classes: ["Storage", "StorageSlot", "StorageData", "StorageRepository", "StorageItem", "DBLoader", "GameRepositories"],
    graph: `flowchart TD
      storage["StorageSlot[,]"]
      export["ExportToStorageData"]
      dto["StorageData DTO"]
      repo["StorageRepository"]
      row["StorageItem Row"]
      loader["DBLoader"]
      sqlite["SQLite DB"]

      storage --> export --> dto --> repo --> row --> sqlite
      loader --> repo

      click storage call selectNode("Storage")
      click dto call selectNode("StorageData")
      click repo call selectNode("StorageRepository")
      click row call selectNode("StorageItem")
      click loader call selectNode("DBLoader")`,
  }),

  "zone-culling": systemNode({
    title: "RegionGraph Zone Culling",
    summary:
      "PlayerRegionTracker가 감지한 현재 Region과 RegionGraph의 인접 Region만 활성화해 보이지 않는 지역의 런타임 부하를 줄입니다.",
    problem:
      "전체 월드를 항상 활성화하면 플레이어가 보지 않는 지역의 몬스터, 상자, 이벤트까지 Update와 렌더링 비용을 발생시킵니다.",
    solution:
      "ZoneController가 현재 지역과 인접 지역을 activeRegions로 유지하고, 이전/다음 집합의 차이만 계산해 Zone GameObject를 토글합니다.",
    doc: "docs/systems/zone-culling.md",
    classes: ["PlayerRegionTracker", "RegionGraph", "RegionNodeData", "ZoneController", "RegionZoneEntry", "Zone", "ZoneState"],
    graph: `flowchart TD
      tracker["PlayerRegionTracker"]
      graph["RegionGraph / RegionNodeData"]
      controller["ZoneController"]
      active["activeRegions"]
      enable["regionsToEnable"]
      disable["regionsToDisable"]
      zone["Zone GameObjects"]

      tracker --> controller
      graph --> controller --> active
      active --> enable --> zone
      active --> disable --> zone

      click tracker call selectNode("PlayerRegionTracker")
      click graph call selectNode("RegionGraph")
      click controller call selectNode("ZoneController")
      click zone call selectNode("Zone")`,
  }),

  "zone-state-api": systemNode({
    title: "Zone State API / Collaboration Extension",
    summary:
      "금지구역, 지역 이벤트, 이동 기능 같은 협업 시스템이 Zone 내부 구현을 직접 알지 않고 Region과 ZoneState API로 연결되도록 확장 지점을 분리했습니다.",
    problem:
      "지역 기반 기능이 Zone 내부 오브젝트나 스폰 구현을 직접 조작하면 협업 중 변경 비용이 커지고 책임 경계가 흐려집니다.",
    solution:
      "ZoneController의 SetZoneState, SetZonesState, OnZoneStateChanged를 통해 외부 시스템은 상태 API만 사용하도록 분리했습니다.",
    doc: "docs/systems/zone-state-api.md",
    classes: ["ZoneController", "Zone", "ZoneState", "RegionGraph", "PlayerRegionTracker"],
    graph: `flowchart TD
      external["External Region Feature"]
      region["Region"]
      api["ZoneController API"]
      zone["Zone.SetZoneState"]
      event["OnZoneStateChanged"]
      extensions["UI / Rule / Event Extensions"]

      external --> region --> api --> zone
      api --> event --> extensions

      click api call selectNode("ZoneController")
      click zone call selectNode("Zone")
      click event call selectNode("ZoneController")`,
  }),

  SceneController: classNode({
    title: "SceneController",
    summary: "모든 씬 컨트롤러가 따르는 공통 라이프사이클 기반 추상 클래스입니다.",
    source: "src/Assets/00_Scripts/Core/SceneController.cs",
    classes: ["GameSceneManager", "SceneEnterContext"],
    graph: `classDiagram
      class SceneController {
        <<abstract>>
        +GameScene SceneType
        +BGMType BGM
        +void Initialize(SceneEnterContext context)
        +void Enter()
        +void Exit()
      }
      SceneController --> SceneEnterContext`,
  }),

  GameSceneManager: classNode({
    title: "GameSceneManager",
    summary: "씬 전환과 씬 진입 컨텍스트 전달을 중앙에서 관리합니다.",
    source: "src/Assets/00_Scripts/Core/GameSceneManager.cs",
    classes: ["SceneController", "SceneEnterContext"],
    graph: `classDiagram
      class GameSceneManager {
        +void ChangeScene(GameScene scene)
        +void ChangeScene(GameScene scene, SceneEnterContext context)
        +SceneEnterContext ConsumeEnterContext()
      }
      GameSceneManager --> SceneEnterContext
      GameSceneManager --> SceneController`,
  }),

  SceneEnterContext: classNode({
    title: "SceneEnterContext",
    summary: "씬 전환 시 다음 씬에 전달할 진입 정보를 담는 컨텍스트 객체입니다.",
    source: "src/Assets/00_Scripts/Core/SceneEnterContext.cs",
    classes: ["GameSceneManager", "SceneController"],
    graph: `classDiagram
      class SceneEnterContext {
        +GameScene PreviousScene
        +GameScene NextScene
        +object Payload
      }
      GameSceneManager --> SceneEnterContext
      SceneController --> SceneEnterContext`,
  }),

  NewUIManager: classNode({
    title: "NewUIManager",
    summary: "UIPanelId 기반으로 UI 패널을 등록하고 Open/Close/Toggle을 처리하는 레지스트리입니다.",
    source: "src/Assets/00_Scripts/Core/NewUIManager.cs",
    classes: ["UIPanel", "UIPanelButton"],
    graph: `classDiagram
      class NewUIManager {
        -Dictionary~UIPanelId, UIPanel~ panelMap
        -HashSet~UIPanel~ openedPanels
        #void Awake()
        -void RebuildPanelMap()
        +void Open(UIPanelId id)
        +void Close(UIPanelId id)
        +void Toggle(UIPanelId id)
        +T Get~T~(UIPanelId id)
        +bool IsOpened(UIPanelId id)
        +void RegisterOpened(UIPanel panel)
        +void RegisterClosed(UIPanel panel)
        +bool IsWorldInputBlocked
      }
      NewUIManager --> UIPanel`,
  }),

  UIPanel: classNode({
    title: "UIPanel",
    summary: "인벤토리, 장비창, 창고, 제작대 등 모든 UI 패널이 따르는 공통 패널 규격입니다.",
    source: "src/Assets/00_Scripts/UI_Scripts/UILogic/UIPanel.cs",
    classes: ["NewUIManager", "UIPanelButton"],
    graph: `classDiagram
      class UIPanel {
        +UIPanelId PanelId
        +void Open()
        +void Close()
      }
      NewUIManager --> UIPanel`,
  }),

  UIPanelButton: classNode({
    title: "UIPanelButton",
    summary: "버튼 입력을 UIPanelId 기반 UI 명령으로 변환합니다.",
    source: "src/Assets/00_Scripts/UI_Scripts/UILogic/UIPanelButton.cs",
    classes: ["NewUIManager", "UIPanel"],
    graph: `classDiagram
      class UIPanelButton {
        -UIPanelId targetPanelId
        +void OnClick()
      }
      UIPanelButton --> NewUIManager`,
  }),

  CraftTreeBuilder: classNode({
    title: "CraftTreeBuilder",
    summary: "레시피 데이터를 기반으로 다단계 제작 트리를 재귀적으로 생성합니다.",
    source: "src/Assets/00_Scripts/Craft/CraftTreeBuilder.cs",
    classes: ["CraftTreeNode", "CraftingService", "CraftTreeRenderer"],
    graph: `classDiagram
      class CraftTreeBuilder {
        -CraftRecipeDatabase recipeDatabase
        +CraftTreeBuilder(CraftRecipeDatabase recipeDatabase)
        +CraftTreeNode BuildTree(int rootItemId, int needAmount)
        -CraftTreeNode BuildNode(int itemId, int needAmount, HashSet~int~ visited)
      }
      class CraftTreeNode {
        +int ItemId
        +int NeedAmount
        +CraftTreeNode Left
        +CraftTreeNode Right
      }
      CraftTreeBuilder --> CraftRecipeDatabase
      CraftTreeBuilder --> CraftTreeNode
      CraftTreeNode --> CraftTreeNode`,
  }),

  CraftTreeNode: classNode({
    title: "CraftTreeNode",
    summary: "제작 결과 또는 재료 아이템을 트리 노드로 표현합니다.",
    source: "src/Assets/00_Scripts/Craft/CraftTreeNode.cs",
    classes: ["CraftTreeBuilder", "CraftTreeRenderer"],
    graph: `classDiagram
      class CraftTreeNode {
        +int ItemId
        +int NeedAmount
        +CraftTreeNode Left
        +CraftTreeNode Right
      }
      CraftTreeNode --> CraftTreeNode : child`,
  }),

  CraftTreeRenderer: classNode({
    title: "CraftTreeRenderer",
    summary: "CraftTreeNode 구조를 런타임 UI 노드로 변환합니다.",
    source: "src/Assets/00_Scripts/Craft/CraftTreeRenderer.cs",
    classes: ["CraftTreeNode", "CraftTreeNodeView"],
    graph: `classDiagram
      class CraftTreeRenderer {
        +void Render(CraftTreeNode root)
        +void Clear()
      }
      CraftTreeRenderer --> CraftTreeNode
      CraftTreeRenderer --> CraftTreeNodeView`,
  }),

  CraftingService: classNode({
    title: "CraftingService",
    summary: "제작 가능 여부, 부족 재료, 실제 제작 처리를 담당합니다.",
    source: "src/Assets/00_Scripts/Craft/CraftingService.cs",
    classes: ["CraftTreeNode", "CraftingStorageAdapter", "Storage"],
    graph: `classDiagram
      class CraftingService {
        -CraftingStorageAdapter storageAdapter
        +bool CanCraft(CraftTreeNode rootNode)
        +bool TryCraft(CraftTreeNode rootNode)
        +Dictionary~int, int~ GetRequiredItems(CraftTreeNode rootNode)
        +Dictionary~int, int~ GetMissingItems(CraftTreeNode rootNode)
        +int GetTotalItemCount(int itemId)
        -void CollectDirectRequirements(CraftTreeNode rootNode, Dictionary~int, int~ result)
        -void AddRequirement(CraftTreeNode node, Dictionary~int, int~ result)
      }
      CraftingService --> CraftingStorageAdapter
      CraftingService --> CraftTreeNode`,
  }),

  CraftingStorageAdapter: classNode({
    title: "CraftingStorageAdapter",
    summary: "제작 서비스가 인벤토리/창고 보유량을 같은 방식으로 조회하고 차감하도록 연결합니다.",
    source: "src/Assets/00_Scripts/Craft/CraftingStorageAdapter.cs",
    classes: ["CraftingService", "Storage"],
    graph: `classDiagram
      class CraftingStorageAdapter {
        -Storage inventory
        -Storage storage
        +int GetTotalItemCount(int itemId)
        +bool HasItem(int itemId, int amount)
        +bool RemoveItem(int itemId, int amount)
        +bool AddItem(int itemId, int amount)
      }
      CraftingStorageAdapter --> Storage`,
  }),

  IItemContainer: classNode({
    title: "IItemContainer",
    summary: "서로 다른 아이템 창을 같은 이동 시스템에 연결하기 위한 공통 인터페이스입니다.",
    source: "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/IItemContainer.cs",
    classes: ["InventoryContainerAdapter", "StorageContainerAdapter", "TargetInventoryContainerAdapter", "EquipmentAdapter", "UIItemMoveManager"],
    graph: `classDiagram
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
      IItemContainer <|.. InventoryContainerAdapter
      IItemContainer <|.. StorageContainerAdapter
      IItemContainer <|.. TargetInventoryContainerAdapter
      IItemContainer <|.. EquipmentAdapter`,
  }),

  UIItemMoveManager: classNode({
    title: "UIItemMoveManager",
    summary: "컨테이너 간 이동, 병합, 스왑, 자동 이동, 장비 검증을 중앙에서 처리합니다.",
    source: "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/UIItemMoveManager.cs",
    classes: ["IItemContainer", "Storage", "EquipmentAdapter"],
    graph: `classDiagram
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
        -bool TryMerge(IItemContainer from, int fromX, int fromY, IItemContainer to, int toX, int toY)
        -bool CanMerge(IItemContainer from, int fromX, int fromY, IItemContainer to, int toX, int toY)
        -bool IsEquippableItem(int itemId)
      }
      UIItemMoveManager --> IItemContainer`,
  }),

  InventoryContainerAdapter: adapterNode({
    title: "InventoryContainerAdapter",
    summary: "인벤토리 HUD와 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.",
    source: "src/Assets/00_Scripts/Inventory_Scripts/InventoryLogic/InventoryContainerAdapter.cs",
    ui: "PlayerInventoryHud",
  }),

  StorageContainerAdapter: adapterNode({
    title: "StorageContainerAdapter",
    summary: "창고 패널과 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.",
    source: "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/StorageContainerAdapter.cs",
    ui: "StoragePanelUI",
  }),

  TargetInventoryContainerAdapter: adapterNode({
    title: "TargetInventoryContainerAdapter",
    summary: "루팅 창과 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.",
    source: "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/TargetInventoryContainerAdapter.cs",
    ui: "TargetInventoryHud",
  }),

  EquipmentAdapter: classNode({
    title: "EquipmentAdapter",
    summary: "장비창을 IItemContainer로 연결하고 슬롯 타입 검증과 장비 스탯 반영을 처리합니다.",
    source: "src/Assets/00_Scripts/Equipment/EquipmentAdapter.cs",
    classes: ["IItemContainer", "Storage", "UIItemMoveManager"],
    graph: `classDiagram
      class EquipmentAdapter {
        -Storage storage
        -EquipmentHud hud
        +ItemContainerType ContainerType
        +int Width
        +int Height
        +bool IsValidSlot(int x, int y)
        +bool SetSlot(int x, int y, int itemId, int amount)
        +bool ClearSlot(int x, int y)
        +bool HandleDrop(UIDragPayload payload, int toX, int toY)
        +bool CanEquipItemToSlot(int itemId, int x, int y)
        -void UpdatePlayerStats(int oldItemId, int newItemId)
        +void RefreshUI()
      }
      EquipmentAdapter ..|> IItemContainer
      EquipmentAdapter --> Storage`,
  }),

  Storage: classNode({
    title: "Storage",
    summary: "인벤토리, 창고, 장비창, 루팅창이 공유하는 런타임 슬롯 데이터 모델입니다.",
    source: "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/Storage.cs",
    classes: ["StorageSlot", "StorageData", "StorageRepository"],
    graph: `classDiagram
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
        +bool AddItem(int itemId, int amount)
        +bool RemoveItem(int itemId, int amount)
        +List~StorageData~ ExportToStorageData(StorageType storageType, int saveId)
      }
      Storage *-- StorageSlot
      Storage --> StorageData`,
  }),

  StorageSlot: classNode({
    title: "StorageSlot",
    summary: "Storage의 한 칸을 표현하는 슬롯 데이터입니다.",
    source: "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/StorageSlot.cs",
    classes: ["Storage"],
    graph: `classDiagram
      class StorageSlot {
        +int X
        +int Y
        +int ItemId
        +int Amount
        +bool IsEmpty
        +void SetItem(int itemId, int amount)
        +void Clear()
      }
      Storage *-- StorageSlot`,
  }),

  StorageData: classNode({
    title: "StorageData",
    summary: "저장 시점에 런타임 StorageSlot을 SQLite 저장용 데이터로 변환한 DTO입니다.",
    source: "src/Assets/00_Scripts/Player/Storage/StorageData.cs",
    classes: ["Storage", "StorageRepository"],
    graph: `classDiagram
      class StorageData {
        +int Id
        +int SaveId
        +StorageType StorageType
        +int ItemId
        +int Quantity
        +int X
        +int Y
      }
      Storage --> StorageData
      StorageRepository --> StorageData`,
  }),

  StorageRepository: classNode({
    title: "StorageRepository",
    summary: "StorageData와 SQLite row 사이를 매핑하는 Repository입니다.",
    source: "src/Assets/00_Scripts/DataBase/StorageRepository.cs",
    classes: ["StorageData", "StorageItem", "DBLoader", "GameRepositories"],
    graph: `classDiagram
      class StorageRepository {
        -DBLoader _dbLoader
        -string _dbKey
        +IEnumerable~StorageData~ GetAll()
        +StorageData GetById(int id)
        +StorageData MapFromRow(StorageItem row)
        +void DeleteAllBySaveId(int saveId)
        +void Add(StorageData data)
        +IEnumerable~StorageData~ GetBySaveId(int saveId)
      }
      StorageRepository --> DBLoader
      StorageRepository --> StorageData
      StorageRepository --> StorageItem`,
  }),

  StorageItem: classNode({
    title: "StorageItem",
    summary: "SQLite player storage table row를 표현하는 DB 모델입니다.",
    source: "src/Assets/00_Scripts/DataBase/PlayerStorageDB.cs",
    classes: ["StorageRepository", "StorageData"],
    graph: `classDiagram
      class StorageItem {
        +int Id
        +int SaveId
        +int StorageType
        +int ItemId
        +int Quantity
        +int X
        +int Y
      }
      StorageRepository --> StorageItem`,
  }),

  DBLoader: classNode({
    title: "DBLoader",
    summary: "StreamingAssets의 SQLite DB 파일을 검색하고 연결을 캐싱/복구합니다.",
    source: "src/Assets/00_Scripts/DataBase/DBLoader.cs",
    classes: ["StorageRepository", "GameRepositories"],
    graph: `classDiagram
      class DBLoader {
        +Dictionary~string, SQLiteConnection~ dbConnections
        -string[] _cachedDbFiles
        -string[] GetDatabaseFiles()
        +void ConnectSQLite()
        +SQLiteConnection GetConnection(string key)
        -void OnDestroy()
      }
      StorageRepository --> DBLoader
      GameRepositories --> DBLoader`,
  }),

  GameRepositories: classNode({
    title: "GameRepositories",
    summary: "DBLoader에서 연결을 받아 도메인별 Repository를 구성합니다.",
    source: "src/Assets/00_Scripts/DataBase/GameRepositories.cs",
    classes: ["DBLoader", "StorageRepository"],
    graph: `classDiagram
      class GameRepositories {
        +ItemRepository Items
        +MonsterSpawnPointRepository MonsterSpawns
        +ItemContainerSpawnPointRepository ItemContainerSpawns
        +StorageRepository Storages
        +SaveFileRepository SaveFiles
        +GameRepositories(DBLoader dbLoader)
      }
      GameRepositories --> DBLoader
      GameRepositories --> StorageRepository`,
  }),

  PlayerRegionTracker: classNode({
    title: "PlayerRegionTracker",
    summary: "플레이어가 현재 어느 Region에 있는지 감지하고 변경 이벤트를 발행합니다.",
    source: "src/Assets/00_Scripts/Player/Core/PlayerRegionTracker.cs",
    classes: ["ZoneController", "RegionSurface"],
    graph: `classDiagram
      class PlayerRegionTracker {
        +Region CurrentRegion
        +event Action~Region~ OnRegionChanged
        -void OnTriggerEnter(Collider other)
        -void OnTriggerExit(Collider other)
        -void SetCurrentRegion(Region region)
      }
      PlayerRegionTracker --> RegionSurface
      ZoneController --> PlayerRegionTracker`,
  }),

  RegionGraph: classNode({
    title: "RegionGraph",
    summary: "지역과 인접 지역 관계를 데이터로 표현해 Zone Culling의 기준을 제공합니다.",
    source: "src/Assets/00_Scripts/ZoneControllers/RegionGraph.cs",
    classes: ["RegionNodeData", "ZoneController"],
    graph: `classDiagram
      class RegionGraph {
        +List~RegionNodeData~ nodes
      }
      class RegionNodeData {
        +Region region
        +List~Region~ adjacentRegions
      }
      RegionGraph *-- RegionNodeData
      ZoneController --> RegionGraph`,
  }),

  RegionNodeData: classNode({
    title: "RegionNodeData",
    summary: "한 Region과 해당 Region의 인접 Region 목록을 보관합니다.",
    source: "src/Assets/00_Scripts/ZoneControllers/RegionGraph.cs",
    classes: ["RegionGraph"],
    graph: `classDiagram
      class RegionNodeData {
        +Region region
        +List~Region~ adjacentRegions
      }
      RegionGraph *-- RegionNodeData`,
  }),

  RegionZoneEntry: classNode({
    title: "RegionZoneEntry",
    summary: "Inspector에서 Region과 Zone 오브젝트를 연결하기 위한 매핑 엔트리입니다.",
    source: "src/Assets/00_Scripts/ZoneControllers/RegionZoneEntry.cs",
    classes: ["ZoneController", "Zone"],
    graph: `classDiagram
      class RegionZoneEntry {
        +Region region
        +Zone zone
      }
      ZoneController --> RegionZoneEntry
      RegionZoneEntry --> Zone`,
  }),

  ZoneController: classNode({
    title: "ZoneController",
    summary: "현재 지역과 인접 지역만 활성화하고 협업 기능을 위한 Zone 상태 API를 제공합니다.",
    source: "src/Assets/00_Scripts/ZoneControllers/ZoneController.cs",
    classes: ["PlayerRegionTracker", "RegionGraph", "RegionZoneEntry", "Zone", "ZoneState"],
    graph: `classDiagram
      class ZoneController {
        -PlayerRegionTracker playerRegionTracker
        -RegionGraphSO regionGraph
        -List~RegionZoneEntry~ regionZones
        -bool useZoneOptimization
        -Dictionary~Region, Zone~ regionZoneMap
        -HashSet~Region~ activeRegions
        +event Action~Region, ZoneState~ OnZoneStateChanged
        +Zone GetZone(Region region)
        +void SetZoneState(Region region, ZoneState state)
        +void SetZonesState(IEnumerable~Region~ regions, ZoneState state)
        -void UpdateZones(HashSet~Region~ nextRegions)
        -HashSet~Region~ GetRegionsToActivate(Region currentRegion)
      }
      ZoneController --> PlayerRegionTracker
      ZoneController --> RegionGraph
      ZoneController --> RegionZoneEntry
      ZoneController --> Zone`,
  }),

  Zone: classNode({
    title: "Zone",
    summary: "지역 단위 GameObject로, 지역 타입과 Zone 상태를 보유하고 지역별 스폰 루트를 관리합니다.",
    source: "src/Assets/00_Scripts/ZoneControllers/Zone.cs",
    classes: ["ZoneController", "ZoneState"],
    graph: `classDiagram
      class Zone {
        -Region _regionType
        -Transform _monsterRoot
        -Transform _itemContainerRoot
        -ZoneMonsterSpawnTable _spawnTable
        -ZoneState zoneState
        +Region RegionType
        +void SetZoneState(ZoneState state)
        +ZoneState GetZoneState()
      }
      ZoneController --> Zone`,
  }),

  ZoneState: classNode({
    title: "ZoneState",
    summary: "Normal, Warning, Restricted 같은 지역 상태를 표현하는 enum입니다.",
    source: "src/Assets/00_Scripts/ZoneControllers/ZoneState.cs",
    classes: ["Zone", "ZoneController"],
    graph: `classDiagram
      class ZoneState {
        <<enumeration>>
        NormalArea
        WarningArea
        RestrictedArea
      }
      Zone --> ZoneState
      ZoneController --> ZoneState`,
  }),
};

function systemNode(config) {
  return {
    kind: "System",
    ...config,
  };
}

function classNode(config) {
  return {
    kind: "Class",
    problem:
      "이 책임이 UI나 다른 시스템에 직접 섞이면 기능 추가 시 변경 범위가 커지고 데이터 흐름을 추적하기 어려워집니다.",
    solution:
      "공통 인터페이스, ID 기반 레지스트리, Repository, 상태 API 같은 경계로 책임을 제한했습니다.",
    ...config,
  };
}

function adapterNode({ title, summary, source, ui }) {
  return classNode({
    title,
    summary,
    source,
    classes: ["IItemContainer", "Storage", "UIItemMoveManager"],
    graph: `classDiagram
      class ${title} {
        -Storage storage
        -${ui} hud
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
      ${title} ..|> IItemContainer
      ${title} --> Storage
      UIItemMoveManager --> ${title}`,
  });
}

const treeGroups = [
  {
    title: "Systems",
    ids: [
      "overview",
      "scene-ui-lifecycle",
      "recursive-crafting-tree",
      "item-container-transaction",
      "storage-persistence",
      "zone-culling",
      "zone-state-api",
    ],
  },
  {
    title: "Scene / UI",
    ids: ["SceneController", "GameSceneManager", "SceneEnterContext", "NewUIManager", "UIPanel", "UIPanelButton"],
  },
  {
    title: "Crafting",
    ids: ["CraftTreeBuilder", "CraftTreeNode", "CraftTreeRenderer", "CraftingService", "CraftingStorageAdapter"],
  },
  {
    title: "Item Containers",
    ids: [
      "IItemContainer",
      "UIItemMoveManager",
      "InventoryContainerAdapter",
      "StorageContainerAdapter",
      "TargetInventoryContainerAdapter",
      "EquipmentAdapter",
      "Storage",
      "StorageSlot",
    ],
  },
  {
    title: "Persistence / Zone",
    ids: [
      "StorageData",
      "StorageRepository",
      "DBLoader",
      "GameRepositories",
      "PlayerRegionTracker",
      "RegionGraph",
      "ZoneController",
      "Zone",
      "ZoneState",
    ],
  },
];

let selectedId = "overview";

window.selectNode = (id) => {
  if (!nodes[id]) return;
  selectedId = id;
  render();
};

function renderTree() {
  const tree = document.querySelector("#tree");
  tree.innerHTML = "";

  for (const group of treeGroups) {
    const wrapper = document.createElement("div");
    wrapper.className = "tree-group";

    const title = document.createElement("div");
    title.className = "tree-title";
    title.textContent = group.title;
    wrapper.append(title);

    for (const id of group.ids) {
      const item = nodes[id];
      if (!item) continue;
      const button = document.createElement("button");
      button.type = "button";
      button.className = `tree-item ${item.kind === "Class" ? "child" : ""}`;
      button.textContent = item.title;
      button.dataset.nodeId = id;
      button.addEventListener("click", () => selectNode(id));
      wrapper.append(button);
    }

    tree.append(wrapper);
  }
}

async function renderGraph(node) {
  const graph = document.querySelector("#graph");
  graph.className = "mermaid";
  graph.removeAttribute("data-processed");
  graph.textContent = node.graph;

  if (!window.mermaid) {
    renderFallbackGraph(node);
    return;
  }

  try {
    window.mermaid.initialize({
      startOnLoad: false,
      securityLevel: "loose",
      theme: "base",
      themeVariables: {
        primaryColor: "#e8f1ff",
        primaryTextColor: "#17202e",
        primaryBorderColor: "#2166c2",
        lineColor: "#667085",
        secondaryColor: "#e8f7f4",
        tertiaryColor: "#fff7ed",
        fontFamily: "Segoe UI, Noto Sans KR, Arial",
      },
    });
    await window.mermaid.run({ nodes: [graph] });
    bindGraphNodeNavigation(node);
  } catch (error) {
    renderFallbackGraph(node, error.message);
  }
}

function bindGraphNodeNavigation(node) {
  const graph = document.querySelector("#graph");
  const targets = getNavigableGraphTargets(node);
  if (!targets.length) return;

  for (const targetId of targets) {
    const target = nodes[targetId];
    if (!target) continue;

    const graphId = cssEscape(`flowchart-${targetId}`);
    const directNode = graph.querySelector(`#${graphId}, #${cssEscape(targetId)}`);
    const labelNodes = [...graph.querySelectorAll(".node, .classGroup")].filter((element) =>
      normalizeGraphText(element.textContent).includes(normalizeGraphText(targetId)),
    );
    const candidates = directNode ? [directNode, ...labelNodes] : labelNodes;

    for (const element of candidates) {
      rewriteGraphLabel(element, targetId, target.title);
      element.setAttribute("role", "button");
      element.setAttribute("tabindex", "0");
      element.style.cursor = "pointer";
      element.addEventListener("click", () => selectNode(targetId));
      element.addEventListener("keydown", (event) => {
        if (event.key === "Enter" || event.key === " ") {
          event.preventDefault();
          selectNode(targetId);
        }
      });
    }
  }
}

function getNavigableGraphTargets(node) {
  const classTargets = node.classes || [];
  const graphText = node.graph || "";
  const graphTargets = Object.keys(nodes).filter((id) => graphText.includes(id));
  return [...new Set([...classTargets, ...graphTargets])].filter((id) => nodes[id]);
}

function rewriteGraphLabel(element, id, title) {
  const labels = [...element.querySelectorAll("text, .nodeLabel, .label")];
  for (const label of labels) {
    const text = normalizeGraphText(label.textContent);
    if (text === normalizeGraphText(id) || text.includes(normalizeGraphText(id))) {
      label.textContent = label.textContent.replace(new RegExp(id, "gi"), title);
    }
  }
}

function normalizeGraphText(value) {
  return String(value || "").replace(/\s+/g, "").toLowerCase();
}

function cssEscape(value) {
  if (window.CSS?.escape) return window.CSS.escape(value);
  return String(value).replace(/[^a-zA-Z0-9_-]/g, "\\$&");
}

function renderFallbackGraph(node, message = "") {
  const graph = document.querySelector("#graph");
  graph.className = "fallback-graph";
  graph.innerHTML = "";

  if (message) {
    const note = document.createElement("button");
    note.type = "button";
    note.className = "fallback-node";
    note.innerHTML = `<strong>Graph fallback</strong><span>${escapeHtml(message)}</span>`;
    graph.append(note);
  }

  for (const id of node.classes || []) {
    const item = nodes[id];
    if (!item) continue;
    const button = document.createElement("button");
    button.type = "button";
    button.className = "fallback-node";
    button.innerHTML = `<strong>${escapeHtml(item.title)}</strong><span>${escapeHtml(item.summary || item.kind)}</span>`;
    button.addEventListener("click", () => selectNode(id));
    graph.append(button);
  }
}

async function loadCodePreview(node) {
  const preview = document.querySelector("#code-preview");
  const link = document.querySelector("#code-link");
  const source = node.source;

  if (!source) {
    preview.textContent = "시스템 노드에서는 Key Classes를 선택하면 코드 미리보기가 표시됩니다.";
    link.href = node.doc ? repoBase + node.doc : repoBase;
    link.textContent = node.doc ? "Open docs" : "Open repo";
    return;
  }

  link.href = repoBase + source;
  link.textContent = "Open file";

  try {
    const response = await fetch(source);
    if (!response.ok) throw new Error("not found");
    const text = await response.text();
    preview.textContent = trimCode(text);
  } catch {
    preview.textContent = `${source}\n\n브라우저 보안 정책 때문에 로컬 파일 미리보기를 불러오지 못했습니다. Open file 링크로 원본을 확인할 수 있습니다.`;
  }
}

function trimCode(text) {
  const lines = text.replace(/\r\n/g, "\n").split("\n");
  if (lines.length <= 120) return text;
  return `${lines.slice(0, 120).join("\n")}\n\n// ... ${lines.length - 120} more lines. Open file for full source.`;
}

async function render() {
  const node = nodes[selectedId] || nodes.overview;

  document.querySelector("#scope-label").textContent = node.kind;
  document.querySelector("#graph-title").textContent = node.title;
  document.querySelector("#breadcrumbs").textContent = selectedId === "overview" ? "Overview" : `${node.kind} / ${node.title}`;
  document.querySelector("#detail-kind").textContent = node.kind;
  document.querySelector("#detail-title").textContent = node.title;
  document.querySelector("#detail-summary").textContent = node.summary;
  document.querySelector("#detail-problem").textContent = node.problem;
  document.querySelector("#detail-solution").textContent = node.solution;

  document.querySelectorAll(".tree-item").forEach((item) => {
    item.classList.toggle("active", item.dataset.nodeId === selectedId);
  });

  const classList = document.querySelector("#class-list");
  classList.innerHTML = "";
  for (const className of node.classes || []) {
    const target = nodes[className];
    if (!target) continue;
    const chip = document.createElement("button");
    chip.type = "button";
    chip.className = "chip";
    chip.textContent = target.title;
    chip.addEventListener("click", () => selectNode(className));
    classList.append(chip);
  }

  await renderGraph(node);
  await loadCodePreview(node);
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

document.querySelector("#reset-view").addEventListener("click", () => selectNode("overview"));
renderTree();
render();
