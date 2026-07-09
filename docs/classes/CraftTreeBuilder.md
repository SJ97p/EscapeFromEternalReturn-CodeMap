# CraftTreeBuilder

## Role

레시피 데이터를 기반으로 다단계 제작 트리를 재귀적으로 생성합니다.

## Responsibilities

- 결과 아이템 ID로 레시피 조회
- 재료가 다시 제작 가능한 아이템인지 확인
- 하위 재료 노드를 반복 생성
- UI 렌더러와 제작 서비스가 사용할 `CraftTreeNode` 구조 생성

## Source

- [CraftTreeBuilder.cs](../../src/Assets/00_Scripts/Craft/CraftTreeBuilder.cs)

## Related

- [Recursive Crafting Tree](../systems/recursive-crafting-tree.md)

