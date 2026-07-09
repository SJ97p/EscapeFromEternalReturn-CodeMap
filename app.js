const repoBase = "https://github.com/sj97p/EscapeFromEternalReturn-CodeMap/blob/main/";

const nodes = {
  overview: {
    kind: "Overview",
    title: "런타임 시스템 구조",
    summary:
      "씬, UI, 제작, 아이템 저장소, 저장/로드, 지역 최적화를 공통 규격과 데이터 흐름으로 연결한 런타임 아키텍처입니다.",
    problem:
      "씬 전환, UI 패널, 아이템 이동, 제작, 저장, 월드 활성화가 각자 구현되면 기능이 늘어날수록 결합도가 높아지고 데이터 무결성이 깨지기 쉽습니다.",
    solution:
      "SceneController, UIPanel Registry, IItemContainer Adapter, UIItemMoveManager, StorageRepository, RegionGraph/ZoneController로 책임을 나누고 중앙 흐름을 만들었습니다.",
    classes: ["SceneController", "NewUIManager", "CraftTreeBuilder", "IItemContainer", "UIItemMoveManager", "StorageRepository", "ZoneController"],
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
  },
  "scene-ui-lifecycle": {
    kind: "System",
    title: "Scene Lifecycle & UI Registry",
    summary:
      "모든 씬은 공통 SceneController 라이프사이클을 따르고, UI 패널은 UIPanelId 기반 레지스트리로 등록/조회/제어됩니다.",
    problem:
      "씬마다 UI와 전환 로직을 직접 참조하면 씬이 늘어날수록 컨텍스트 전달과 패널 상태가 어긋나기 쉽습니다.",
    solution:
      "GameSceneManager가 씬 전환과 컨텍스트 전달을 중앙화하고, NewUIManager가 UIPanelId로 패널 Open/Close/Toggle을 처리합니다.",
    doc: "docs/systems/scene-ui-lifecycle.md",
    classes: ["SceneController", "GameSceneManager", "NewUIManager", "UIPanel"],
    graph: `flowchart TD
      request["Scene Change Request"]
      manager["GameSceneManager"]
      context["SceneEnterContext"]
      controller["SceneController"]
      ui["NewUIManager"]
      panel["UIPanel / UIPanelId"]

      request --> manager --> context
      manager --> controller
      controller --> ui --> panel

      click manager call selectNode("GameSceneManager")
      click controller call selectNode("SceneController")
      click ui call selectNode("NewUIManager")
      click panel call selectNode("UIPanel")`,
  },
  "recursive-crafting-tree": {
    kind: "System",
    title: "Recursive Crafting Tree",
    summary:
      "재료가 다시 제작 아이템일 수 있는 다단계 조합식을 레시피 DB 기반 재귀 트리로 생성하고 런타임 UI로 렌더링합니다.",
    problem:
      "제작 재료가 다시 제작 가능한 아이템이면 단순 리스트 UI로는 전체 제작 경로를 표현하기 어렵습니다.",
    solution:
      "CraftTreeBuilder가 레시피를 재귀 탐색해 CraftTreeNode를 만들고, CraftTreeRenderer와 CraftingService가 표시와 제작 실행을 나눠 맡습니다.",
    doc: "docs/systems/recursive-crafting-tree.md",
    classes: ["CraftTreeBuilder", "CraftingService", "CraftTreeNode", "CraftTreeRenderer"],
    graph: `flowchart TD
      db["CraftRecipeDatabase"]
      builder["CraftTreeBuilder"]
      node["CraftTreeNode"]
      child["Recursive Ingredients"]
      renderer["CraftTreeRenderer"]
      service["CraftingService"]

      db --> builder --> node --> child
      child --> builder
      node --> renderer
      node --> service

      click builder call selectNode("CraftTreeBuilder")
      click service call selectNode("CraftingService")`,
  },
  "item-container-transaction": {
    kind: "System",
    title: "Item Container Transaction",
    summary:
      "인벤토리, 창고, 장비창, 루팅창을 IItemContainer와 Adapter로 통합하고, UIItemMoveManager가 이동 트랜잭션을 처리합니다.",
    problem:
      "각 UI가 아이템 이동을 따로 처리하면 병합, 스왑, 장비 검증, 자동 루팅 규칙이 중복되고 아이템 복사/증발 위험이 커집니다.",
    solution:
      "컨테이너를 인터페이스로 추상화하고 중앙 이동 루틴에서 Resolve, Validate, Merge/Swap, Commit, Refresh 순서로 처리합니다.",
    doc: "docs/systems/item-container-transaction.md",
    classes: ["IItemContainer", "UIItemMoveManager", "Storage", "InventoryContainerAdapter", "StorageContainerAdapter", "EquipmentAdapter"],
    graph: `flowchart TD
      ui["Drag / Click"]
      iface["IItemContainer"]
      adapters["Adapters"]
      move["UIItemMoveManager"]
      validate["Validate / Merge / Swap"]
      commit["Commit or Rollback"]
      refresh["Refresh UI"]

      ui --> iface
      adapters --> iface
      iface --> move --> validate --> commit --> refresh

      click iface call selectNode("IItemContainer")
      click move call selectNode("UIItemMoveManager")
      click commit call selectNode("Storage")`,
  },
  "storage-persistence": {
    kind: "System",
    title: "SQLite Storage Persistence",
    summary:
      "런타임 Storage 슬롯 데이터를 저장 시점에 StorageData DTO로 변환하고, SQLite Repository를 통해 세이브 슬롯별로 저장/로드합니다.",
    problem:
      "런타임 슬롯 배열을 DB 구조에 직접 묶으면 UI, 게임 로직, 저장 로직이 강하게 결합됩니다.",
    solution:
      "Storage는 런타임 모델만 관리하고, 저장 시 비어있지 않은 슬롯만 StorageData로 추출해 StorageRepository가 SQLite row로 매핑합니다.",
    doc: "docs/systems/storage-persistence.md",
    classes: ["Storage", "StorageRepository", "DBLoader", "GameRepositories"],
    graph: `flowchart TD
      storage["StorageSlot[,]"]
      export["ExportToStorageData"]
      dto["StorageData DTO"]
      repo["StorageRepository"]
      loader["DBLoader"]
      sqlite["SQLite DB"]

      storage --> export --> dto --> repo
      loader --> repo --> sqlite

      click storage call selectNode("Storage")
      click repo call selectNode("StorageRepository")`,
  },
  "zone-culling": {
    kind: "System",
    title: "RegionGraph Zone Culling",
    summary:
      "플레이어 현재 Region과 인접 Region만 활성화해 보이지 않는 지역의 몬스터/상자/이벤트 부하를 줄입니다.",
    problem:
      "전체 월드를 항상 활성화하면 플레이어가 보지 않는 지역까지 Update와 렌더링 비용이 발생합니다.",
    solution:
      "RegionGraph로 현재 지역의 인접 지역을 조회하고, ZoneController가 activeRegions 차집합만 계산해 Zone GameObject를 토글합니다.",
    doc: "docs/systems/zone-culling.md",
    classes: ["ZoneController", "RegionGraph", "Zone", "PlayerRegionTracker"],
    graph: `flowchart TD
      tracker["PlayerRegionTracker"]
      graph["RegionGraph"]
      controller["ZoneController"]
      active["activeRegions"]
      enable["regionsToEnable"]
      disable["regionsToDisable"]
      zone["Zone GameObjects"]

      tracker --> controller
      graph --> controller --> active
      active --> enable --> zone
      active --> disable --> zone

      click controller call selectNode("ZoneController")
      click graph call selectNode("RegionGraph")`,
  },
  "zone-state-api": {
    kind: "System",
    title: "Zone State API / Collaboration Extension",
    summary:
      "금지구역, 하이퍼루프, 지역 이벤트 같은 협업 기능이 Zone 내부 구현을 직접 알지 않고 Region과 ZoneState API로 연결되도록 확장 지점을 분리했습니다.",
    problem:
      "지역 기반 기능이 Zone 내부 오브젝트나 스폰 구현을 직접 조작하면 협업 중 변경 비용이 커집니다.",
    solution:
      "ZoneController의 SetZoneState, SetZonesState, OnZoneStateChanged를 통해 외부 시스템은 상태 API만 사용하도록 분리했습니다.",
    doc: "docs/systems/zone-state-api.md",
    classes: ["ZoneController", "Zone", "RegionGraph"],
    graph: `flowchart TD
      external["External Region Feature"]
      api["ZoneController API"]
      zone["Zone.SetZoneState"]
      event["OnZoneStateChanged"]
      ui["UI / Damage / Event Extensions"]

      external --> api --> zone
      api --> event --> ui

      click api call selectNode("ZoneController")`,
  },
  SceneController: classNode("SceneController", "모든 씬 컨트롤러가 따르는 공통 라이프사이클 기반 클래스입니다.", "src/Assets/00_Scripts/Core/SceneController.cs", ["Scene Lifecycle"], `classDiagram
    class SceneController {
      <<abstract>>
      +GameScene SceneType
      +BGMType BGM
      +void Initialize(SceneEnterContext context)
      +void Enter()
      +void Exit()
    }`),
  GameSceneManager: classNode("GameSceneManager", "씬 전환과 씬 진입 컨텍스트 전달을 중앙에서 관리합니다.", "src/Assets/00_Scripts/Core/GameSceneManager.cs", ["Scene Lifecycle"], `classDiagram
    class GameSceneManager {
      +void ChangeScene(GameScene scene)
      +void ChangeScene(GameScene scene, SceneEnterContext context)
      +SceneEnterContext ConsumeEnterContext()
    }
    GameSceneManager --> SceneEnterContext`),
  NewUIManager: classNode("NewUIManager", "UIPanelId 기반으로 UI 패널을 등록하고 Open/Close/Toggle을 처리합니다.", "src/Assets/00_Scripts/Core/NewUIManager.cs", ["UI Registry"], `classDiagram
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
    NewUIManager --> UIPanel`),
  UIPanel: classNode("UIPanel", "모든 UI 패널이 따르는 공통 패널 규격입니다.", "src/Assets/00_Scripts/UI_Scripts/UILogic/UIPanel.cs", ["UI Registry"], `classDiagram
    class UIPanel {
      +UIPanelId PanelId
      +void Open()
      +void Close()
    }`),
  CraftTreeBuilder: classNode("CraftTreeBuilder", "레시피 데이터를 기반으로 다단계 제작 트리를 재귀적으로 생성합니다.", "src/Assets/00_Scripts/Craft/CraftTreeBuilder.cs", ["Recursive Crafting Tree"], `classDiagram
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
    CraftTreeNode --> CraftTreeNode`),
  CraftingService: classNode("CraftingService", "제작 가능 여부, 부족 재료, 실제 제작 처리를 담당합니다.", "src/Assets/00_Scripts/Craft/CraftingService.cs", ["Recursive Crafting Tree"], `classDiagram
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
    CraftingService --> CraftTreeNode`),
  IItemContainer: classNode("IItemContainer", "서로 다른 아이템 창을 같은 이동 시스템에 연결하기 위한 공통 인터페이스입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/IItemContainer.cs", ["Item Container Transaction"], `classDiagram
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
    IItemContainer <|.. EquipmentAdapter`),
  UIItemMoveManager: classNode("UIItemMoveManager", "컨테이너 간 이동, 병합, 스왑, 자동 이동, 장비 검증을 중앙에서 처리합니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/UIItemMoveManager.cs", ["Item Container Transaction"], `classDiagram
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
    }
    UIItemMoveManager --> IItemContainer`),
  Storage: classNode("Storage", "인벤토리, 창고, 장비창, 루팅창이 공유하는 런타임 슬롯 데이터 모델입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/Storage.cs", ["Item Transaction", "Persistence"], `classDiagram
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
      +bool AddItem(int itemId, int amount)
      +bool RemoveItem(int itemId, int amount)
      +List~StorageData~ ExportToStorageData(StorageType storageType, int saveId)
    }
    Storage *-- StorageSlot
    Storage --> StorageData`),
  StorageRepository: classNode("StorageRepository", "StorageData와 SQLite row 사이를 매핑하는 Repository입니다.", "src/Assets/00_Scripts/DataBase/StorageRepository.cs", ["Storage Persistence"], `classDiagram
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
    StorageRepository --> StorageItem`),
  ZoneController: classNode("ZoneController", "현재 지역과 인접 지역만 활성화하고 협업 기능을 위한 Zone 상태 API를 제공하는 컨트롤러입니다.", "src/Assets/00_Scripts/ZoneControllers/ZoneController.cs", ["Zone Culling", "Zone State API"], `classDiagram
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
    ZoneController --> Zone`),
  RegionGraph: classNode("RegionGraph", "지역과 인접 지역 관계를 데이터로 표현해 Zone Culling의 기준을 제공합니다.", "src/Assets/00_Scripts/ZoneControllers/RegionGraph.cs", ["Zone Culling"], `classDiagram
    class RegionGraph {
      +List~RegionNodeData~ nodes
    }
    class RegionNodeData {
      +Region region
      +List~Region~ adjacentRegions
    }
    RegionGraph *-- RegionNodeData`),
  InventoryContainerAdapter: classNode("InventoryContainerAdapter", "인벤토리 HUD와 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.", "src/Assets/00_Scripts/Inventory_Scripts/InventoryLogic/InventoryContainerAdapter.cs", ["Adapter Pattern"], `classDiagram
    class InventoryContainerAdapter {
      -PlayerInventoryHud hud
      -Storage storage
      +ItemContainerType ContainerType
      +int Width
      +int Height
      +bool IsValidSlot(int x, int y)
      +bool SetSlot(int x, int y, int itemId, int amount)
      +bool ClearSlot(int x, int y)
      +bool HandleDrop(UIDragPayload payload, int toX, int toY)
      +bool HandleClick(int x, int y)
      +void RefreshUI()
    }
    InventoryContainerAdapter ..|> IItemContainer
    InventoryContainerAdapter --> Storage`),
  StorageContainerAdapter: classNode("StorageContainerAdapter", "창고 패널과 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/StorageContainerAdapter.cs", ["Adapter Pattern"], `classDiagram
    class StorageContainerAdapter {
      -Storage storage
      -StoragePanelUI panelUI
      +ItemContainerType ContainerType
      +int Width
      +int Height
      +bool IsValidSlot(int x, int y)
      +bool HandleDrop(UIDragPayload payload, int toX, int toY)
      +bool HandleClick(int x, int y)
      +void RefreshUI()
    }
    StorageContainerAdapter ..|> IItemContainer
    StorageContainerAdapter --> Storage`),
  EquipmentAdapter: classNode("EquipmentAdapter", "장비창을 IItemContainer로 연결하고 슬롯 타입 검증과 스탯 반영을 처리합니다.", "src/Assets/00_Scripts/Equipment/EquipmentAdapter.cs", ["Adapter Pattern"], `classDiagram
    class EquipmentAdapter {
      -Storage storage
      -EquipmentHud hud
      +ItemContainerType ContainerType
      +bool SetSlot(int x, int y, int itemId, int amount)
      +bool ClearSlot(int x, int y)
      +bool HandleDrop(UIDragPayload payload, int toX, int toY)
      +bool CanEquipItemToSlot(int itemId, int x, int y)
      -void UpdatePlayerStats(int oldItemId, int newItemId)
      +void RefreshUI()
    }
    EquipmentAdapter ..|> IItemContainer
    EquipmentAdapter --> Storage`),
};

