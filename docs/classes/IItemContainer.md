# IItemContainer

## Role

인벤토리, 창고, 장비창, 루팅창을 같은 이동 시스템에 연결하기 위한 공통 컨테이너 인터페이스입니다.

## Responsibilities

- 컨테이너 타입과 크기 제공
- 슬롯 유효성/비어 있음 확인
- 아이템 ID와 수량 조회
- 슬롯 설정/비우기
- Drop/Click 처리 진입점 제공
- UI 갱신 메서드 제공

## Source

- [IItemContainer.cs](../../src/Assets/00_Scripts/Storage_Scripts/StorageLogic/IItemContainer.cs)

## Related

- [Item Container Transaction](../systems/item-container-transaction.md)

