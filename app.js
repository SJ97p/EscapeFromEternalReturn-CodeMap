const repoBase = "https://github.com/sj97p/EscapeFromEternalReturn-CodeMap/blob/main/";

const evidence = {
  sceneLifecycle: {
    src: "assets/evidence/scene-lifecycle.gif",
    caption:
      "씬 전환 흐름을 Exit -> LoadSceneAsync -> Initialize(context) -> Enter 순서로 통일해, 씬이 늘어나도 동일한 진입/종료 규칙을 유지하도록 구성했습니다.",
  },
  uiRegistry: {
    src: "assets/evidence/ui-panel-registry.gif",
    caption:
      "UIPanelId 기반 레지스트리로 UI 호출 방식을 통일해, 새 패널이 추가되어도 동일한 방식으로 열기/닫기/토글이 가능하도록 설계했습니다.",
  },
  craftingTree: {
    src: "assets/evidence/crafting-tree.gif",
    caption:
      "레시피 데이터를 기반으로 CraftTreeBuilder가 재귀 제작 트리를 생성하고, UI는 생성된 CraftTreeNode 구조를 렌더링하는 역할만 담당하도록 분리했습니다.",
  },
  itemContainer: {
    src: "assets/evidence/item-container-transaction.gif",
    caption:
      "서로 다른 저장소를 Adapter로 공통 컨테이너 규격에 맞추고, 이동 요청은 중앙 매니저에서 검증 후 Commit되도록 구성해 데이터 무결성을 우선했습니다.",
  },
  zoneCulling: {
    src: "assets/evidence/zone-culling.gif",
    caption:
      "PlayerRegionTracker가 현재 Region을 감지하면, ZoneController가 RegionGraph를 기준으로 현재 지역과 인접 지역만 ActiveZone으로 유지합니다.",
  },
  zoneCpu: {
    src: "assets/evidence/zone-cpu-result.png",
    caption:
      "Zone Culling 적용 후 평균 CPU 사용량은 약 4.75ms에서 2.75ms로 감소했으며, 평균 기준 약 42.1% 개선을 확인했습니다.",
  },
};

