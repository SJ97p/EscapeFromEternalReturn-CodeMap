# ZoneController

## Role

플레이어 현재 지역과 인접 지역만 활성화하는 Region 기반 월드 최적화 컨트롤러입니다.

## Class Diagram

```mermaid
classDiagram
    class ZoneController {
        -PlayerRegionTracker playerRegionTracker
        -RegionGraphSO regionGraph
        -List~RegionZoneEntry~ regionZones
        -bool useZoneOptimization
        -Dictionary~Region, Zone~ regionZoneMap
        -HashSet~Region~ activeRegions
        +event Action~Region, ZoneState~ OnZoneStateChanged
        -void Awake()
        -void Start()
        -void InitializeZones()
        -void OnEnable()
        -void OnDisable()
        -void HandleRegionChanged(Region region)
        +Zone GetZone(Region region)
        +void NotifyZoneStateChanged(Region region, ZoneState state)
        +void SetZoneState(Region region, ZoneState state)
        +void SetZonesState(IEnumerable~Region~ regions, ZoneState state)
        -void UpdateZones(HashSet~Region~ nextRegions)
        -HashSet~Region~ GetRegionsToActivate(Region currentRegion)
        -void AutoPopulateRegionZones()
    }

    class RegionGraph {
        +List~RegionNodeData~ nodes
    }

    class Zone {
        +Region RegionType
        +void SetZoneState(ZoneState state)
        +ZoneState GetZoneState()
    }

    class PlayerRegionTracker {
        +Region CurrentRegion
        +event Action~Region~ OnRegionChanged
    }

    ZoneController --> PlayerRegionTracker : subscribe
    ZoneController --> RegionGraph : adjacent lookup
    ZoneController --> Zone : activate / state
```

## Design Point

`activeRegions`와 다음 활성 지역 집합의 차집합만 계산해 필요한 Zone만 켜고 끕니다. 금지구역이나 지역 이벤트 같은 협업 기능은 `SetZoneState`와 이벤트를 통해 붙을 수 있습니다.

## Source

- [ZoneController.cs](../../src/Assets/00_Scripts/ZoneControllers/ZoneController.cs)

