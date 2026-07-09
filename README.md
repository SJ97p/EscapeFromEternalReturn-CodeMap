# EscapeFromEternalReturn Code Map

Unity 기반 탑다운 생존 액션 RPG **Escape From Eternal Return**의 런타임 시스템 구조를 설명하는 포트폴리오용 레포지토리입니다.

이 레포지토리는 전체 프로젝트를 그대로 공개하기보다, 제가 담당한 **씬 라이프사이클, UI 레지스트리, 제작 트리, 아이템 컨테이너, 저장/로드, 지역 최적화**를 중심으로 코드 구조와 설계 의도를 정리합니다.

## Project Summary

| Item | Description |
|---|---|
| Project | Escape From Eternal Return |
| Genre | 탑다운 생존 액션 RPG |
| Team | 3인 팀 프로젝트 |
| Engine / Language | Unity / C# |
| My Focus | Runtime Architecture, Scene/UI Flow, Inventory Transaction, SQLite Persistence, Region Culling |

## What I Wanted To Show

이번 프로젝트에서 제가 중점적으로 개발한 부분은 개별 기능보다, **기능들이 서로 안정적으로 연결되는 구조**였습니다.

- 모든 씬이 공통 `SceneController` 라이프사이클을 따르도록 구성
- `UIPanelId` 기반 레지스트리로 UI 패널 열기/닫기/토글 흐름 통합
- 레시피 데이터를 재귀 탐색해 다단계 제작 트리를 런타임에 생성
- 인벤토리, 루팅 창, 장비창, 창고를 `Storage` 모델과 `IItemContainer` 인터페이스로 통합
- `UIItemMoveManager`에서 이동, 병합, 스왑, 자동 루팅, 장비 장착 검증, UI 상태별 이동 우선순위 처리
- 런타임 `Storage` 데이터를 저장 시점에 `StorageData` DTO로 변환하고 SQLite Repository로 저장
- `RegionGraph`와 `ZoneController`로 현재 지역과 인접 지역만 활성화해 월드 부하 감소
- 금지구역 시스템이 `Zone` 상태 API만 사용해 연결될 수 있도록 확장 지점 분리

## Core Systems

| System | Problem | Solution | Docs |
|---|---|---|---|
| Scene Lifecycle & UI Registry | 씬 전환, UI 호출, 컨텍스트 전달이 씬마다 흩어질 수 있음 | `SceneController`, `GameSceneManager`, `UIPanelId` 기반 등록/조회 구조 | [Scene UI Lifecycle](docs/systems/scene-ui-lifecycle.md) |
| Recursive Crafting Tree | 재료가 다시 제작 아이템인 다단계 조합식을 UI에서 직접 처리하기 어려움 | 레시피 DB를 재귀 탐색해 `CraftTreeNode` 트리를 만들고 렌더러가 UI 생성 | [Recursive Crafting Tree](docs/systems/recursive-crafting-tree.md) |
| Item Container Transaction | 인벤토리/창고/장비창/루팅창 이동 규칙이 중복되면 복사/증발 문제가 생김 | `IItemContainer` + Adapter + `UIItemMoveManager` 중앙 이동 루틴 | [Item Container Transaction](docs/systems/item-container-transaction.md) |
| SQLite Storage Persistence | 런타임 슬롯 모델을 그대로 저장하면 저장 구조가 UI/런타임에 묶임 | 비어있지 않은 슬롯만 `StorageData`로 변환해 세이브 슬롯별 저장 | [Storage Persistence](docs/systems/storage-persistence.md) |
| RegionGraph Zone Culling | 보이지 않는 지역의 몬스터/상자/이벤트까지 활성화하면 런타임 비용 증가 | 현재 지역과 인접 지역만 `activeRegions`로 유지하고 차집합만 토글 | [Zone Culling](docs/systems/zone-culling.md) |
| Restricted Zone Extension API | 금지구역 시스템이 Zone 내부 구현에 직접 의존하면 협업 확장이 어려움 | `SetZoneState`, `SetZonesState`, `OnZoneStateChanged`로 상태 API 분리 | [Restricted Zone API](docs/systems/restricted-zone-api.md) |

## Technical Stack / Patterns

| Topic | Applied In | Note |
|---|---|---|
| Adapter Pattern | `InventoryContainerAdapter`, `StorageContainerAdapter`, `TargetInventoryContainerAdapter`, `EquipmentAdapter` | 서로 다른 UI/데이터 모델을 `IItemContainer`로 통일 |
| Mediator / Facade | `UIItemMoveManager` | 컨테이너 간 이동, 병합, 스왑, 자동 이동 우선순위를 중앙에서 조율 |
| Repository Pattern | `StorageRepository`, `ItemRepository`, `GameRepositories` | SQLite 접근을 도메인 Repository로 감쌈 |
| DTO Mapping | `Storage.ExportToStorageData`, `StorageData` | 런타임 슬롯 모델과 저장 모델 분리 |
| Registry | `NewUIManager`, `UIPanelId` | UI 패널을 ID 기반으로 등록/조회/제어 |
| Template Method 성격 | `SceneController` 계층 | 씬별 공통 라이프사이클을 상속 구조로 통일 |
| Graph-based Culling | `RegionGraph`, `ZoneController` | 현재 지역 + 인접 지역만 활성화 |
| Event-driven Extension | `OnZoneStateChanged`, UI refresh calls | 상태 변경과 외부 반응을 느슨하게 연결 |

## Architecture Map

- [Interactive Code Map](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/)
- [Architecture Overview](docs/architecture.md)
- [Class Diagram](docs/class-diagram.md)
- [Documentation Index](docs/index.md)
- [Improvement Notes](docs/improvements.md)

## Key Class Pages

- [SceneController](docs/classes/SceneController.md)
- [GameSceneManager](docs/classes/GameSceneManager.md)
- [NewUIManager](docs/classes/NewUIManager.md)
- [UIPanel](docs/classes/UIPanel.md)
- [CraftTreeBuilder](docs/classes/CraftTreeBuilder.md)
- [CraftingService](docs/classes/CraftingService.md)
- [IItemContainer](docs/classes/IItemContainer.md)
- [UIItemMoveManager](docs/classes/UIItemMoveManager.md)
- [Storage](docs/classes/Storage.md)
- [StorageRepository](docs/classes/StorageRepository.md)
- [ZoneController](docs/classes/ZoneController.md)
- [RegionGraph](docs/classes/RegionGraph.md)

## Source Code

핵심 스크립트는 [`src/Assets/00_Scripts`](src/Assets/00_Scripts)에 정리되어 있습니다.

## Reading Guide

1. 발표 흐름을 보고 싶다면 [Architecture Overview](docs/architecture.md)를 먼저 읽습니다.
2. 설계 패턴과 기술 스택은 [Documentation Index](docs/index.md)의 시스템 문서를 따라가면 됩니다.
3. 아이템 이동 구조는 [Item Container Transaction](docs/systems/item-container-transaction.md)을 중심으로 확인합니다.
4. 저장/로드 구조는 [Storage Persistence](docs/systems/storage-persistence.md)를 읽습니다.
5. 월드 최적화는 [Zone Culling](docs/systems/zone-culling.md)에서 확인합니다.
