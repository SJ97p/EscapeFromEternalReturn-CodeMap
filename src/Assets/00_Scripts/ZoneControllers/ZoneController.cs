using System;
using System.Collections.Generic;
using System.Linq;
using HBDinosaur_ER_Project.Database; 
using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.ZoneSystem
{
    public class ZoneController : MonoBehaviour
    {
        [SerializeField] private PlayerRegionTracker playerRegionTracker;
        [SerializeField] private RegionGraphSO regionGraph;
        [SerializeField] private List<RegionZoneEntry> regionZones;  // Inspector용
        [SerializeField] private bool useZoneOptimization = true;
        private Dictionary<Region, Zone> regionZoneMap;
        private HashSet<Region> activeRegions = new();

        public event Action<Region, ZoneState> OnZoneStateChanged;

        private void Awake()
        {
            // 1. 내부 맵 초기화
            var zones = GetComponentsInChildren<Zone>(true);
            regionZoneMap = zones.ToDictionary(z => z.RegionType, z => z);

            // 2. [핵심] 플레이어 트래커를 가장 먼저 찾아서 할당합니다.
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerRegionTracker = playerObj.GetComponent<PlayerRegionTracker>();
            }
            else
            {
                Debug.LogError("[ZoneController] 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }

        private void Start()
        {
            if (useZoneOptimization)
                InitializeZones();
        }

        private void InitializeZones()
        {
            // 이미 Awake에서 찾았으므로 Find 코드는 제거해도 됩니다.
            if (playerRegionTracker == null) return;

            // 현재 플레이어 지역 기준으로 활성 목록 계산
            Region currentRegion = playerRegionTracker.CurrentRegion;
            HashSet<Region> initialRegions = GetRegionsToActivate(currentRegion);

            // 전부 끄기
            foreach (var entry in regionZones)
            {
                if (entry.zone != null)
                    entry.zone.gameObject.SetActive(false);
            }

            // 필요한 것만 켜기
            foreach (Region region in initialRegions)
            {
                Zone zone = GetZone(region);
                if (zone != null)
                    zone.gameObject.SetActive(true);
            }

            activeRegions = initialRegions;
        }
        private void OnEnable()
        {
            // 안전장치: playerRegionTracker가 확실히 있을 때만 구독
            if (useZoneOptimization && playerRegionTracker != null)
            {
                playerRegionTracker.OnRegionChanged += HandleRegionChanged;
                Debug.Log("RegionTracker 구독 완료");
            }
        }

        private void OnDisable()
        {
            // 안전장치: 구독 해제할 때도 null 체크
            if (useZoneOptimization && playerRegionTracker != null)
            {
                playerRegionTracker.OnRegionChanged -= HandleRegionChanged;
            }
        }
        private void HandleRegionChanged(Region region)
        {
            HashSet<Region> regions = GetRegionsToActivate(region);
            UpdateZones(regions);
        }

        public Zone GetZone(Region region)
        {
            regionZoneMap.TryGetValue(region, out Zone zone);
            return zone;
        }

        public void NotifyZoneStateChanged(Region region, ZoneState state)
        {
            OnZoneStateChanged?.Invoke(region, state);
        }

        public void SetZoneState(Region region, ZoneState state)
        {
            Zone zone = GetZone(region);
            if (zone != null)
            {
                zone.SetZoneState(state);
                NotifyZoneStateChanged(region, state);
            }
            else
            {
                Debug.LogWarning($"[ZoneController] Cannot find zone for region: {region}");
            }
        }

        public void SetZonesState(IEnumerable<Region> regions, ZoneState state)
        {
            foreach (var region in regions)
            {
                SetZoneState(region, state);
            }
        }
        private void UpdateZones(HashSet<Region> nextRegions)
        {
            HashSet<Region> regionsToEnable = new(nextRegions);
            regionsToEnable.ExceptWith(activeRegions);

            HashSet<Region> regionsToDisable = new(activeRegions);
            regionsToDisable.ExceptWith(nextRegions);

            Debug.Log($"[ZoneController] nextRegions: {string.Join(", ", nextRegions.Select(r => r.ToString()))}");

            foreach (Region region in regionsToEnable)
            {
                Zone zone = GetZone(region);
                if (zone != null)
                    zone.gameObject.SetActive(true);
            }

            foreach (Region region in regionsToDisable)
            {
                Zone zone = GetZone(region);
                if (zone != null)
                    zone.gameObject.SetActive(false);
            }

            activeRegions = nextRegions;
        }
        private HashSet<Region> GetRegionsToActivate(Region currentRegion)
        {
            HashSet<Region> result = new();
            // 안전 장치: regionGraph가 인스펙터에서 할당되지 않았을 때
            if (regionGraph == null)
            {
                Debug.LogError("[ZoneController] RegionGraphSO(regionGraph) 에셋이 인스펙터에 할당되지 않았습니다!");
                return result;
            }

            // 안전 장치: 현재 지역 정보가 null일 때
            if (currentRegion == Region.None)
            {
                Debug.LogWarning("[ZoneController] currentRegion이 Null입니다.");
                return result;
            }
            result.Add(currentRegion);

            RegionNodeData node = regionGraph.nodes.Find(x => x.region == currentRegion);
            if (node != null)
            {
                foreach (var adjacent in node.adjacentRegions)
                    result.Add(adjacent);
            }

            return result;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoPopulateRegionZones();
        }

        [ContextMenu("Auto Populate Region Zones")]
        private void AutoPopulateRegionZones()
        {
            Zone[] zones = GetComponentsInChildren<Zone>(true);

            regionZones = zones
                .Where(z => z != null)
                .GroupBy(z => z.RegionType)
                .Select(g => new RegionZoneEntry
                {
                    region = g.Key,
                    zone = g.First()
                })
                .ToList();

            foreach (var group in zones.GroupBy(z => z.RegionType))
            {
                if (group.Count() > 1)
                {
                    Debug.LogWarning(
                        $"[ZoneController] Region {group.Key} 에 해당하는 Zone이 여러 개입니다. 첫 번째만 사용합니다.",
                        this);
                }
            }
        }
#endif
    }
}

