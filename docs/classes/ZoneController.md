# ZoneController

## Role

현재 플레이어 지역과 인접 지역을 기준으로 활성화할 `Zone`을 관리하는 지역 시스템의 중심 클래스입니다.

## Responsibilities

- 자식 `Zone`들을 수집해 `Region -> Zone` 맵 구성
- `PlayerRegionTracker.OnRegionChanged`를 받아 활성 지역 갱신
- `RegionGraphSO`를 통해 현재 지역의 인접 지역 계산
- 외부 시스템에 `SetZoneState`, `SetZonesState` 진입점 제공
- 지역 상태 변경 이벤트 발행

## Source

- [ZoneController.cs](../../src/Assets/00_Scripts/ZoneControllers/ZoneController.cs)

## Related

- [Restricted Zone & Region](../systems/restricted-zone.md)

