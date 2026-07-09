# ZoneController

## Role

플레이어 현재 지역과 인접 지역만 활성화하는 Region 기반 월드 최적화 컨트롤러입니다.

## Responsibilities

- 자식 `Zone`들을 수집해 `Region -> Zone` 맵 구성
- `PlayerRegionTracker.OnRegionChanged`를 받아 활성 지역 갱신
- `RegionGraph`를 통해 현재 지역의 인접 지역 계산
- 이전 활성 지역과 다음 활성 지역의 차집합만 토글
- 금지구역, 하이퍼루프, 지역 이벤트 같은 협업 기능이 붙을 수 있는 Zone 상태 API와 이벤트 제공

## Source

- [ZoneController.cs](../../src/Assets/00_Scripts/ZoneControllers/ZoneController.cs)

## Related

- [RegionGraph Zone Culling](../systems/zone-culling.md)
- [Zone State API / Collaboration Extension](../systems/zone-state-api.md)
