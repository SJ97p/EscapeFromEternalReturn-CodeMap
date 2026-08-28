# Escape From Eternal Return Code Map

![Escape From Eternal Return title](assets/evidence/escape-from-eternal-return-title.png)

Unity 기반 생존 액션 RPG 프로젝트 **Escape From Eternal Return**에서 제가 담당한 런타임 시스템 구조를 정리한 포트폴리오용 Code Map입니다.

이 저장소는 전체 Unity 프로젝트를 공개하기보다, 기능을 하나 더 붙일 때마다 수정 범위가 넓어지던 문제를 어떻게 줄였는지 보여주기 위해 만들었습니다. 제가 설계·구현한 **제작 트리, 아이템 컨테이너, SQLite 저장/로드, RegionGraph 기반 런타임 Zone 활성화** 구조를 중심으로 설명합니다.

- [Interactive GitHub Pages](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/)
- [Architecture Overview](docs/architecture.md)
- [Class Diagram](docs/class-diagram.md)
- [Source Snapshot](src/Assets/00_Scripts)

## 바로가기

[![인터랙티브 코드맵](assets/navigation/code-map-link.svg)](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/)

> 이미지를 클릭하면 Escape From Eternal Return의 시스템 구조와 기술 문서로 이동합니다.
## Project Summary

| Item | Description |
|---|---|
| Project | Escape From Eternal Return |
| Development Period | 2026.04.17 ~ 2026.05.26 |
| Engine / Language | Unity / C# |
| Team | 3인 팀 프로젝트 |
| My Focus | Runtime Architecture, Scene/UI Flow, Inventory Transaction, SQLite Persistence, Region Culling |
| Portfolio Goal | 기능 나열보다 설계 의도, 구조적 고민, 결과 증거를 보여주는 것 |

## My Role

이번 프로젝트에서 가장 크게 마주한 문제는, 기존에 분리되어 만들어진 장비창·인벤토리·루팅창에 창고를 추가하려 할 때였습니다. 한 기능을 고칠 때 참조해야 하는 코드가 너무 많았고, 빠른 기능 추가가 오히려 다음 변경을 어렵게 만들고 있었습니다. 그래서 개별 기능보다 **기능들이 서로 안정적으로 연결되고 확장되는 구조**를 만드는 데 집중했습니다.

- 모든 씬이 공통 `SceneController` 라이프사이클을 따르도록 설계
- `GameSceneManager`와 `SceneEnterContext`로 씬 전환과 데이터 전달 중앙화
- `UIPanelId` 기반 UI 레지스트리로 패널 Open/Close/Toggle 규격 통일
- `CraftTreeBuilder` 기반 재귀 제작 트리 생성 구조 구현
- 인벤토리, 창고, 장비창, 루팅창을 `IItemContainer`와 Adapter로 통합
- `UIItemMoveManager`로 이동, 병합, 스왑, 장비 검증, 자동 루팅 흐름 중앙 처리
- 런타임 `Storage`와 저장용 `StorageData`를 분리하고 SQLite Repository로 저장/로드 처리
- `RegionGraph`, `PlayerRegionTracker`, `ZoneController` 기반 Zone Culling 구조 구현

## Visual Evidence

### 1. Scene Lifecycle

![Scene Lifecycle](assets/evidence/scene-lifecycle.gif)

씬 전환 흐름을 `Exit -> LoadSceneAsync -> Initialize(context) -> Enter` 순서로 통일해, 씬이 늘어나도 동일한 진입/종료 규칙을 유지하도록 구성했습니다.

### 2. UIPanel Registry

![UI Panel Registry](assets/evidence/ui-panel-registry.gif)

각 UI 버튼은 직접 패널을 참조하지 않고 `UIPanelId`만 전달하며, `NewUIManager`가 공통 `Open`, `Close`, `Toggle` 흐름으로 패널 상태를 제어합니다.

### 3. Recursive Crafting Tree

![Recursive Crafting Tree](assets/evidence/crafting-tree.gif)

최상위 아이템을 선택했을 때 최하위 재료까지 한 번에 보여주기 위해, ScriptableObject 레시피 데이터를 먼저 재귀 트리로 완성한 뒤 UI가 이를 출력하도록 분리했습니다.

### 4. Item Container Transaction

![Item Container Transaction](assets/evidence/item-container-transaction.gif)

서로 다른 저장소를 Adapter로 공통 컨테이너 규격에 맞추고, 이동 요청은 `UIItemMoveManager`에서 검증 후 Commit되도록 구성해 데이터 무결성을 우선했습니다.

### 5. RegionGraph Zone Culling

![Zone Culling](assets/evidence/zone-culling.gif)

`PlayerRegionTracker`가 현재 Region을 감지하면, `ZoneController`가 `RegionGraph`를 기준으로 현재 지역과 인접 지역만 런타임에서 활성 상태로 유지합니다. 이는 렌더링 컬링이 아니라 해당 Region GameObject의 `SetActive`를 제어하는 구조입니다.

### 6. CPU Optimization Result

![Zone CPU Result](assets/evidence/zone-cpu-result.png)

에디터 환경에서 Zone 활성 범위를 제한한 뒤 Update CPU 측정값이 약 **4.75ms에서 2.75ms**로 감소했습니다. 이 수치는 해당 장면과 환경에서 확인한 비교 결과입니다.

## Core Systems

