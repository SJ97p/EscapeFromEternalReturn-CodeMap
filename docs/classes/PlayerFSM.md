# PlayerFSM

## Role

플레이어의 이동, 공격, 스킬, 상호작용, 피격, 사망 상태 전이를 관리합니다.

## Responsibilities

- 상태 객체 생성 및 현재 상태 보관
- enum 요청을 실제 상태 객체로 변환
- 점유 상태와 우선순위 기반 전이 제한
- 상태 Enter/Exit 호출
- 상태 변경 이벤트 발행
- 매 프레임 현재 상태 업데이트 위임

## Source

- [PlayerFSM.cs](../../src/Assets/00_Scripts/Player/Core/PlayerFSM.cs)

## Related

- [Player Combat](../systems/player-combat.md)