const nodes = {
  overview: system({
    title: "런타임 시스템 구조",
    summary:
      "씬, UI, 제작, 아이템 컨테이너, 저장/로드, 지역 최적화를 공통 규격과 데이터 흐름으로 연결한 런타임 아키텍처입니다.",
    intent:
      "개별 기능보다 기능들이 서로 안정적으로 연결되는 구조를 만드는 데 초점을 두었습니다. 씬 전환, UI 호출, 아이템 이동, 저장/로드, 지역 활성화가 각자 따로 움직이면 변경 범위가 커지기 때문에 공통 규격과 중앙 진입점을 먼저 정의했습니다.",
    decision:
      "기존 인벤토리와 UI 구조는 특정 패널에 강하게 맞춰져 있어 창고, 루팅창, 장비창, 제작대까지 확장하기 어려웠습니다. 그래서 SceneController, UIPanelId, IItemContainer, StorageRepository, RegionGraph/ZoneController처럼 각 시스템의 경계를 명확히 나누었습니다.",
    final:
      "UML에서 각 시스템 노드를 클릭하면 설계 의도와 결과 증거, 관련 클래스, 실제 GitHub 코드로 이어집니다. 코드 자체보다 구조를 먼저 이해하고 세부 구현으로 들어갈 수 있도록 구성했습니다.",
    next:
      "다음 개선에서는 이동 정책의 Policy 분리, 제작 레시피 검증, Zone 이벤트 구독 구조 강화처럼 현재 구조의 확장 지점을 더 명확히 분리할 수 있습니다.",
    classes: [
      "SceneController",
      "NewUIManager",
      "CraftTreeBuilder",
      "IItemContainer",
      "UIItemMoveManager",
      "StorageRepository",
      "ZoneController",
      "PlayerRegionTracker",
    ],
    evidence: [evidence.sceneLifecycle, evidence.itemContainer, evidence.zoneCpu],
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

  "scene-ui-lifecycle": system({
    title: "Scene Lifecycle & UI Registry",
    summary:
      "모든 씬은 SceneController 라이프사이클을 따르고, UI 패널은 UIPanelId 기반 레지스트리로 등록/조회/제어됩니다.",
    intent:
      "처음부터 각 씬이 공통 라이프사이클을 따르도록 요구사항을 정리했습니다. 씬 이동 요청을 GameSceneManager라는 싱글톤 중앙 객체에서 처리하고, SceneEnterContext로 저장 슬롯과 캐릭터 관련 데이터를 다음 씬에 전달하는 구조를 의도했습니다.",
    decision:
      "기존 UI는 패널을 직접 참조하고 각 함수를 직접 호출하는 방식이었고, 인벤토리 하나에 강하게 맞춰져 있었습니다. 창고, 루팅창, 제작대처럼 새 패널이 늘어나면 호출 구조가 쉽게 흩어질 수 있어 UIPanelId 기반 Open/Close/Toggle 규격으로 통일했습니다.",
    final:
      "SceneController는 Exit, Initialize(context), Enter 같은 공통 흐름을 제공하고, NewUIManager는 UIPanelId로 패널을 조회해 동일한 방식으로 상태를 제어합니다. 버튼은 구체 패널을 몰라도 UIPanelId만 전달하면 됩니다.",
    next:
      "씬별 진입 데이터 타입을 더 명확히 분리하면 SceneEnterContext의 Payload 사용을 줄이고, 컴파일 타임에 더 안전한 씬 전환 계약을 만들 수 있습니다.",
    classes: ["SceneController", "GameSceneManager", "SceneEnterContext", "NewUIManager", "UIPanel", "UIPanelButton"],
    evidence: [evidence.sceneLifecycle, evidence.uiRegistry],
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

  "recursive-crafting-tree": system({
    title: "Recursive Crafting Tree",
    summary:
      "재료가 다시 제작 아이템일 수 있는 다단계 조합식을 레시피 데이터 기반 재귀 트리로 생성하고 런타임 UI로 렌더링합니다.",
    intent:
      "UI는 계산을 하지 않고 출력만 담당해야 한다고 보았습니다. 레시피를 기반으로 데이터 단에서 제작 트리를 완성한 뒤, UI는 완성된 CraftTreeNode 구조만 렌더링하도록 책임을 분리했습니다.",
    decision:
      "재료가 다시 제작 아이템일 수 있다는 요구사항이 처음부터 있었기 때문에 재귀 탐색이 필요했습니다. 구현 과정에서 가장 큰 도전은 Unity 기능보다 실제 데이터를 알고리즘 구조로 녹여내는 일이었습니다.",
    final:
      "CraftTreeBuilder는 레시피 데이터를 탐색해 트리 축을 만들고, CraftTreeNode는 각 노드 정보를 담으며, CraftTreeRenderer는 UI 출력만 담당합니다. CraftingService는 외부 요청을 받아 제작 가능 여부와 부족 재료 계산을 제공합니다.",
    next:
      "레시피는 직접 관리하는 데이터였기 때문에 순환 레시피 방지나 중복 재료 캐싱은 구현하지 않았습니다. 이후 데이터 규모가 커지거나 외부 편집이 가능해진다면 순환 검증, 중복 합산 캐싱, 레시피 유효성 검사를 추가할 수 있습니다.",
    classes: ["CraftTreeBuilder", "CraftTreeNode", "CraftTreeRenderer", "CraftingService", "CraftingStorageAdapter"],
    evidence: [evidence.craftingTree],
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

  "item-container-transaction": system({
    title: "Item Container Transaction",
    summary:
      "인벤토리, 창고, 장비창, 루팅창을 IItemContainer와 Adapter로 통합하고 UIItemMoveManager가 이동 트랜잭션을 처리합니다.",
    intent:
      "기존 인벤토리 시스템은 자동 루팅과 패널 열기/닫기처럼 인벤토리 하나의 동작에는 맞춰져 있었지만, 창고, 장비창, 루팅창, 제작대까지 확장하기에는 닫혀 있었습니다. 서로 다른 저장소라도 아이템을 보관하고 슬롯을 갱신한다는 공통점은 같다고 판단했습니다.",
    decision:
      "각 컨테이너는 크기와 규칙이 달랐습니다. 장비창은 장비 타입 검증이 필요했고, 창고와 루팅창은 이동 우선순위가 달랐습니다. 그래서 IItemContainer로 CRUD와 Refresh 규격을 정의하고, 각 UI는 Adapter로 자신의 규칙을 공통 인터페이스에 맞추도록 했습니다.",
    final:
      "UIItemMoveManager는 클릭 이동, 드래그 앤 드랍, 스택 병합, 스왑, 자동 루팅, 장비 장착 검증을 중앙에서 처리합니다. 이동은 Try 계열 검증을 거쳐 Commit되며, 완료 후 각 컨테이너 UI가 Refresh됩니다.",
    next:
      "현재 이동 정책과 우선순위가 UIItemMoveManager에 집중되어 있습니다. 다음 개선에서는 이동 정책을 별도 Policy 객체로 분리하고, Undo 또는 명시적인 rollback 구조를 도입해 새 컨테이너 추가 비용을 줄일 수 있습니다.",
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
    evidence: [evidence.itemContainer],
    graph: `flowchart TD
      ui["Drag / Click"]
      iface["IItemContainer"]
      inv["InventoryContainerAdapter"]
      storageAdapter["StorageContainerAdapter"]
      loot["TargetInventoryContainerAdapter"]
      equip["EquipmentAdapter"]
      move["UIItemMoveManager"]
      validate["Validate / Merge / Swap"]
      commit["Commit + Refresh"]

      inv --> iface
      storageAdapter --> iface
      loot --> iface
      equip --> iface
      ui --> move --> iface --> validate --> commit

      click iface call selectNode("IItemContainer")
      click inv call selectNode("InventoryContainerAdapter")
      click storageAdapter call selectNode("StorageContainerAdapter")
      click loot call selectNode("TargetInventoryContainerAdapter")
      click equip call selectNode("EquipmentAdapter")
      click move call selectNode("UIItemMoveManager")`,
  }),

  "storage-persistence": system({
    title: "SQLite Storage Persistence",
    summary:
      "런타임 Storage 데이터를 저장 시점에 StorageData로 변환하고 SQLite Repository를 통해 세이브 슬롯별로 저장/로드합니다.",
    intent:
      "Unity에서 직접 사용하기 편한 내장형 데이터베이스로 SQLite를 선택했습니다. 런타임 Storage와 저장용 StorageData를 분리해 저장 데이터의 무결성을 유지하고, 세이브 슬롯 단위로 저장/로드되도록 만들고자 했습니다.",
    decision:
      "빈 슬롯은 저장하지 않고, 아이템이 존재하는 슬롯의 storageType, saveId, itemId, quantity, x, y를 저장했습니다. DB 접근은 StorageRepository로 모으고, DBLoader는 연결이 끊겼을 때 복구할 수 있는 연결 지점을 담당하도록 했습니다.",
    final:
      "Storage는 런타임 슬롯 모델을 유지하고, 저장 시점에 StorageData DTO로 변환합니다. 저장/로드 이후에는 전체 Storage UI를 Refresh해 화면과 데이터 상태를 다시 맞춥니다.",
    next:
      "현재 저장 구조는 슬롯 좌표와 UI 구조에 어느 정도 의존합니다. 이후에는 저장 정책과 UI 표시 정책을 더 분리하고, 슬롯 크기 변경이나 컨테이너 추가에도 더 유연한 구조로 개선할 수 있습니다.",
    classes: ["Storage", "StorageSlot", "StorageData", "StorageRepository", "StorageItem", "DBLoader", "GameRepositories"],
    evidence: [evidence.itemContainer],
    graph: `flowchart TD
      runtime["Storage Runtime Slots"]
      export["ExportToStorageData"]
      dto["StorageData"]
      repo["StorageRepository"]
      row["StorageItem SQLite Row"]
      db["DBLoader"]
      save["Save Slot"]

      runtime --> export --> dto --> repo --> row
      db --> repo
      save --> repo

      click runtime call selectNode("Storage")
      click dto call selectNode("StorageData")
      click repo call selectNode("StorageRepository")
      click row call selectNode("StorageItem")
      click db call selectNode("DBLoader")`,
  }),

  "zone-culling": system({
    title: "RegionGraph Zone Culling",
    summary:
      "PlayerRegionTracker가 감지한 현재 Region과 RegionGraph의 인접 Region만 활성화해 보이지 않는 지역의 런타임 부하를 줄입니다.",
    intent:
      "맵과 오브젝트 수가 많고 몬스터와 상자 스폰도 존재했기 때문에, 플레이어가 보지 않는 지역까지 항상 활성화하는 구조는 피하고자 했습니다. 광활한 맵을 탐색하는 느낌은 유지하면서 메모리와 Update 비용을 줄이는 것이 목표였습니다.",
    decision:
      "RegionGraph를 ScriptableObject 기반 데이터로 두어 지역 추가나 인접 관계 수정이 쉬운 구조로 만들었습니다. PlayerRegionTracker는 플레이어 하단 방향으로 바닥 Collider를 감지해 현재 Region을 얻고, ZoneController는 현재 지역과 인접 지역만 활성 집합으로 유지합니다.",
    final:
      "activeRegions는 HashSet으로 관리해 중복을 제거하고 포함 여부 검사를 빠르게 처리합니다. 적용 후 평균 CPU 사용량은 약 4.75ms에서 2.75ms로 감소했고, 평균 기준 약 42.1% 개선을 확인했습니다.",
    next:
      "Zone 비활성화 자체는 문제없이 동작했지만, 이후에는 Zone 활성/비활성 이벤트를 두고 몬스터와 상자가 이를 구독하는 방식으로 바꾸면 결합도를 더 낮출 수 있습니다.",
    classes: ["PlayerRegionTracker", "RegionGraph", "RegionNodeData", "RegionZoneEntry", "ZoneController", "Zone", "ZoneState"],
    evidence: [evidence.zoneCulling, evidence.zoneCpu],
    graph: `flowchart TD
      tracker["PlayerRegionTracker"]
      current["Current Region"]
      regionGraphNode["RegionGraph"]
      adjacent["Adjacent Regions"]
      controller["ZoneController"]
      active["Active Zone Set"]
      inactive["Inactive Zones"]

      tracker --> current --> controller
      regionGraphNode --> adjacent --> controller
      controller --> active
      controller --> inactive

      click tracker call selectNode("PlayerRegionTracker")
      click regionGraphNode call selectNode("RegionGraph")
      click controller call selectNode("ZoneController")
      click active call selectNode("Zone")`,
  }),

  "zone-state-api": system({
    title: "Zone State API",
    summary:
      "금지구역, 이동, 이벤트 같은 지역 기반 기능이 Zone 내부 구현을 몰라도 상태 API로 연결될 수 있도록 경계를 마련했습니다.",
    intent:
      "금지구역, 하이퍼루프, 몬스터 스폰, 이벤트 같은 팀원 기능이 지역 구조 위에 붙을 수 있도록 ZoneController와 RegionGraph를 공통 기반으로 제공했습니다.",
    decision:
      "현재 코드맵 스냅샷 기준으로 OnZoneStateChanged를 외부 클래스가 직접 구독한 코드는 확인되지 않습니다. 따라서 실제 구독 사례로 단정하기보다, SetZoneState, SetZonesState, OnZoneStateChanged를 통해 구독 가능한 확장 지점을 제공한 것으로 표현하는 것이 정확합니다.",
    final:
      "외부 기능은 Zone 내부 GameObject 구조를 직접 몰라도 ZoneController API를 통해 지역 상태를 변경하거나 상태 변경 이벤트에 연결될 수 있습니다.",
    next:
      "다음 개선에서는 RestrictedZone, Hyperloop, Event 시스템이 ZoneState 이벤트를 명시적으로 구독하는 예제를 남겨 협업 API의 사용 방식을 더 분명히 만들 수 있습니다.",
    classes: ["ZoneController", "Zone", "ZoneState", "RegionGraph", "PlayerRegionTracker"],
    evidence: [evidence.zoneCulling],
    graph: `flowchart LR
      external["External Region Feature"]
      api["ZoneController API"]
      state["ZoneState"]
      zone["Zone"]
      event["OnZoneStateChanged"]

      external --> api --> state --> zone
      api --> event

      click api call selectNode("ZoneController")
      click state call selectNode("ZoneState")
      click zone call selectNode("Zone")`,
  }),
};

Object.assign(nodes, {
  SceneController: cls("SceneController", "모든 씬 컨트롤러가 따르는 공통 라이프사이클 기반 추상 클래스입니다.", "src/Assets/00_Scripts/Core/SceneController.cs", ["GameSceneManager", "SceneEnterContext"], `classDiagram
    class SceneController {
      <<abstract>>
      +GameScene SceneType
      +BGMType BGM
      +void Initialize(SceneEnterContext context)
      +void Enter()
      +void Exit()
    }
    SceneController --> SceneEnterContext`),
  GameSceneManager: cls("GameSceneManager", "씬 전환과 씬 진입 컨텍스트 전달을 중앙에서 관리합니다.", "src/Assets/00_Scripts/Core/GameSceneManager.cs", ["SceneController", "SceneEnterContext"], `classDiagram
    class GameSceneManager {
      +void ChangeScene(GameScene scene)
      +void ChangeScene(GameScene scene, SceneEnterContext context)
      +SceneEnterContext ConsumeEnterContext()
    }
    GameSceneManager --> SceneEnterContext
    GameSceneManager --> SceneController`),
  SceneEnterContext: cls("SceneEnterContext", "씬 전환 시 다음 씬에 전달할 저장 슬롯과 진입 정보를 담는 컨텍스트 객체입니다.", "src/Assets/00_Scripts/Core/SceneEnterContext.cs", ["GameSceneManager", "SceneController"], `classDiagram
    class SceneEnterContext {
      +GameScene PreviousScene
      +GameScene NextScene
      +object Payload
    }
    GameSceneManager --> SceneEnterContext`),
  NewUIManager: cls("NewUIManager", "UIPanelId 기반으로 UI 패널을 등록하고 Open/Close/Toggle을 처리하는 레지스트리입니다.", "src/Assets/00_Scripts/Core/NewUIManager.cs", ["UIPanel", "UIPanelButton"], `classDiagram
    class NewUIManager {
      -Dictionary_UIPanelId_UIPanel panelMap
      -HashSet_UIPanel openedPanels
      +void Open(UIPanelId id)
      +void Close(UIPanelId id)
      +void Toggle(UIPanelId id)
      +T Get_T(UIPanelId id)
      +bool IsOpened(UIPanelId id)
    }
    NewUIManager --> UIPanel`),
  UIPanel: cls("UIPanel", "인벤토리, 장비창, 창고, 제작대 등 모든 UI 패널이 따르는 공통 패널 규격입니다.", "src/Assets/00_Scripts/UI_Scripts/UILogic/UIPanel.cs", ["NewUIManager", "UIPanelButton"], `classDiagram
    class UIPanel {
      +UIPanelId PanelId
      +void Open()
      +void Close()
    }
    NewUIManager --> UIPanel`),
  UIPanelButton: cls("UIPanelButton", "버튼 입력을 UIPanelId 기반 UI 명령으로 변환합니다.", "src/Assets/00_Scripts/UI_Scripts/UILogic/UIPanelButton.cs", ["NewUIManager", "UIPanel"], `classDiagram
    class UIPanelButton {
      -UIPanelId targetPanelId
      +void OnClick()
    }
    UIPanelButton --> NewUIManager`),
  CraftTreeBuilder: cls("CraftTreeBuilder", "레시피 데이터를 기반으로 다단계 제작 트리를 재귀적으로 생성합니다.", "src/Assets/00_Scripts/Craft/CraftTreeBuilder.cs", ["CraftTreeNode", "CraftingService", "CraftTreeRenderer"], `classDiagram
    class CraftTreeBuilder {
      -CraftRecipeDatabase recipeDatabase
      +CraftTreeNode BuildTree(int rootItemId, int needAmount)
      -CraftTreeNode BuildNode(int itemId, int needAmount, HashSet_int visited)
    }
    CraftTreeBuilder --> CraftTreeNode`),
  CraftTreeNode: cls("CraftTreeNode", "제작 결과 또는 재료 아이템을 트리 노드로 표현합니다.", "src/Assets/00_Scripts/Craft/CraftTreeNode.cs", ["CraftTreeBuilder", "CraftTreeRenderer"], `classDiagram
    class CraftTreeNode {
      +int ItemId
      +int NeedAmount
      +CraftTreeNode Left
      +CraftTreeNode Right
    }
    CraftTreeNode --> CraftTreeNode : child`),
  CraftTreeRenderer: cls("CraftTreeRenderer", "CraftTreeNode 구조를 런타임 UI 노드로 변환합니다.", "src/Assets/00_Scripts/Craft/CraftTreeRenderer.cs", ["CraftTreeNode", "CraftTreeNodeView"], `classDiagram
    class CraftTreeRenderer {
      +void Render(CraftTreeNode root)
      +void Clear()
    }
    CraftTreeRenderer --> CraftTreeNode`),
  CraftingService: cls("CraftingService", "제작 가능 여부, 부족 재료, 실제 제작 처리를 담당합니다.", "src/Assets/00_Scripts/Craft/CraftingService.cs", ["CraftTreeNode", "CraftingStorageAdapter", "Storage"], `classDiagram
    class CraftingService {
      -CraftingStorageAdapter storageAdapter
      +bool CanCraft(CraftTreeNode rootNode)
      +bool TryCraft(CraftTreeNode rootNode)
      +Dictionary_int_int GetRequiredItems(CraftTreeNode rootNode)
      +Dictionary_int_int GetMissingItems(CraftTreeNode rootNode)
    }
    CraftingService --> CraftingStorageAdapter`),
  CraftingStorageAdapter: cls("CraftingStorageAdapter", "제작 서비스가 인벤토리/창고 보유량을 같은 방식으로 조회하고 차감하도록 연결합니다.", "src/Assets/00_Scripts/Craft/CraftingStorageAdapter.cs", ["CraftingService", "Storage"], `classDiagram
    class CraftingStorageAdapter {
      -Storage inventory
      -Storage storage
      +int GetTotalItemCount(int itemId)
      +bool HasItem(int itemId, int amount)
      +bool RemoveItem(int itemId, int amount)
    }
    CraftingStorageAdapter --> Storage`),
  IItemContainer: cls("IItemContainer", "서로 다른 아이템 창을 같은 이동 시스템에 연결하기 위한 공통 인터페이스입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/IItemContainer.cs", ["InventoryContainerAdapter", "StorageContainerAdapter", "TargetInventoryContainerAdapter", "EquipmentAdapter", "UIItemMoveManager"], `classDiagram
    class IItemContainer {
      <<interface>>
      +ItemContainerType ContainerType
      +int Width
      +int Height
      +bool IsValidSlot(int x, int y)
      +bool SetSlot(int x, int y, int itemId, int amount)
      +bool ClearSlot(int x, int y)
      +void RefreshUI()
    }
    IItemContainer <|.. InventoryContainerAdapter
    IItemContainer <|.. StorageContainerAdapter
    IItemContainer <|.. TargetInventoryContainerAdapter
    IItemContainer <|.. EquipmentAdapter`),
  UIItemMoveManager: cls("UIItemMoveManager", "컨테이너 간 이동, 병합, 스왑, 자동 이동, 장비 검증을 중앙에서 처리합니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/UIItemMoveManager.cs", ["IItemContainer", "Storage", "EquipmentAdapter"], `classDiagram
    class UIItemMoveManager {
      -Dictionary_ItemContainerType_IItemContainer containers
      -HashSet_ItemContainerType activeUIStates
      +void RegisterContainer(IItemContainer container)
      +bool TryMove(ItemContainerType fromType, int fromX, int fromY, ItemContainerType toType, int toX, int toY)
      +bool TryAutoMove(ItemContainerType fromType, int fromX, int fromY)
    }
    UIItemMoveManager --> IItemContainer`),
  InventoryContainerAdapter: adapter("InventoryContainerAdapter", "인벤토리 HUD와 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.", "src/Assets/00_Scripts/Inventory_Scripts/InventoryLogic/InventoryContainerAdapter.cs", "PlayerInventoryHud"),
  StorageContainerAdapter: adapter("StorageContainerAdapter", "창고 패널과 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/StorageContainerAdapter.cs", "StoragePanelUI"),
  TargetInventoryContainerAdapter: adapter("TargetInventoryContainerAdapter", "루팅 창과 Storage 모델을 IItemContainer로 연결하는 Adapter입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/TargetInventoryContainerAdapter.cs", "TargetInventoryHud"),
  EquipmentAdapter: adapter("EquipmentAdapter", "장비창을 IItemContainer로 연결하고 슬롯 타입 검증과 장비 스탯 반영을 처리합니다.", "src/Assets/00_Scripts/Equipment/EquipmentAdapter.cs", "EquipmentHud"),
  Storage: cls("Storage", "인벤토리, 창고, 장비창, 루팅창이 공유하는 런타임 슬롯 데이터 모델입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/Storage.cs", ["StorageSlot", "StorageData", "StorageRepository"], `classDiagram
    class Storage {
      +int Width
      +int Height
      -StorageSlot[,] slots
      +void LoadFromDB(List_StorageData rows)
      +void SetItem(int x, int y, int itemId, int amount)
      +void ClearSlot(int x, int y)
      +List_StorageData ExportToStorageData(StorageType storageType, int saveId)
    }
    Storage *-- StorageSlot
    Storage --> StorageData`),
  StorageSlot: cls("StorageSlot", "Storage의 한 칸을 표현하는 슬롯 데이터입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/StorageSlot.cs", ["Storage"], `classDiagram
    class StorageSlot {
      +int X
      +int Y
      +int ItemId
      +int Amount
      +bool IsEmpty
      +void SetItem(int itemId, int amount)
      +void Clear()
    }
    Storage *-- StorageSlot`),
  StorageData: cls("StorageData", "저장 시점에 런타임 StorageSlot을 SQLite 저장용 데이터로 변환한 DTO입니다.", "src/Assets/00_Scripts/DataBase/StorageRepository.cs", ["Storage", "StorageRepository"], `classDiagram
    class StorageData {
      +int SaveId
      +StorageType StorageType
      +int ItemId
      +int Quantity
      +int X
      +int Y
    }
    Storage --> StorageData`),
  StorageRepository: cls("StorageRepository", "StorageData와 SQLite row 사이를 매핑하는 Repository입니다.", "src/Assets/00_Scripts/DataBase/StorageRepository.cs", ["StorageData", "StorageItem", "DBLoader"], `classDiagram
    class StorageRepository {
      -DBLoader _dbLoader
      +IEnumerable_StorageData GetAll()
      +StorageData MapFromRow(StorageItem row)
      +void DeleteAllBySaveId(int saveId)
      +void Add(StorageData data)
      +IEnumerable_StorageData GetBySaveId(int saveId)
    }
    StorageRepository --> DBLoader
    StorageRepository --> StorageData`),
  StorageItem: cls("StorageItem", "SQLite player storage table row를 표현하는 DB 모델입니다.", "src/Assets/00_Scripts/DataBase/PlayerStorageDB.cs", ["StorageRepository", "StorageData"], `classDiagram
    class StorageItem {
      +int Id
      +int SaveId
      +int StorageType
      +int ItemId
      +int Quantity
      +int X
      +int Y
    }
    StorageRepository --> StorageItem`),
  DBLoader: cls("DBLoader", "StreamingAssets의 SQLite DB 파일을 검색하고 연결을 캐싱/복구합니다.", "src/Assets/00_Scripts/DataBase/DBLoader.cs", ["StorageRepository", "GameRepositories"], `classDiagram
    class DBLoader {
      +Dictionary_string_SQLiteConnection dbConnections
      +void ConnectSQLite()
      +SQLiteConnection GetConnection(string key)
    }
    StorageRepository --> DBLoader`),
  GameRepositories: cls("GameRepositories", "DBLoader에서 연결을 받아 도메인별 Repository를 구성합니다.", "src/Assets/00_Scripts/DataBase/GameRepositories.cs", ["DBLoader", "StorageRepository"], `classDiagram
    class GameRepositories {
      +ItemRepository Items
      +StorageRepository Storages
      +SaveFileRepository SaveFiles
    }
    GameRepositories --> DBLoader`),
  PlayerRegionTracker: cls("PlayerRegionTracker", "플레이어가 현재 어느 Region에 있는지 감지하고 변경 이벤트를 발행합니다.", "src/Assets/00_Scripts/Player/Core/PlayerRegionTracker.cs", ["ZoneController", "RegionGraph"], `classDiagram
    class PlayerRegionTracker {
      +Region CurrentRegion
      +event Action_Region OnRegionChanged
      -void SetCurrentRegion(Region region)
    }
    ZoneController --> PlayerRegionTracker`),
  RegionGraph: cls("RegionGraph", "지역과 인접 지역 관계를 데이터로 표현해 Zone Culling의 기준을 제공합니다.", "src/Assets/00_Scripts/ZoneControllers/RegionGraph.cs", ["RegionNodeData", "ZoneController"], `classDiagram
    class RegionGraph {
      +List_RegionNodeData nodes
    }
    class RegionNodeData {
      +Region region
      +List_Region adjacentRegions
    }
    RegionGraph *-- RegionNodeData`),
  RegionNodeData: cls("RegionNodeData", "한 Region과 해당 Region의 인접 Region 목록을 보관합니다.", "src/Assets/00_Scripts/ZoneControllers/RegionGraph.cs", ["RegionGraph"], `classDiagram
    class RegionNodeData {
      +Region region
      +List_Region adjacentRegions
    }`),
  RegionZoneEntry: cls("RegionZoneEntry", "Inspector에서 Region과 Zone 오브젝트를 연결하기 위한 매핑 엔트리입니다.", "src/Assets/00_Scripts/ZoneControllers/RegionZoneEntry.cs", ["ZoneController", "Zone"], `classDiagram
    class RegionZoneEntry {
      +Region region
      +Zone zone
    }
    RegionZoneEntry --> Zone`),
  ZoneController: cls("ZoneController", "현재 지역과 인접 지역만 활성화하고 협업 기능을 위한 Zone 상태 API를 제공합니다.", "src/Assets/00_Scripts/ZoneControllers/ZoneController.cs", ["PlayerRegionTracker", "RegionGraph", "Zone", "ZoneState"], `classDiagram
    class ZoneController {
      -PlayerRegionTracker playerRegionTracker
      -RegionGraphSO regionGraph
      -Dictionary_Region_Zone regionZoneMap
      -HashSet_Region activeRegions
      +event Action_Region_ZoneState OnZoneStateChanged
      +void SetZoneState(Region region, ZoneState state)
      +void SetZonesState(IEnumerable_Region regions, ZoneState state)
      -void UpdateZones(HashSet_Region nextRegions)
    }
    ZoneController --> PlayerRegionTracker
    ZoneController --> RegionGraph
    ZoneController --> Zone`),
  Zone: cls("Zone", "지역 단위 GameObject로 지역 타입과 Zone 상태를 보유합니다.", "src/Assets/00_Scripts/ZoneControllers/Zone.cs", ["ZoneController", "ZoneState"], `classDiagram
    class Zone {
      -Region _regionType
      -ZoneState zoneState
      +Region RegionType
      +void SetZoneState(ZoneState state)
      +ZoneState GetZoneState()
    }
    Zone --> ZoneState`),
  ZoneState: cls("ZoneState", "Normal, Warning, Restricted 같은 지역 상태를 표현하는 enum입니다.", "src/Assets/00_Scripts/ZoneControllers/ZoneState.cs", ["Zone", "ZoneController"], `classDiagram
    class ZoneState {
      +NormalArea
      +WarningArea
      +RestrictedArea
    }`),
});

const treeGroups = [
  { title: "Systems", ids: ["overview", "scene-ui-lifecycle", "recursive-crafting-tree", "item-container-transaction", "storage-persistence", "zone-culling", "zone-state-api"] },
  { title: "Scene / UI", ids: ["SceneController", "GameSceneManager", "SceneEnterContext", "NewUIManager", "UIPanel", "UIPanelButton"] },
  { title: "Crafting", ids: ["CraftTreeBuilder", "CraftTreeNode", "CraftTreeRenderer", "CraftingService", "CraftingStorageAdapter"] },
  { title: "Item Containers", ids: ["IItemContainer", "UIItemMoveManager", "InventoryContainerAdapter", "StorageContainerAdapter", "TargetInventoryContainerAdapter", "EquipmentAdapter", "Storage", "StorageSlot"] },
  { title: "Persistence / Zone", ids: ["StorageData", "StorageRepository", "StorageItem", "DBLoader", "GameRepositories", "PlayerRegionTracker", "RegionGraph", "RegionNodeData", "RegionZoneEntry", "ZoneController", "Zone", "ZoneState"] },
];

let selectedId = "overview";
const navigationStack = [];
let currentCodeMethods = [];

window.selectNode = (id, options = {}) => {
  if (!nodes[id]) return;
  const shouldPush = options.push !== false;
  if (shouldPush && selectedId !== id) {
    navigationStack.push(selectedId);
    if (navigationStack.length > 40) navigationStack.shift();
  }
  selectedId = id;
  render();
};

function goBack() {
  const previousId = navigationStack.pop();
  if (!previousId) return;
  selectNode(previousId, { push: false });
}

function system(config) {
  return { kind: "System", ...config };
}

function cls(title, summary, source, classes, graph) {
  return {
    kind: "Class",
    title,
    summary,
    source,
    classes,
    graph,
    intent: "이 책임이 UI나 다른 시스템에 직접 섞이면 기능 추가 시 변경 범위가 커지고 데이터 흐름을 추적하기 어려워집니다.",
    decision: "공통 인터페이스, ID 기반 레지스트리, Repository, 상태 API 같은 경계로 책임을 제한했습니다.",
    final: "클래스 다이어그램에서 관계를 확인하고, Code Preview 또는 Open file 링크로 실제 구현을 확인할 수 있습니다.",
    next: "시스템 규모가 커질 경우 정책 객체, 검증 로직, 이벤트 구독 예제를 추가해 확장 지점을 더 명확히 만들 수 있습니다.",
    evidence: [],
  };
}

function adapter(title, summary, source, uiName) {
  return cls(title, summary, source, ["IItemContainer", "Storage", "UIItemMoveManager"], `classDiagram
    class ${title} {
      -Storage storage
      -${uiName} hud
      +ItemContainerType ContainerType
      +int Width
      +int Height
      +bool IsValidSlot(int x, int y)
      +bool SetSlot(int x, int y, int itemId, int amount)
      +bool ClearSlot(int x, int y)
      +void RefreshUI()
    }
    ${title} ..|> IItemContainer
    ${title} --> Storage`);
}

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
  graph.textContent = withClassClicks(node);

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
    normalizeGraphLabelCase(node);
    attachGraphNodeClicks(node);
    attachGraphMethodClicks(node);
  } catch (error) {
    renderFallbackGraph(node, error.message);
  }
}

function withClassClicks(node) {
  return node.graph;
}

function attachGraphNodeClicks(node) {
  const graph = document.querySelector("#graph");
  const svg = graph.querySelector("svg");
  if (!svg) return;

  const targets = getGraphClickTargets(node);
  const textNodes = [...svg.querySelectorAll("text, tspan, span")];

  for (const target of targets) {
    for (const textNode of textNodes) {
      if (!isGraphLabelMatch(textNode.textContent, target.label)) continue;

      const clickable = textNode.closest("g.node, g.classGroup, g.cluster, g") || textNode;
      clickable.classList.add("graph-clickable");
      clickable.setAttribute("role", "button");
      clickable.setAttribute("tabindex", "0");
      clickable.setAttribute("aria-label", `Open ${target.label}`);
      clickable.addEventListener("click", (event) => {
        event.stopPropagation();
        selectNode(target.id);
      });
      clickable.addEventListener("keydown", (event) => {
        if (event.key !== "Enter" && event.key !== " ") return;
        event.preventDefault();
        selectNode(target.id);
      });
    }
  }
}

function attachGraphMethodClicks(node) {
  const graph = document.querySelector("#graph");
  const svg = graph.querySelector("svg");
  if (!svg || !node.source) return;

  const methods = getGraphMethods(node);
  if (!methods.length) return;

  const textNodes = [...svg.querySelectorAll("text, tspan, span.nodeLabel")];
  for (const method of methods) {
    for (const textNode of textNodes) {
      const text = normalizeLabel(textNode.textContent);
      if (!text.includes(`${method.name}(`) && !text.includes(`${method.graphName}(`)) continue;

      textNode.classList.add("graph-method-clickable");
      textNode.setAttribute("role", "button");
      textNode.setAttribute("tabindex", "0");
      textNode.setAttribute("aria-label", `Jump to ${method.name}`);
      textNode.addEventListener("click", (event) => {
        event.stopPropagation();
        jumpToMethod(method.name);
      });
      textNode.addEventListener("keydown", (event) => {
        if (event.key !== "Enter" && event.key !== " ") return;
        event.preventDefault();
        event.stopPropagation();
        jumpToMethod(method.name);
      });
    }
  }
}

function normalizeGraphLabelCase(node) {
  const graph = document.querySelector("#graph");
  const svg = graph.querySelector("svg");
  if (!svg) return;

  const labels = getGraphLabels(node);
  const textNodes = [...svg.querySelectorAll("text, tspan, span.nodeLabel")];

  for (const textNode of textNodes) {
    const current = normalizeLabel(textNode.textContent);
    const match = labels.find((label) => current === normalizeLabel(label).toUpperCase());
    if (match && textNode.children.length === 0) {
      textNode.textContent = match;
    }
  }
}

function getGraphClickTargets(node) {
  const ids = new Set([selectedId, ...(node.classes || [])]);
  for (const target of getMermaidClickTargets(node)) {
    if (nodes[target.id]) ids.add(target.id);
  }

  const targets = [...ids]
    .filter((id) => nodes[id])
    .map((id) => ({ id, label: nodes[id].title }));

  for (const target of getMermaidClickTargets(node)) {
    if (nodes[target.id]) targets.push(target);
  }

  return dedupeTargets(targets);
}

function getGraphLabels(node) {
  const labels = new Set();
  if (node?.title) labels.add(node.title);
  for (const target of getGraphClickTargets(node)) labels.add(target.label);

  const labelPattern = /\["([^"]+)"\]/g;
  for (const match of node.graph.matchAll(labelPattern)) {
    labels.add(match[1]);
  }

  return [...labels];
}

function getMermaidClickTargets(node) {
  const labelsByKey = new Map();
  const nodePattern = /(\w+)\["([^"]+)"\]/g;
  for (const match of node.graph.matchAll(nodePattern)) {
    labelsByKey.set(match[1], match[2]);
  }

  const clickTargets = [];
  const clickPattern = /click\s+(\w+)\s+call\s+selectNode\("([^"]+)"\)/g;
  for (const match of node.graph.matchAll(clickPattern)) {
    const label = labelsByKey.get(match[1]) || nodes[match[2]]?.title;
    if (label) clickTargets.push({ id: match[2], label });
  }

  return clickTargets;
}

function dedupeTargets(targets) {
  const seen = new Set();
  return targets.filter((target) => {
    const key = `${target.id}:${target.label}`;
    if (seen.has(key)) return false;
    seen.add(key);
    return true;
  });
}

function isGraphLabelMatch(actual, expected) {
  return normalizeLabel(actual) === normalizeLabel(expected);
}

function normalizeLabel(value) {
  return String(value || "").replace(/\s+/g, " ").trim();
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
  currentCodeMethods = [];
  renderMethodChips([]);

  if (!source) {
    preview.textContent = "시스템 노드에서는 Key Classes를 선택하면 코드 미리보기가 표시됩니다.";
    link.href = node.doc ? repoBase + node.doc : repoBase;
    link.textContent = node.doc ? "Open docs" : "Open repo";
    return;
  }

  link.href = repoBase + source;
  link.textContent = "Open file";

  try {
    const response = await fetch(encodeURI(source));
    if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
    const text = await response.text();
    currentCodeMethods = getGraphMethods(node);
    renderMethodChips(currentCodeMethods);
    renderCodePreview(text);
  } catch (error) {
    preview.textContent = `${source}\n\n코드 미리보기를 불러오지 못했습니다: ${error.message}\nOpen file 링크로 원본을 확인할 수 있습니다.`;
  }
}

function renderMethodChips(methods) {
  const container = document.querySelector("#code-methods");
  container.innerHTML = "";

  for (const method of methods) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "method-chip";
    button.textContent = method.name;
    button.addEventListener("click", () => jumpToMethod(method.name));
    container.append(button);
  }
}

