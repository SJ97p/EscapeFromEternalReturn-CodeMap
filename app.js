const repoBase = "https://github.com/sj97p/EscapeFromEternalReturn-CodeMap/blob/main/";

const nodes = {
  overview: {
    kind: "Overview",
    title: "전체 시스템 구조",
    summary:
      "Escape From Eternal Return은 지역 이동, 파밍, 제작, 전투, 몬스터 AI, 데이터 로딩이 동시에 맞물리는 탑다운 생존 액션 RPG 프로젝트입니다.",
    problem:
      "생존 액션 RPG는 지역 상태, 아이템 이동, 플레이어 전투, AI, 데이터 접근이 서로 직접 의존하면 작은 변경도 전체 런타임 흐름에 영향을 줍니다.",
    solution:
      "Region, Inventory, Crafting, Player Combat, Monster AI, Data Repository를 독립된 시스템으로 나누고 공통 enum, adapter, event, repository 계층으로 연결했습니다.",
    classes: ["ZoneController", "Inventory", "CraftingService", "PlayerFSM", "BehaviorTree", "GameDataStore"],
    graph: `flowchart TD
      region["Restricted Zone / Region"]
      hyperloop["Hyperloop Travel"]
      inventory["Inventory / Storage"]
      crafting["Crafting Tree"]
      player["Player Combat"]
      monster["Monster / Boss AI"]
      data["Data Repository"]
      fog["Fog of War"]

      data --> inventory
      data --> monster
      region --> hyperloop
      region --> monster
      inventory --> crafting
      inventory --> player
      player --> monster
      fog --> player
      fog --> monster

      click region call selectNode("restricted-zone")
      click hyperloop call selectNode("hyperloop-travel")
      click inventory call selectNode("inventory-storage")
      click crafting call selectNode("crafting-tree")
      click player call selectNode("player-combat")
      click monster call selectNode("monster-ai")
      click data call selectNode("data-repository")`,
  },
  "restricted-zone": {
    kind: "System",
    title: "Restricted Zone & Region",
    summary:
      "플레이어 현재 지역과 인접 지역을 기준으로 Zone 활성화 범위를 관리하고, 시간 기반 금지구역 상태를 전파하는 시스템입니다.",
    problem:
      "지역 상태, 금지구역, 스폰, UI 알림이 각자 지역을 판단하면 실제 월드 상태와 표시가 쉽게 어긋납니다.",
    solution:
      "RegionGraphSO와 ZoneController로 지역 기준을 통일하고, RestrictedZoneController는 경고/확정 상태를 이벤트로 알리도록 나눴습니다.",
    doc: "docs/systems/restricted-zone.md",
    classes: ["ZoneController", "RestrictedZoneController", "RegionGraph", "Zone", "PlayerRegionTracker"],
    graph: `flowchart TD
      tracker["PlayerRegionTracker"]
      graph["RegionGraphSO"]
      controller["ZoneController"]
      active["Active Regions"]
      zone["Zone"]
      restricted["RestrictedZoneController"]
      state["Warning / Restricted State"]

      tracker --> controller
      graph --> controller
      controller --> active --> zone
      restricted --> state --> controller

      click controller call selectNode("ZoneController")
      click restricted call selectNode("RestrictedZoneController")`,
  },
  "hyperloop-travel": {
    kind: "System",
    title: "Hyperloop Travel",
    summary:
      "월드의 하이퍼루프 오브젝트가 지역 정보와 UI 진입만 담당하고 실제 이동 정책은 씬/UI 컨트롤러로 위임합니다.",
    problem:
      "지역 이동 오브젝트가 씬 전환, 탈출 가능 여부, 목적지 UI를 모두 직접 처리하면 이동 규칙이 여러 곳으로 흩어집니다.",
    solution:
      "Hyperloop은 Region과 Interact만 갖고, InGameSceneController와 NewUIManager를 통해 탈출/이동 UI 흐름을 연결합니다.",
    doc: "docs/systems/hyperloop-travel.md",
    classes: ["Hyperloop", "HyperloopRegionUI", "InGameSceneController", "NewUIManager"],
    graph: `flowchart LR
      player["Player Interact"]
      hyperloop["Hyperloop"]
      scene["InGameSceneController"]
      ui["NewUIManager"]
      panel["Hyperloop UI"]

      player --> hyperloop
      hyperloop --> scene
      hyperloop --> ui --> panel

      click hyperloop call selectNode("Hyperloop")`,
  },
  "inventory-storage": {
    kind: "System",
    title: "Inventory & Storage",
    summary:
      "인벤토리, 장비, 보관함, 루팅 컨테이너를 공통 컨테이너 조작 흐름으로 묶는 아이템 이동 시스템입니다.",
    problem:
      "드래그, 퀵무브, 장착, 루팅 로직이 UI마다 따로 있으면 같은 아이템 이동인데도 예외 처리가 중복됩니다.",
    solution:
      "IItemContainer와 Adapter, DragDrop/Move/Use 매니저를 통해 UI 이벤트와 실제 컨테이너 변경을 분리했습니다.",
    doc: "docs/systems/inventory-storage.md",
    classes: ["Inventory", "Storage", "IItemContainer", "UIDragDropManager", "InventoryEventBus"],
    graph: `flowchart TD
      view["Slot View"]
      drag["UIDragDropManager"]
      payload["UIDragPayload"]
      container["IItemContainer"]
      inventory["Inventory"]
      storage["Storage"]
      event["InventoryEventBus"]

      view --> drag --> payload --> container
      container --> inventory --> event
      container --> storage

      click inventory call selectNode("Inventory")
      click storage call selectNode("Storage")`,
  },
  "crafting-tree": {
    kind: "System",
    title: "Crafting Tree",
    summary:
      "제작 트리 표시와 제작 가능 여부, 부족 재료 계산, 재료 차감/결과 지급을 분리한 제작 시스템입니다.",
    problem:
      "제작 가능 여부와 트리 표시가 UI에 섞이면 워크벤치, 검색 테이블, 디버그 제작에서 같은 규칙을 재사용하기 어렵습니다.",
    solution:
      "CraftTreeBuilder/Renderer는 표현을 담당하고 CraftingService는 재료 계산과 제작 실행을 담당하도록 분리했습니다.",
    doc: "docs/systems/crafting-tree.md",
    classes: ["CraftingService", "CraftTreeBuilder", "CraftTreeNode", "CraftingStorageAdapter"],
    graph: `flowchart TD
      recipe["CraftRecipeDatabase"]
      builder["CraftTreeBuilder"]
      node["CraftTreeNode"]
      renderer["CraftTreeRenderer"]
      service["CraftingService"]
      adapter["CraftingStorageAdapter"]

      recipe --> builder --> node
      node --> renderer
      node --> service --> adapter

      click service call selectNode("CraftingService")`,
  },
  "player-combat": {
    kind: "System",
    title: "Player Combat",
    summary:
      "플레이어 FSM, 스킬 캐스팅, 타겟 쿼리, 상태이상 효과를 나누어 전투 상태 충돌을 줄인 구조입니다.",
    problem:
      "이동, 공격, 스킬, 스턴, 넉백, 사망이 동시에 발생하면 상태 우선순위가 없을 때 입력과 액션이 꼬입니다.",
    solution:
      "PlayerFSM은 점유 상태와 우선순위로 전이를 제한하고, SkillCaster는 캐스팅 중 이동 잠금과 종료 대기를 담당합니다.",
    doc: "docs/systems/player-combat.md",
    classes: ["PlayerFSM", "SkillCaster", "SkillBase", "ISkillTargetQuery", "HitResolver"],
    graph: `flowchart TD
      input["InputEventBus"]
      fsm["PlayerFSM"]
      state["IPlayerState"]
      caster["SkillCaster"]
      skill["SkillBase"]
      query["ISkillTargetQuery"]
      target["IDamageable"]

      input --> fsm --> state --> caster
      caster --> skill --> query --> target

      click fsm call selectNode("PlayerFSM")
      click caster call selectNode("SkillCaster")`,
  },
  "monster-ai": {
    kind: "System",
    title: "Monster / Boss AI",
    summary:
      "일반 몬스터와 보스 패턴을 상태/행동 트리/전략/스킬 슬롯/콤보 실행기로 나누어 구성한 AI 시스템입니다.",
    problem:
      "보스 페이즈, 스킬 쿨타임, 콤보, 추적/복귀 조건을 큰 조건문 하나로 처리하면 패턴 추가가 어려워집니다.",
    solution:
      "BehaviorTree와 Strategy가 판단을 담당하고, BossMonsterSkillManager와 BossComboExecutor가 스킬 실행을 담당합니다.",
    doc: "docs/systems/monster-ai.md",
    classes: ["MonsterController", "BehaviorTree", "BossComboExecutor", "BossMonsterSkillManager"],
    graph: `flowchart TD
      controller["BossMonsterController"]
      tree["BehaviorTree"]
      selector["Selector / Sequence"]
      strategy["IStrategy"]
      manager["BossMonsterSkillManager"]
      combo["BossComboExecutor"]
      action["BossCombatAction"]

      controller --> tree --> selector --> strategy
      strategy --> manager
      strategy --> combo --> action

      click tree call selectNode("BehaviorTree")
      click combo call selectNode("BossComboExecutor")
      click controller call selectNode("MonsterController")`,
  },
  "data-repository": {
    kind: "System",
    title: "Data Repository",
    summary:
      "SQLite/ScriptableObject 기반 데이터를 Repository와 GameDataStore로 감싸 런타임 시스템의 데이터 접근을 단순화합니다.",
    problem:
      "런타임 코드가 DB 연결명과 테이블 구조를 직접 알면 데이터 변경이 게임 로직 곳곳으로 퍼집니다.",
    solution:
      "DBLoader와 GameRepositories가 DB 접근을 만들고, GameDataStore는 ScriptableObject 참조 허브 역할을 맡습니다.",
    doc: "docs/systems/data-repository.md",
    classes: ["GameRepositories", "GameDataStore", "DBLoader", "ItemRepository", "StorageRepository"],
    graph: `flowchart TD
      bootstrap["DataBootstrapper"]
      loader["DBLoader"]
      repos["GameRepositories"]
      item["ItemRepository"]
      storage["StorageRepository"]
      save["SaveFileRepository"]
      store["GameDataStore"]
      so["ScriptableObject DB"]

      bootstrap --> loader --> repos
      repos --> item
      repos --> storage
      repos --> save
      store --> so

      click store call selectNode("GameDataStore")
      click repos call selectNode("GameRepositories")`,
  },
  ZoneController: classNode("ZoneController", "지역과 Zone GameObject를 연결하고 현재/인접 지역만 활성화하는 컨트롤러입니다.", "src/Assets/00_Scripts/ZoneControllers/ZoneController.cs", ["Restricted Zone & Region"]),
  RestrictedZoneController: classNode("RestrictedZoneController", "시간 기반으로 경고 지역과 금지구역을 선택해 이벤트로 전파하는 컨트롤러입니다.", "src/Assets/00_Scripts/ZoneControllers/RestrictedZoneController.cs", ["Restricted Zone & Region"]),
  Hyperloop: classNode("Hyperloop", "지역 이동 상호작용과 하이퍼루프 UI 진입을 연결하는 월드 오브젝트입니다.", "src/Assets/00_Scripts/Hyperloop/Hyperloop.cs", ["Hyperloop Travel"]),
  Inventory: classNode("Inventory", "슬롯 배열 기반의 인벤토리 데이터와 기본 아이템 이동 연산을 담당합니다.", "src/Assets/00_Scripts/Inventory_Scripts/InventoryLogic/Inventory.cs", ["Inventory & Storage"]),
  Storage: classNode("Storage", "보관함/루팅 컨테이너의 슬롯 데이터를 관리하는 아이템 컨테이너입니다.", "src/Assets/00_Scripts/Storage_Scripts/StorageLogic/Storage.cs", ["Inventory & Storage"]),
  CraftingService: classNode("CraftingService", "필요 재료 계산, 제작 가능 여부, 부족 재료, 실제 제작 처리를 담당합니다.", "src/Assets/00_Scripts/Craft/CraftingService.cs", ["Crafting Tree"]),
  PlayerFSM: classNode("PlayerFSM", "플레이어 상태 객체를 보유하고 우선순위 기반 상태 전이를 수행합니다.", "src/Assets/00_Scripts/Player/Core/PlayerFSM.cs", ["Player Combat"]),
  SkillCaster: classNode("SkillCaster", "스킬 캐스팅 중 이동 잠금, 애니메이션 이벤트, 종료 대기와 취소를 처리합니다.", "src/Assets/00_Scripts/Player/Skill/SkillCaster.cs", ["Player Combat"]),
  MonsterController: classNode("MonsterController", "일반 몬스터의 이동, 전투, 감지, 애니메이션 흐름을 묶는 컨트롤러입니다.", "src/Assets/00_Scripts/Monster/MonsterController.cs", ["Monster / Boss AI"]),
  BehaviorTree: classNode("BehaviorTree", "보스 AI 행동 트리의 루트 노드로 자식 노드를 순차 평가합니다.", "src/Assets/00_Scripts/Monster/BossMonster/BT/BehaviorTree.cs", ["Monster / Boss AI"]),
  BossComboExecutor: classNode("BossComboExecutor", "보스 스킬과 중첩 콤보 액션을 순차 실행하는 코루틴 실행기입니다.", "src/Assets/00_Scripts/Monster/BossMonster/BossSkill/Combo/BossComboExecutor.cs", ["Monster / Boss AI"]),
  GameDataStore: classNode("GameDataStore", "ScriptableObject 데이터베이스와 공용 프리팹 참조를 제공하는 데이터 허브입니다.", "src/Assets/00_Scripts/DataBase/GameDataStore.cs", ["Data Repository"]),
  GameRepositories: classNode("GameRepositories", "DBLoader에서 연결을 받아 도메인별 Repository를 생성하는 팩토리성 객체입니다.", "src/Assets/00_Scripts/DataBase/GameRepositories.cs", ["Data Repository"]),
};