function classNode(title, summary, source, related, graph) {
  return {
    kind: "Class",
    title,
    summary,
    problem: "이 책임이 UI나 다른 시스템에 직접 섞이면 기능 추가 시 변경 범위가 커지고 데이터 흐름을 추적하기 어려워집니다.",
    solution: "공통 인터페이스, ID 기반 레지스트리, Repository, 상태 API 같은 경계로 책임을 제한했습니다.",
    source,
    classes: related,
    graph: graph || `flowchart TD
      self["${title}"]
      related["${related.join(" / ")}"]
      source["Source Code"]
      self --> related
      self --> source`,
  };
}

const treeGroups = [
  {
    title: "Systems",
    items: [
      ["scene-ui-lifecycle", "Scene / UI"],
      ["recursive-crafting-tree", "Crafting Tree"],
      ["item-container-transaction", "Item Transaction"],
      ["storage-persistence", "Storage Persistence"],
      ["zone-culling", "Zone Culling"],
      ["zone-state-api", "Zone State API"],
    ],
  },
  {
    title: "Classes",
    items: [
      ["SceneController", "SceneController"],
      ["NewUIManager", "NewUIManager"],
      ["CraftTreeBuilder", "CraftTreeBuilder"],
      ["IItemContainer", "IItemContainer"],
      ["UIItemMoveManager", "UIItemMoveManager"],
      ["Storage", "Storage"],
      ["StorageRepository", "StorageRepository"],
      ["ZoneController", "ZoneController"],
      ["RegionGraph", "RegionGraph"],
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

    for (const [id, label] of group.items) {
      const button = document.createElement("button");
      button.type = "button";
      button.className = `tree-item ${nodes[id]?.kind === "Class" ? "child" : ""}`;
      button.textContent = label;
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
  graph.innerHTML = node.graph;

  if (!window.mermaid) {
    renderFallbackGraph(node);
    return;
  }

  try {
    window.mermaid.initialize({ startOnLoad: false, securityLevel: "loose", theme: "neutral" });
    await window.mermaid.run({ nodes: [graph] });
  } catch {
    renderFallbackGraph(node);
  }
}

function renderFallbackGraph(node) {
  const graph = document.querySelector("#graph");
  graph.className = "fallback-graph";
  graph.innerHTML = "";
  for (const className of node.classes || []) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "fallback-node";
    button.innerHTML = `<strong>${className}</strong><span>${nodes[className]?.summary || "Related node"}</span>`;
    if (nodes[className]) button.addEventListener("click", () => selectNode(className));
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
    preview.textContent = text.split("\n").slice(0, 100).join("\n");
  } catch {
    preview.textContent = `${source}\n\n브라우저 보안 정책 때문에 로컬 파일 미리보기를 불러오지 못했습니다. Open file 링크로 원본을 확인할 수 있습니다.`;
  }
}

async function render() {
  const node = nodes[selectedId] || nodes.overview;

  document.querySelector("#scope-label").textContent = node.kind;
  document.querySelector("#graph-title").textContent = node.title;
  document.querySelector("#breadcrumbs").textContent = selectedId === "overview" ? "Overview" : `Overview / ${node.title}`;
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
    const chip = document.createElement("button");
    chip.type = "button";
    chip.className = "chip";
    chip.textContent = className;
    if (nodes[className]) chip.addEventListener("click", () => selectNode(className));
    classList.append(chip);
  }

  await renderGraph(node);
  await loadCodePreview(node);
}

document.querySelector("#reset-view").addEventListener("click", () => selectNode("overview"));
renderTree();
render();