function renderCodePreview(text) {
  const preview = document.querySelector("#code-preview");
  const lines = text.replace(/\r\n/g, "\n").split("\n");
  preview.innerHTML = lines
    .map((line, index) => {
      const lineNumber = index + 1;
      return `<span class="code-line" data-line="${lineNumber}"><span class="line-number">${lineNumber}</span><span class="line-code">${highlightCSharp(line) || " "}</span></span>`;
    })
    .join("");
}

function jumpToMethod(methodName) {
  const preview = document.querySelector("#code-preview");
  const line = findMethodLine(methodName);
  if (!line) return;

  preview.querySelectorAll(".active-line").forEach((item) => item.classList.remove("active-line"));
  const target = preview.querySelector(`[data-line="${line}"]`);
  if (!target) return;

  target.classList.add("active-line");
  target.scrollIntoView({ block: "center", behavior: "smooth" });

  window.setTimeout(() => {
    target.classList.remove("active-line");
  }, 1800);
}

function findMethodLine(methodName) {
  const rows = [...document.querySelectorAll("#code-preview .code-line")];
  const pattern = new RegExp(`\\b${escapeRegExp(methodName)}\\s*(?:<[^>]+>)?\\s*\\(`);

  for (const row of rows) {
    const code = row.querySelector(".line-code")?.textContent || "";
    if (pattern.test(code)) return Number(row.dataset.line);
  }

  return 0;
}

