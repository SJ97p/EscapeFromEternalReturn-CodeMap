# Inventory

## Role

플레이어 인벤토리의 슬롯 배열과 아이템 이동 기본 연산을 담당합니다.

## Responsibilities

- 빈 슬롯에 아이템 추가
- 슬롯 아이템 제거/조회
- 슬롯 간 아이템 교환
- 다른 컨테이너로 넘길 아이템 추출
- `InventoryEventBus`로 UI 갱신 이벤트 발행
- `IInteractable` 기반 상호작용 요청 처리

## Source

- [Inventory.cs](../../src/Assets/00_Scripts/Inventory_Scripts/InventoryLogic/Inventory.cs)

## Related

- [Inventory & Storage](../systems/inventory-storage.md)

