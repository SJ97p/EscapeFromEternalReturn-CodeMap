# Escape From Eternal Return Code Map

![Escape From Eternal Return title](assets/evidence/escape-from-eternal-return-title.png)

**Escape From Eternal Return**은 전투·파밍·탈출 뒤 전리품을 보관·제작해 다음 탐험을 준비하는 Unity 기반 싱글 플레이 익스트랙션 생존 액션 RPG입니다.

## 프로젝트 개요

| 항목 | 내용 |
|---|---|
| 프로젝트 | Escape From Eternal Return |
| 장르 | 싱글 플레이 익스트랙션 생존 액션 RPG |
| 기간 | 2026.04.17 ~ 2026.05.26 |
| 인원 | 3인 팀 프로젝트 |
| 역할 | 팀장 / 전체 플레이 흐름 및 시스템 요구사항 설계 / 아이템·제작·저장·Zone 시스템 구현 및 통합 |
| 엔진·언어 | Unity 6.3 / C# |
| 핵심 기술 | ScriptableObject, SQLite, Unity Input System, URP, Physics Raycast |
| 직접 구현·재구성 | 아이템 컨테이너 통합 및 이동 시스템, 재귀 제작 트리·제작 처리, SQLite 저장·복원, RegionGraph 기반 Zone 활성화 최적화 |

[![인터랙티브 코드맵](assets/navigation/code-map-link.svg)](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/)

<details>
<summary>주요 사례 바로가기</summary>

- [서로 다른 저장공간의 아이템 이동 규칙](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/#case=item-container)
- [목표 장비를 위한 재귀 제작 트리](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/#case=crafting-tree)
- [Region 기반 Zone 활성화](https://sj97p.github.io/EscapeFromEternalReturn-CodeMap/#case=zone-culling)

</details>