function classNode(title, summary, source, related) {
  return {
    kind: "Class",
    title,
    summary,
    problem: "이 클래스가 맡은 책임이 다른 시스템에 섞이면 변경 범위가 커지고 테스트하기 어려워집니다.",
    solution: "명확한 입력과 출력, 이벤트 또는 어댑터를 통해 주변 시스템과 연결되도록 책임을 제한했습니다.",
    source,
    classes: related,
    graph: `flowchart TD
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
      ["restricted-zone", "Restricted Zone"],
      ["hyperloop-travel", "Hyperloop Travel"],
      ["inventory-storage", "Inventory / Storage"],
      ["crafting-tree", "Crafting Tree"],
      ["player-combat", "Player Combat"],
      ["monster-ai", "Monster / Boss AI"],
      ["data-repository", "Data Repository"],
    ],
  },
  {
    title: "Classes",
    items: [
      ["ZoneController", "ZoneController"],
      ["Hyperloop", "Hyperloop"],
      ["Inventory", "Inventory"],
      ["Storage", "Storage"],
      ["CraftingService", "CraftingService"],
      ["PlayerFSM", "PlayerFSM"],
      ["SkillCaster", "SkillCaster"],
      ["BehaviorTree", "BehaviorTree"],
      ["GameDataStore", "GameDataStore"],
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
      button.addEventListener("click", () => selectNode(id));
      wrapper.append(button);
    }

    tree.append(wrapper);
  }
}

async function renderGraph(node) {
  const graph = document.querySelector("#graph");
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
    preview.textContent = text.split("\n").slice(0, 90).join("\n");
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
    item.classList.toggle("active", item.textContent === node.title || item.textContent === selectedId);
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
