document.addEventListener("DOMContentLoaded", () => {
  const shell = document.getElementById("app-shell");
  const tree = document.getElementById("tree");
  const sidebarHead = document.querySelector(".sidebar-head");
  const graph = document.getElementById("graph");
  const graphNav = document.querySelector(".graph-nav");
  const codeSplitter = document.querySelector('[data-resizer="detail-code"]');
  if (!shell || !tree || !sidebarHead || !graph) return;

  const search = document.createElement("input");
  search.type = "search";
  search.className = "explorer-search";
  search.placeholder = "모듈 검색...";
  search.setAttribute("aria-label", "모듈 검색");
  sidebarHead.appendChild(search);

  const themeButton = document.createElement("button");
  themeButton.type = "button";
  themeButton.textContent = "Black";
  themeButton.title = "Toggle theme";
  document.querySelector(".top-actions")?.appendChild(themeButton);
  themeButton.addEventListener("click", () => {
    const enabled = document.body.classList.toggle("dark-theme");
    themeButton.textContent = enabled ? "White" : "Black";
  });

  const expanded = new Set();
  const enhanceTree = () => {
    tree.querySelectorAll(".tree-group").forEach((group) => {
      if (group.dataset.enhanced) return;
      group.dataset.enhanced = "true";
      const oldTitle = group.querySelector(".tree-title");
      if (!oldTitle) return;
      const title = document.createElement("button");
      title.type = "button";
      title.className = "tree-title";
      title.textContent = oldTitle.textContent;
      title.setAttribute("aria-label", oldTitle.textContent.trim());
      title.setAttribute("aria-expanded", "false");
      oldTitle.replaceWith(title);
      const items = document.createElement("div");
      items.className = "tree-group-items";
      items.hidden = true;
      [...group.querySelectorAll(":scope > .tree-item")].forEach((item) => items.appendChild(item));
      group.appendChild(items);
      title.addEventListener("click", () => {
        const key = title.textContent.trim();
        expanded.has(key) ? expanded.delete(key) : expanded.add(key);
        applyFilter();
      });
    });
  };

  const applyFilter = () => {
    enhanceTree();
    const query = search.value.trim().toLocaleLowerCase("ko-KR");
    let matches = 0;
    tree.querySelectorAll(".tree-group").forEach((group) => {
      const title = group.querySelector(".tree-title");
      const items = group.querySelector(".tree-group-items");
      const key = title.textContent.trim();
      const groupMatch = key.toLocaleLowerCase("ko-KR").includes(query);
      let groupMatches = 0;
      items.querySelectorAll(".tree-item").forEach((item) => {
        const visible = !query || groupMatch || item.textContent.toLocaleLowerCase("ko-KR").includes(query);
        item.hidden = !visible;
        if (visible) groupMatches += 1;
      });
      group.hidden = groupMatches === 0;
      const open = Boolean(query) || expanded.has(key);
      items.hidden = !open;
      title.setAttribute("aria-expanded", String(open));
      matches += groupMatches;
    });
    let empty = tree.querySelector(".tree-empty");
    if (!matches && !empty) {
      empty = document.createElement("p");
      empty.className = "tree-empty";
      empty.textContent = "일치하는 모듈이 없습니다.";
      tree.appendChild(empty);
    } else if (matches && empty) empty.remove();
  };

  search.addEventListener("input", applyFilter);
  applyFilter();

  let scale = 1;
  let zoomResetButton = null;
  const applyScale = () => {
    graph.style.transform = `scale(${scale})`;
    graph.style.width = `${100 / scale}%`;
    if (zoomResetButton) zoomResetButton.textContent = `${Math.round(scale * 100)}%`;
  };
  if (graphNav) {
    const zoom = document.createElement("div");
    zoom.className = "graph-zoom";
    const makeButton = (label, title, action) => {
      const button = document.createElement("button");
      button.type = "button";
      button.textContent = label;
      button.title = title;
      button.addEventListener("click", action);
      return button;
    };
    const zoomOutButton = makeButton("-", "Zoom out", () => { scale = Math.max(.55, scale - .15); applyScale(); });
    zoomResetButton = makeButton("100%", "Reset zoom", () => { scale = 1; applyScale(); });
    const zoomInButton = makeButton("+", "Zoom in", () => { scale = Math.min(2, scale + .15); applyScale(); });
    zoom.append(zoomOutButton, zoomResetButton, zoomInButton);
    graphNav.appendChild(zoom);
  }

  new MutationObserver(() => requestAnimationFrame(applyScale)).observe(graph, { childList: true });

  if (codeSplitter) {
    const cleanSplitter = codeSplitter.cloneNode(true);
    codeSplitter.replaceWith(cleanSplitter);
    const setWidth = (width) => {
      const maximum = Math.min(820, window.innerWidth * .62);
      shell.style.setProperty("--code-panel-width", `${Math.round(Math.max(320, Math.min(maximum, width)))}px`);
    };
    cleanSplitter.addEventListener("pointerdown", (event) => {
      if (!matchMedia("(min-width:1281px)").matches) return;
      cleanSplitter.classList.add("active");
      cleanSplitter.setPointerCapture(event.pointerId);
    });
    cleanSplitter.addEventListener("pointermove", (event) => {
      if (!cleanSplitter.classList.contains("active")) return;
      setWidth(window.innerWidth - event.clientX);
    });
    const stop = (event) => {
      cleanSplitter.classList.remove("active");
      if (cleanSplitter.hasPointerCapture(event.pointerId)) cleanSplitter.releasePointerCapture(event.pointerId);
    };
    cleanSplitter.addEventListener("pointerup", stop);
    cleanSplitter.addEventListener("pointercancel", stop);
    cleanSplitter.addEventListener("dblclick", () => setWidth(460));
  }
});
