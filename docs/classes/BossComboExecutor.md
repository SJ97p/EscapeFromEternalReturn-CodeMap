# BossComboExecutor

## Role

보스 스킬과 중첩 콤보 액션을 순차 실행하는 코루틴 기반 실행기입니다.

## Responsibilities

- 현재 콤보 실행 여부 관리
- 단일 보스 스킬 액션 실행
- 여러 액션을 포함한 콤보 액션 재귀 실행
- 스킬 슬롯의 종료 조건을 기다림
- 실행 중단 시 코루틴 정리

## Source

- [BossComboExecutor.cs](../../src/Assets/00_Scripts/Monster/BossMonster/BossSkill/Combo/BossComboExecutor.cs)

## Related

- [Monster / Boss AI](../systems/monster-ai.md)
