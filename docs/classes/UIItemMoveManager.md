# UIItemMoveManager

## Role

서로 다른 아이템 컨테이너 사이의 이동, 병합, 스왑, 자동 이동을 중앙에서 조율합니다.

## Responsibilities

- `IItemContainer` 등록/조회
- 현재 열린 UI 상태 추적
- 이동 가능 여부 검증
- 스택 병합 처리
- 스왑과 순수 이동 커밋
- 실패 시 부분 롤백
- 장비 슬롯 타입 검증
- UI 상태별 자동 이동 우선순위 처리

## Source

- [UIItemMoveManager.cs](../../src/Assets/00_Scripts/Storage_Scripts/StorageLogic/UIItemMoveManager.cs)

## Related

- [Item Container Transaction](../systems/item-container-transaction.md)

