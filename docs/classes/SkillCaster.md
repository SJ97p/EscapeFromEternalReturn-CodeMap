# SkillCaster

## Role

플레이어 스킬의 캐스팅 흐름, 이동 잠금, 취소, 종료 대기를 관리합니다.

## Responsibilities

- 점유 스킬과 비점유 스킬 실행 흐름 분리
- 스킬 데이터 초기화
- 캐스팅 중 플레이어 이동 잠금
- 애니메이션 이벤트 발행
- 스킬 완료 이벤트를 기다렸다가 캐스팅 종료
- 재시전과 취소 처리

## Source

- [SkillCaster.cs](../../src/Assets/00_Scripts/Player/Skill/SkillCaster.cs)

## Related

- [Player Combat](../systems/player-combat.md)

