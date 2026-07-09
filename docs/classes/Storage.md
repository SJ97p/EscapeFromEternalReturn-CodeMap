# Storage

## Role

인벤토리, 창고, 장비창, 루팅창이 공유하는 런타임 슬롯 데이터 모델입니다.

## Responsibilities

- 2차원 슬롯 배열 초기화
- 슬롯 조회/설정/비우기
- 아이템 수량 합산
- 스택 우선 추가
- 재료 제거
- 저장 시 비어있지 않은 슬롯만 `StorageData`로 변환

## Source

- [Storage.cs](../../src/Assets/00_Scripts/Storage_Scripts/StorageLogic/Storage.cs)

## Related

- [Item Container Transaction](../systems/item-container-transaction.md)
- [Storage Persistence](../systems/storage-persistence.md)

