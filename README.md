# EscapeFromEternalReturn Code Map

Unity 기반 탑다운 생존 액션 RPG **Escape From Eternal Return**의 코드 구조를 설명하는 포트폴리오용 레포지토리입니다.

이 레포지토리는 전체 Unity 프로젝트를 그대로 공개하기보다, 팀 프로젝트에서 직접 구현한 핵심 시스템과 설계 의도를 문서화합니다.

## Project Summary

| Item | Description |
|---|---|
| Project | Escape From Eternal Return |
| Genre | 탑다운 생존 액션 RPG |
| Team | 3인 팀 프로젝트 |
| Engine / Language | Unity / C# |
| Focus | Restricted Zone, Hyperloop, Inventory/Storage, Crafting, Player Combat, Monster AI, Data Repository |

## What I Wanted To Show

이 프로젝트에서 가장 중요하게 본 것은 “기능을 많이 붙이는 것”보다, **전투, 파밍, 이동, 지역 상태, 데이터 로딩이 서로 어긋나지 않게 연결되는 구조**였습니다.

- 지역 진입과 시간 흐름에 따라 금지구역/스폰/지역 UI 상태를 갱신
- 하이퍼루프를 통해 지역 이동과 씬/플레이어 위치 흐름을 분리
- 인벤토리, 보관함, 장비, 루팅 컨테이너를 공통 컨테이너 인터페이스로 연결
- 제작 트리와 재료 확인을 서비스/뷰 계층으로 분리
- 플레이어 FSM, 스킬 캐스팅, 타겟 쿼리, 상태이상을 전투 흐름으로 구성
- 몬스터와 보스 AI를 행동 트리/스킬 슬롯/콤보 실행 구조로 분리
- CSV/SQLite 기반 데이터 로딩을 Repository 계층으로 감싸 런타임 시스템과 데이터 소스를 분리

## Core Systems

| System | Problem | Solution | Docs |
|---|---|---|---|
| Restricted Zone & Region | 지역 상태, 금지구역, 몬스터 스폰, UI 알림이 흩어질 수 있음 | RegionGraph와 ZoneController 계층으로 지역 상태를 중심화 | [Restricted Zone](docs/systems/restricted-zone.md) |
| Hyperloop Travel | 지역 이동이 씬/플레이어/지역 UI에 직접 얽히기 쉬움 | Hyperloop와 Region UI를 분리해 목적지 선택과 이동 처리를 연결 | [Hyperloop Travel](docs/systems/hyperloop-travel.md) |
| Inventory & Storage | 인벤토리, 장비, 보관함, 루팅 컨테이너가 각자 다른 이동 규칙을 갖기 쉬움 | IItemContainer, Adapter, DragDrop/QuickMove 서비스로 공통 조작 처리 | [Inventory Storage](docs/systems/inventory-storage.md) |
| Crafting Tree | 제작 가능 여부와 제작 경로 표시가 UI에 섞이기 쉬움 | CraftingService와 CraftTreeBuilder/Renderer로 로직과 표시 분리 | [Crafting Tree](docs/systems/crafting-tree.md) |
| Player Combat | 이동, 공격, 스킬, 피격, 상태이상이 동시에 발생하면 상태가 꼬일 수 있음 | PlayerFSM, SkillCaster, TargetQuery, Effect 계층으로 책임 분리 | [Player Combat](docs/systems/player-combat.md) |
| Monster / Boss AI | 일반 몬스터와 보스 패턴이 거대 조건문으로 커지기 쉬움 | Behavior Tree, Strategy, SkillSlot, ComboExecutor로 의사결정과 실행 분리 | [Monster AI](docs/systems/monster-ai.md) |
| Data Repository | CSV/SQLite/ScriptableObject 데이터 접근이 런타임 코드에 직접 퍼질 수 있음 | DBLoader, GameDataStore, Repository 인터페이스로 접근 경로 통일 | [Data Repository](docs/systems/data-repository.md) |

## Architecture Map

- [Interactive Code Map](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/)
- [Architecture Overview](docs/architecture.md)
- [Class Diagram](docs/class-diagram.md)
- [Documentation Index](docs/index.md)
- [Improvement Notes](docs/improvements.md)

## Key Class Pages

- [ZoneController](docs/classes/ZoneController.md)
- [RestrictedZoneController](docs/classes/RestrictedZoneController.md)
- [Hyperloop](docs/classes/Hyperloop.md)
- [Inventory](docs/classes/Inventory.md)
- [Storage](docs/classes/Storage.md)
- [CraftingService](docs/classes/CraftingService.md)
- [PlayerFSM](docs/classes/PlayerFSM.md)
- [SkillCaster](docs/classes/SkillCaster.md)
- [MonsterController](docs/classes/MonsterController.md)
- [BehaviorTree](docs/classes/BehaviorTree.md)
- [GameDataStore](docs/classes/GameDataStore.md)

## Source Code

핵심 스크립트는 [`src/Assets/00_Scripts`](src/Assets/00_Scripts)에 정리되어 있습니다.

## Reading Guide

1. 빠르게 프로젝트 의도를 보고 싶다면 [Core Systems](#core-systems)를 먼저 읽습니다.
2. 전체 클래스 관계는 [Class Diagram](docs/class-diagram.md)에서 확인합니다.
3. 면접용 문제 해결 흐름은 [docs/systems](docs/systems) 문서를 읽습니다.
4. 구현 당시 한계와 개선 방향은 [Improvement Notes](docs/improvements.md)에 정리했습니다.

## Diagram Note

GitHub의 Mermaid 렌더러는 환경에 따라 `click` 링크가 제한될 수 있습니다. 그래서 각 다이어그램 아래에 동일한 클래스 링크 목록을 함께 제공합니다.
