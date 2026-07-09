# Improvement Notes

## Scene / UI

`UIPanelId` 기반 레지스트리로 호출 구조는 정리되었지만, 패널 의존성이 늘어날수록 열림 조건과 닫힘 정책이 복잡해질 수 있습니다. 다음 단계에서는 `UIPanelPolicy`나 화면 상태 머신을 두어 “어떤 패널 조합이 동시에 열릴 수 있는가”를 데이터로 관리할 수 있습니다.

## Item Transaction

`UIItemMoveManager`가 이동, 병합, 스왑, 자동 이동, 장비 검증을 모두 처리합니다. 현재 구조는 중앙화 장점이 크지만 클래스가 커질 수 있으므로, 장기적으로는 `MoveCommand`, `MergePolicy`, `AutoMovePolicy`, `EquipmentMovePolicy`로 나누면 테스트 단위가 더 선명해집니다.

## Persistence

저장 시 비어있지 않은 슬롯만 `StorageData`로 변환하는 방식은 데이터량을 줄입니다. 다음 단계에서는 저장 전후 슬롯 총량 검증을 추가해 아이템 복사/증발을 자동 감지할 수 있습니다.

## Zone Culling

현재 구조는 현재 지역과 인접 지역만 활성화하는 방식입니다. 추가 개선으로는 거리 단계별 활성화, 비동기 프리로드, 몬스터/상자 풀링 유지 정책을 분리할 수 있습니다.

## Flyweight Direction

현재 구현은 Flyweight 패턴이 직접 적용되었다기보다, 적용하기 좋은 구조에 가깝습니다. `RegionGraphSO`, `ZoneMonsterSpawnTable`, 아이템/레시피 DB처럼 여러 Zone이 공유하는 데이터를 ScriptableObject 기반 불변 데이터로 분리하면 Zone 인스턴스별 중복 상태를 줄일 수 있습니다.

## Encoding Cleanup

일부 원본 C# 주석 인코딩이 깨져 있습니다. 공개 포트폴리오 품질을 위해 UTF-8로 통일하고 깨진 주석은 제거하거나 새 설명으로 교체하는 것이 좋습니다.