function getGraphMethods(node) {
  if (!node?.graph || !node.graph.trim().startsWith("classDiagram")) return [];

  const methodPattern = /^[ \t]*[+\-#~]\s*[\w<>,_\[\]\s]+\s+([A-Za-z_]\w*)\s*\(/gm;
  const seen = new Set();
  const methods = [];

  for (const match of node.graph.matchAll(methodPattern)) {
    const graphName = match[1];
    const name = normalizeMethodName(graphName);
    if (seen.has(name) || name === node.title) continue;
    seen.add(name);
    methods.push({ name, graphName });
  }

  return methods;
}

function normalizeMethodName(name) {
  return name.replace(/_[A-Z]$/, "");
}

function highlightCSharp(line) {
  const keywordPattern =
    /\b(abstract|as|base|bool|break|case|catch|class|const|continue|default|delegate|do|else|enum|event|false|finally|float|for|foreach|get|if|in|int|interface|internal|is|namespace|new|null|object|out|override|private|protected|public|readonly|return|set|static|string|struct|switch|this|throw|true|try|typeof|using|var|virtual|void|while)\b/;
  const tokenPattern =
    /\/\/.*|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\b\d+(?:\.\d+)?\b|\b[A-Z][A-Za-z0-9_]*\b|\b[A-Za-z_][A-Za-z0-9_]*(?=\s*\()|\b(abstract|as|base|bool|break|case|catch|class|const|continue|default|delegate|do|else|enum|event|false|finally|float|for|foreach|get|if|in|int|interface|internal|is|namespace|new|null|object|out|override|private|protected|public|readonly|return|set|static|string|struct|switch|this|throw|true|try|typeof|using|var|virtual|void|while)\b/g;

  let html = "";
  let cursor = 0;

  for (const match of line.matchAll(tokenPattern)) {
    const token = match[0];
    html += escapeHtml(line.slice(cursor, match.index));
    html += wrapCodeToken(token, keywordPattern);
    cursor = match.index + token.length;
  }

  html += escapeHtml(line.slice(cursor));
  return html;
}

function wrapCodeToken(token, keywordPattern) {
  const escaped = escapeHtml(token);
  if (token.startsWith("//")) return `<span class="tok-comment">${escaped}</span>`;
  if (token.startsWith('"') || token.startsWith("'")) return `<span class="tok-string">${escaped}</span>`;
  if (/^\d/.test(token)) return `<span class="tok-number">${escaped}</span>`;
  if (keywordPattern.test(token)) return `<span class="tok-keyword">${escaped}</span>`;
  if (/^[A-Z]/.test(token)) return `<span class="tok-type">${escaped}</span>`;
  return `<span class="tok-method">${escaped}</span>`;
}

function escapeRegExp(value) {
  return String(value).replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

async function render() {
  const node = nodes[selectedId] || nodes.overview;

  document.querySelector("#scope-label").textContent = node.kind;
  document.querySelector("#graph-title").textContent = node.title;
  document.querySelector("#breadcrumbs").textContent = selectedId === "overview" ? "Overview" : `${node.kind} / ${node.title}`;
  document.querySelector("#detail-kind").textContent = node.kind;
  document.querySelector("#detail-title").textContent = node.title;
  document.querySelector("#detail-summary").textContent = node.summary;
  document.querySelector("#detail-problem").textContent = node.intent;
  document.querySelector("#detail-solution").textContent = node.decision;
  document.querySelector("#detail-final").textContent = node.final;
  document.querySelector("#detail-next").textContent = node.next;
  document.querySelector("#graph-back").disabled = navigationStack.length === 0;

  document.querySelectorAll(".tree-item").forEach((item) => {
    item.classList.toggle("active", item.dataset.nodeId === selectedId);
  });

  renderEvidence(node.evidence || []);
  renderClassList(node.classes || []);
  await renderGraph(node);
  await loadCodePreview(node);
}

function renderEvidence(items) {
  const list = document.querySelector("#evidence-list");
  list.innerHTML = "";

  if (!items.length) {
    list.textContent = "이 클래스는 상위 시스템의 Evidence에서 함께 확인할 수 있습니다.";
    return;
  }

  for (const item of items) {
    const card = document.createElement("button");
    card.type = "button";
    card.className = "evidence-card";
    card.innerHTML = `<img src="${item.src}" alt=""><span>${escapeHtml(item.caption)}</span>`;
    card.addEventListener("click", () => openMediaModal(item));
    list.append(card);
  }
}

function renderClassList(classNames) {
  const classList = document.querySelector("#class-list");
  classList.innerHTML = "";

  for (const className of classNames) {
    const target = nodes[className];
    if (!target) continue;
    const chip = document.createElement("button");
    chip.type = "button";
    chip.className = "chip";
    chip.textContent = target.title;
    chip.addEventListener("click", () => selectNode(className));
    classList.append(chip);
  }
}

function openMediaModal(item) {
  const modal = document.querySelector("#media-modal");
  const image = document.querySelector("#modal-image");
  const caption = document.querySelector("#modal-caption");
  image.src = item.src;
  image.alt = item.caption;
  caption.textContent = item.caption;
  modal.classList.add("open");
  modal.setAttribute("aria-hidden", "false");
}

function closeMediaModal() {
  const modal = document.querySelector("#media-modal");
  const image = document.querySelector("#modal-image");
  modal.classList.remove("open");
  modal.setAttribute("aria-hidden", "true");
  image.removeAttribute("src");
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function initResizableLayout() {
  const shell = document.querySelector("#app-shell");
  if (!shell) return;

  const saved = readSavedLayout();
  if (saved) applyLayout(saved);

  document.querySelectorAll("[data-resizer]").forEach((handle) => {
    handle.addEventListener("pointerdown", (event) => {
      if (window.matchMedia("(max-width: 1100px)").matches) return;

      const type = handle.dataset.resizer;
      const canvas = document.querySelector(".canvas-panel");
      const detail = document.querySelector(".detail-panel");
      const code = document.querySelector(".code-panel");
      const start = {
        x: event.clientX,
        graph: canvas.getBoundingClientRect().width,
        detail: detail.getBoundingClientRect().width,
        code: code.getBoundingClientRect().width,
      };

      handle.classList.add("active");
      document.body.classList.add("resizing");
      handle.setPointerCapture(event.pointerId);

      const move = (moveEvent) => {
        const delta = moveEvent.clientX - start.x;
        const next = { graph: start.graph, detail: start.detail, code: start.code };

        if (type === "graph-detail") {
          next.graph = clamp(start.graph + delta, 260, start.graph + start.detail - 280);
          next.detail = start.graph + start.detail - next.graph;
        } else {
          next.detail = clamp(start.detail + delta, 280, start.detail + start.code - 280);
          next.code = start.detail + start.code - next.detail;
        }

        applyLayout(next);
      };

      const stop = () => {
        handle.classList.remove("active");
        document.body.classList.remove("resizing");
        handle.removeEventListener("pointermove", move);
        handle.removeEventListener("pointerup", stop);
        handle.removeEventListener("pointercancel", stop);
        saveCurrentLayout();
      };

      handle.addEventListener("pointermove", move);
      handle.addEventListener("pointerup", stop);
      handle.addEventListener("pointercancel", stop);
    });

    handle.addEventListener("dblclick", () => {
      localStorage.removeItem("codeMapLayout");
      shell.style.removeProperty("--graph-col");
      shell.style.removeProperty("--detail-col");
      shell.style.removeProperty("--code-col");
    });
  });
}

function applyLayout({ graph, detail, code }) {
  const shell = document.querySelector("#app-shell");
  shell.style.setProperty("--graph-col", `${Math.round(graph)}px`);
  shell.style.setProperty("--detail-col", `${Math.round(detail)}px`);
  shell.style.setProperty("--code-col", `${Math.round(code)}px`);
}

function saveCurrentLayout() {
  const canvas = document.querySelector(".canvas-panel");
  const detail = document.querySelector(".detail-panel");
  const code = document.querySelector(".code-panel");
  const layout = {
    graph: Math.round(canvas.getBoundingClientRect().width),
    detail: Math.round(detail.getBoundingClientRect().width),
    code: Math.round(code.getBoundingClientRect().width),
  };
  localStorage.setItem("codeMapLayout", JSON.stringify(layout));
}

function readSavedLayout() {
  try {
    const saved = JSON.parse(localStorage.getItem("codeMapLayout"));
    if (!saved?.graph || !saved?.detail || !saved?.code) return null;
    return saved;
  } catch {
    return null;
  }
}

function clamp(value, min, max) {
  return Math.min(Math.max(value, min), max);
}

document.querySelector("#reset-view").addEventListener("click", () => selectNode("overview"));
document.querySelector("#graph-back").addEventListener("click", goBack);
document.querySelector("#modal-close").addEventListener("click", closeMediaModal);
document.querySelector("#media-modal").addEventListener("click", (event) => {
  if (event.target.id === "media-modal") closeMediaModal();
});
document.addEventListener("keydown", (event) => {
  if (event.key === "Escape") closeMediaModal();
});

initResizableLayout();
renderTree();
render();
