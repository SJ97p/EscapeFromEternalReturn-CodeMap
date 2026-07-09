# RegionGraph

## Role

지역과 인접 지역 관계를 데이터로 표현해 Zone Culling과 지역 기반 시스템의 기준을 제공합니다.

## Responsibilities

- 각 Region의 인접 지역 정보 보관
- `ZoneController`가 현재 지역 주변 활성 범위를 계산할 수 있게 지원
- 금지구역, 스폰, 이벤트, 난이도 변화 같은 지역 기반 시스템의 공통 데이터 축 제공

## Source

- [RegionGraph.cs](../../src/Assets/00_Scripts/ZoneControllers/RegionGraph.cs)

## Related

- [RegionGraph Zone Culling](../systems/zone-culling.md)
