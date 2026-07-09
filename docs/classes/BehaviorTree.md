# BehaviorTree

## Role

보스 AI의 행동 트리 루트 노드입니다.

## Responsibilities

- 자식 노드를 순서대로 평가
- 실패 또는 진행 중 상태가 나오면 즉시 반환
- 모든 자식이 성공하면 성공 반환
- Selector/Sequence/Strategy 조합의 실행 시작점 제공

## Source

- [BehaviorTree.cs](../../src/Assets/00_Scripts/Monster/BossMonster/BT/BehaviorTree.cs)

## Related

- [Monster / Boss AI](../systems/monster-ai.md)

