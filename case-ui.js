const escapeCases = {
  "item-container": {
    number: "01",
    nodeId: "item-container-transaction",
    title: "서로 다른 저장공간의 아이템 이동 규칙",
    card: "인벤토리·창고·루팅·장비창이 서로를 직접 알지 않아도 이동·교체·검증을 같은 흐름으로 처리",
    situation: "창고를 추가하려 할 때 인벤토리·장비·루팅 구조가 서로를 직접 참조하고 있었습니다. 퀵무브나 장비 교체를 붙일수록 여러 클래스를 함께 수정해야 했고, 컨테이너마다 이동 규칙이 달라지는 문제가 생겼습니다.",
    alternatives: "각 컨테이너에 이동 규칙을 계속 추가하는 방법과, 저장공간의 종류보다 아이템 이동의 공통 규칙을 먼저 분리하는 방법을 비교했습니다.",
    decision: "IItemContainer로 읽기·검증·이동·비우기·UI 갱신의 공통 규칙을 정의하고, 크기와 장비 슬롯 제한은 Adapter가 맡게 했습니다. UIItemMoveManager가 요청을 받아 검증·이동·스왑·갱신을 조율합니다.",
    structure: ["UI 이동 요청", "UIItemMoveManager", "IItemContainer", "컨테이너별 Adapter", "검증·이동·스왑", "UI 갱신"],
    implementation: "IItemContainer는 슬롯 조회·검증·이동의 공통 계약을 제공하고, 인벤토리·창고·장비·루팅 컨테이너는 Adapter로 각 규칙을 적용합니다. UIItemMoveManager는 이동 요청의 검증과 갱신 순서를 조율합니다.",
    result: "인벤토리·창고·루팅·장비창의 CRUD와 장비 교체를 하나의 이동 흐름으로 다룰 수 있게 했습니다. 퀵무브는 열린 루팅·창고와 장비 가능 여부를 확인한 뒤 목적지를 정하도록 구성했습니다.",
    feedback: "목적지 우선순위는 아직 UIItemMoveManager에 남아 있습니다. 다시 확장한다면 이동 목적지 선택을 Policy 객체로 분리하고, 스택 병합·실패 복구까지 포함한 트랜잭션 경계를 더 명확히 두겠습니다.",
    cta: "CodeMap · 컨테이너 Adapter와 이동 검증 구조 보기"
  },
  "crafting-tree": {
    number: "02",
    nodeId: "recursive-crafting-tree",
    title: "목표 장비를 위한 재귀 제작 트리",
    card: "상위 장비 하나를 선택하면 최하위 재료까지의 관계와 보유 수량을 한 번에 확인",
    situation: "제작을 재료 소비 UI로만 만들면 플레이어가 다음 탐험에서 무엇을 챙겨야 하는지 알기 어려웠습니다. 상위 장비를 보면서 필요한 하위 재료까지 함께 확인하는 목표 경험이 필요했습니다.",
    alternatives: "재료를 조회하는 동안 UI를 단계별로 갱신하는 방법과, 재료 관계를 먼저 완성한 뒤 한 번에 출력하는 방법을 비교했습니다.",
    decision: "CraftTreeBuilder가 ScriptableObject 레시피를 재귀 탐색해 완성된 CraftTreeNode를 만들고, CraftTreeRenderer는 완성된 트리만 출력하도록 책임을 나눴습니다. 실제 제작은 CraftingService가 인벤토리와 창고 수량을 확인한 뒤 처리합니다.",
    structure: ["상위 아이템 선택", "CraftRecipe", "CraftTreeBuilder 재귀 탐색", "CraftTreeNode 완성", "CraftTreeRenderer", "CraftingService"],
    implementation: "CraftTreeBuilder가 레시피와 하위 재료를 재귀적으로 완성한 뒤 CraftTreeNode를 반환하고, CraftTreeRenderer가 완성된 결과만 UI로 변환합니다. CraftingService는 별도로 재료·공간을 검증한 뒤 차감과 지급을 처리합니다.",
    result: "플레이어가 상위 목표와 최하위 재료, 현재 보유 수량을 같은 화면에서 확인하고 중간 재료를 직접 만들어 올라갈 수 있는 제작 흐름을 구성했습니다.",
    feedback: "현재는 조회마다 트리와 UI를 새로 만듭니다. 결과물을 넣을 공간이 없을 때 재료 차감까지 완전히 되돌리는 제작 트랜잭션도 부족해, 데이터가 커지면 캐싱과 명시적인 롤백 경계가 필요합니다.",
    cta: "CodeMap · 재귀 제작 트리와 제작 처리 구조 보기"
  },
  "zone-culling": {
    number: "03",
    nodeId: "zone-culling",
    title: "Region 기반 Zone 활성화",
    card: "현재 Region과 인접 Region만 유지해 맵·몬스터·상자의 불필요한 활성 상태를 줄인 구조",
    situation: "맵 전체와 몬스터·상자가 계속 활성 상태로 남아 있으면 전투와 파밍이 있어도 플레이가 답답해질 수 있다고 봤습니다. 실제 씬의 잔렉을 줄이기 위해, 플레이어 주변에 필요한 Zone만 유지할 필요가 있었습니다.",
    alternatives: "모든 Zone을 계속 활성화하는 방식과, 플레이어 위치를 기준으로 현재·인접 Region만 계산해 활성 상태를 갱신하는 방식을 비교했습니다.",
    decision: "PlayerRegionTracker가 바닥 Raycast로 Region 변화를 감지하면, ZoneController가 RegionGraphSO에서 현재·인접 Region을 조회합니다. 기존과 다음 활성 집합의 차이를 HashSet으로 계산해 필요한 Zone만 SetActive합니다.",
    structure: ["PlayerRegionTracker", "바닥 Raycast", "OnRegionChanged", "RegionGraphSO", "HashSet 차집합", "Zone SetActive"],
    implementation: "PlayerRegionTracker가 Region 변경 이벤트를 보내면 ZoneController가 RegionGraphSO의 인접 정보를 조회합니다. 현재 활성 집합과 다음 집합의 차이만 계산해 필요한 Zone에만 SetActive를 호출했습니다.",
    result: "동일 게임 씬 개발 환경에서 전체 Zone 활성 상태와 비교해 Update CPU 측정값이 약 4.75ms에서 2.75ms로 줄었습니다. 플레이 중에도 불필요한 Zone을 계속 활성화한 경우보다 끊김이 줄어드는 것을 확인했습니다.",
    feedback: "RegionGraph를 사람이 직접 설정해야 하는 수작업이 남아 있습니다. 다음에는 맵 데이터에서 인접 관계를 추출하거나 검증하는 편집 도구로 그래프 설정을 보완하겠습니다.",
    cta: "CodeMap · RegionGraph와 Zone 활성 집합 계산 보기"
  }
};