| System | Design Intent | Result |
|---|---|---|
| Scene Lifecycle & UI Registry | 씬 전환과 UI 호출 흐름을 씬/패널마다 흩어지지 않게 중앙화 | `SceneController`, `GameSceneManager`, `UIPanelId`, `NewUIManager` |
| Recursive Crafting Tree | ScriptableObject 레시피로 전체 재료 트리를 먼저 만들고 UI는 완성된 결과를 출력 | `CraftTreeBuilder`, `CraftTreeNode`, `CraftingService` |
| Item Container Transaction | 인벤토리, 창고, 장비창, 루팅창의 공통 슬롯 조작 규격 정의 | `IItemContainer`, Adapter, `UIItemMoveManager` |
| SQLite Persistence | 런타임 슬롯 모델과 저장 DTO를 분리해 세이브 슬롯별 저장/로드 처리 | `Storage`, `StorageData`, `StorageRepository`, `DBLoader` |
| RegionGraph Zone Culling | 현재 지역과 인접 지역만 `SetActive`로 유지해 불필요한 런타임 활성 상태를 줄임 | `PlayerRegionTracker`, `RegionGraph`, `ZoneController` |
| Zone State API | 지역 기반 협업 기능이 붙을 수 있는 상태 API와 이벤트 확장 지점 제공 | `SetZoneState`, `SetZonesState`, `OnZoneStateChanged` |

## Design Notes

### Item Container

기존 인벤토리 구조는 인벤토리 하나의 동작에 강하게 맞춰져 있어 창고, 장비창, 루팅창, 제작대까지 확장하기 어려웠습니다.  
서로 다른 저장소라도 “슬롯에서 아이템을 읽고, 쓰고, 비우고, 갱신한다”는 공통 흐름은 같다고 판단했고, 이를 `IItemContainer`와 Adapter 구조로 통합했습니다.

한계도 있었습니다. 현재 이동 정책과 우선순위가 `UIItemMoveManager`에 집중되어 있어, 다음 개선에서는 이동 정책을 별도 Policy 객체로 분리하고 Undo/rollback 구조를 강화할 수 있습니다.

### Crafting

이터널 리턴의 제작 흐름처럼 최상위 아이템을 조회했을 때 최하위 재료까지 보여주는 것을 목표로 했습니다. 재료가 다시 제작 아이템일 수 있으므로 재귀 탐색이 필요했고, 화면을 탐색 과정마다 조금씩 갱신하기보다 `CraftTreeBuilder`가 ScriptableObject 레시피에서 트리를 먼저 완성한 뒤 `CraftTreeRenderer`가 출력하도록 나눴습니다.

레시피는 ScriptableObject로 관리했습니다. 현재 범위에서는 순환 레시피 방지나 중복 재료 캐싱까지 구현하지 않았으며, 데이터 규모가 커진다면 편집 단계의 유효성 검사와 캐싱을 추가할 여지가 있습니다.

### Zone Culling

맵 에셋이 넓고 무거운 상태에서 몬스터·상자 등 런타임 요소까지 늘어나면, 전 지역을 계속 활성 상태로 유지하는 방식은 이후 비용이 커질 수 있다고 보았습니다. `RegionGraph`를 데이터로 두고, 현재 지역과 인접 지역만 `SetActive(true)`로 유지하는 방식으로 런타임 활성 범위를 제한했습니다. 이는 렌더링 프러스텀 컬링이 아니라 GameObject 활성화 제어입니다.

`OnZoneStateChanged` 이벤트는 확장 지점으로 제공했지만, 현재 코드맵 스냅샷 기준으로 외부 클래스가 직접 구독한 코드는 확인되지 않습니다. 따라서 포트폴리오에서는 “구독 가능한 API를 제공했다”로 표현합니다.

## Technical Stack / Patterns

| Topic | Applied In | Note |
|---|---|---|
| Adapter Pattern | `InventoryContainerAdapter`, `StorageContainerAdapter`, `TargetInventoryContainerAdapter`, `EquipmentAdapter` | 서로 다른 UI/데이터 모델을 `IItemContainer`로 통일 |
| Mediator / Facade | `UIItemMoveManager` | 컨테이너 간 이동, 병합, 스왑, 장비 검증을 중앙 처리 |
| Repository Pattern | `StorageRepository`, `GameRepositories` | SQLite 접근을 Repository로 분리 |
| DTO Mapping | `Storage.ExportToStorageData`, `StorageData` | 런타임 모델과 저장 모델 분리 |
| Registry | `NewUIManager`, `UIPanelId` | UI 패널을 ID 기반으로 등록/조회/제어 |
| Template Method 성격 | `SceneController` | 씬별 공통 라이프사이클을 상속 구조로 통일 |
| Graph-based Culling | `RegionGraph`, `ZoneController` | 현재 지역 + 인접 지역만 활성화 |
| Event-driven Extension Point | `OnZoneStateChanged` | 외부 지역 기능이 연결될 수 있는 이벤트 API 제공 |

## Key Class Pages

- [SceneController](docs/classes/SceneController.md)
- [GameSceneManager](docs/classes/GameSceneManager.md)
- [NewUIManager](docs/classes/NewUIManager.md)
- [CraftTreeBuilder](docs/classes/CraftTreeBuilder.md)
- [CraftingService](docs/classes/CraftingService.md)
- [IItemContainer](docs/classes/IItemContainer.md)
- [UIItemMoveManager](docs/classes/UIItemMoveManager.md)
- [Storage](docs/classes/Storage.md)
- [StorageRepository](docs/classes/StorageRepository.md)
- [ZoneController](docs/classes/ZoneController.md)
- [RegionGraph](docs/classes/RegionGraph.md)

## Reading Guide

1. 빠르게 결과를 보고 싶다면 README의 Visual Evidence를 먼저 확인합니다.
2. 구조 흐름을 보고 싶다면 [Interactive GitHub Pages](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/)에서 UML 노드를 클릭합니다.
3. 설계 의도와 세부 구현을 함께 보고 싶다면 GitHub Pages의 `설계 의도`, `고려한 문제와 선택`, `최종 구조`, `Evidence`, `Code Preview`를 순서대로 확인합니다.