document.addEventListener("DOMContentLoaded", () => {
  const hub = document.getElementById("case-hub");
  const cards = document.getElementById("case-cards");
  const detail = document.getElementById("case-detail-view");
  const shell = document.getElementById("app-shell");
  if (!hub || !cards || !detail || !shell) return;

  const setHash = (value) => {
    const next = value ? `#${value}` : "";
    if (window.location.hash !== next) history.replaceState(null, "", next || window.location.pathname);
  };

  const updateNavigation = (view) => {
    document.querySelectorAll("[data-app-view]").forEach((button) => {
      button.classList.toggle("is-active", button.dataset.appView === view);
    });
  };

  const renderCards = () => {
    cards.innerHTML = "";
    Object.entries(escapeCases).forEach(([id, item]) => {
      const button = document.createElement("button");
      button.type = "button";
      button.className = "case-card";
      button.innerHTML = `<span class="case-number">${item.number}</span><h3>${escapeHtml(item.title)}</h3><p>${escapeHtml(item.card)}</p><span class="case-card-link">사례 읽기 <span aria-hidden="true">→</span></span>`;
      button.addEventListener("click", () => selectCase(id));
      cards.append(button);
    });
  };

  const renderDetail = (item) => {
    const flow = item.structure.map((step, index) => `<li><span>${String(index + 1).padStart(2, "0")}</span>${escapeHtml(step)}</li>`).join("");
    detail.innerHTML = `
      <div class="case-detail-inner">
        <button class="case-back" type="button" data-app-view="cases">← 사례 목록</button>
        <p class="eyebrow">CASE ${item.number}</p>
        <h2>${escapeHtml(item.title)}</h2>
        <p class="case-lead">${escapeHtml(item.card)}</p>
        <div class="case-story-grid">
          <article><h3>상황</h3><p>${escapeHtml(item.situation)}</p></article>
          <article><h3>검토한 방식</h3><p>${escapeHtml(item.alternatives)}</p></article>
          <article class="case-decision"><h3>선택한 방식</h3><p>${escapeHtml(item.decision)}</p></article>
        </div>
        <section class="case-flow"><h3>구조·구현 흐름</h3><ol>${flow}</ol></section>
        <section class="case-implementation"><h3>구현과 근거</h3><p>${escapeHtml(item.implementation)}</p></section>
        <section class="case-feedback case-result-panel"><h3>확인한 결과</h3><p>${escapeHtml(item.result)}</p></section>
        <section class="case-feedback"><h3>자체 피드백</h3><p>${escapeHtml(item.feedback)}</p></section>
        <button class="case-cta" type="button" data-case-node="${escapeHtml(item.nodeId)}">${escapeHtml(item.cta)} <span aria-hidden="true">↓</span></button>
      </div>`;
    detail.querySelector("[data-app-view='cases']").addEventListener("click", () => setAppView("cases"));
    detail.querySelector("[data-case-node]").addEventListener("click", (event) => {
      const nodeId = event.currentTarget.dataset.caseNode;
      setAppView("systems", { updateUrl: false });
      if (typeof window.selectNode === "function") window.selectNode(nodeId, { push: false });
      document.getElementById("graph-wrap").scrollIntoView({ behavior: "smooth", block: "start" });
    });
  };

  const setAppView = (view, { updateUrl = true } = {}) => {
    const casesView = view === "cases";
    hub.hidden = !casesView;
    detail.hidden = true;
    shell.hidden = casesView;
    updateNavigation(view);
    if (updateUrl) setHash(casesView ? "cases" : view === "code" ? "code" : "systems");
    if (!casesView && typeof window.selectNode === "function") window.selectNode("overview", { push: false });
  };

  const selectCase = (id, { updateUrl = true } = {}) => {
    const item = escapeCases[id];
    if (!item) return;
    hub.hidden = true;
    detail.hidden = false;
    shell.hidden = false;
    renderDetail(item);
    updateNavigation("cases");
    if (updateUrl) setHash(`case=${encodeURIComponent(id)}`);
    if (typeof window.selectNode === "function") window.selectNode(item.nodeId, { push: false });
    window.scrollTo({ top: 0, behavior: "auto" });
  };

  const openRoute = () => {
    const hash = decodeURIComponent(window.location.hash.replace(/^#/, ""));
    if (hash.startsWith("case=")) {
      selectCase(hash.slice(5), { updateUrl: false });
      return;
    }
    if (hash === "systems" || hash === "code") {
      setAppView(hash, { updateUrl: false });
      return;
    }
    setAppView("cases", { updateUrl: false });
  };

  document.querySelectorAll("[data-app-view]").forEach((button) => {
    button.addEventListener("click", () => setAppView(button.dataset.appView));
  });

  renderCards();
  openRoute();
  window.addEventListener("hashchange", openRoute);
});
