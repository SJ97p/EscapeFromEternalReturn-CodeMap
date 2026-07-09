//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using HBDinosaur_ER_Project.Database;
//using UnityEngine;

//namespace HBDinosaur_ER_Project.ZoneSystem
//{
//    public class RestrictedZoneController : MonoBehaviour
//    {
//        [Header("Cycle Settings")]
//        [SerializeField] private float cycleDuration = 60f;        // 1사이클 총 소요 시간 (초)
//        [SerializeField] private float warningDuration = 15f;      // 금지구역 예고(경고) 지속 시간 (초)
//        [SerializeField] private int restrictedCountPerCycle = 1;  // 사이클당 금지구역으로 설정할 구역 수
//        [SerializeField] private List<Region> excludeRegions = new() { Region.None, Region.Laboratory }; // 금지구역 대상에서 제외할 구역들

//        private List<Region> allRegions = new();
//        private HashSet<Region> restrictedRegions = new();
//        private HashSet<Region> warningRegions = new();

//        public float CurrentTimer { get; private set; }
//        public bool IsWarningPeriod { get; private set; }

//        public event Action<float> OnTimerUpdated;
//        public event Action<HashSet<Region>, ZoneState> OnZoneStateCycleChanged; // UI 및 연동 스크립트 노출용 이벤트

//        private Coroutine cycleCoroutine;

//        private void Start()
//        {
//            InitializeRegions();
//            StartZoneCycle();
//        }

//        private void InitializeRegions()
//        {
//            allRegions = Enum.GetValues(typeof(Region))
//                .Cast<Region>()
//                .Where(r => !excludeRegions.Contains(r))
//                .ToList();

//            restrictedRegions.Clear();
//            warningRegions.Clear();
//        }

//        public void StartZoneCycle()
//        {
//            if (cycleCoroutine != null)
//            {
//                StopCoroutine(cycleCoroutine);
//            }
//            cycleCoroutine = StartCoroutine(ZoneCycleRoutine());
//        }

//        public void StopZoneCycle()
//        {
//            if (cycleCoroutine != null)
//            {
//                StopCoroutine(cycleCoroutine);
//                cycleCoroutine = null;
//            }
//        }

//        private IEnumerator ZoneCycleRoutine()
//        {
//            while (true)
//            {
//                // 1. 대기 단계 (Safe Period)
//                IsWarningPeriod = false;
//                float waitDuration = Mathf.Max(0f, cycleDuration - warningDuration);

//                // 만약 남은 일반 구역이 1개 이하라면 최종 구역이므로 더 이상 금지구역을 설정하지 않고 대기만 함
//                int activeNormalZonesCount = allRegions.Count - restrictedRegions.Count;
//                if (activeNormalZonesCount <= 1)
//                {
//                    Debug.Log("[RestrictedZoneController] 최종 안전 구역만 남았습니다. 사이클을 종료합니다.");
//                    yield break;
//                }

//                CurrentTimer = waitDuration;
//                while (CurrentTimer > 0)
//                {
//                    OnTimerUpdated?.Invoke(CurrentTimer);
//                    yield return null;
//                    CurrentTimer -= Time.deltaTime;
//                }
//                CurrentTimer = 0;
//                OnTimerUpdated?.Invoke(0f);

//                // 2. 경고 구역 선정 및 경고 단계 시작 (Warning Period)
//                IsWarningPeriod = true;
//                SelectWarningRegions();

//                if (warningRegions.Count > 0)
//                {
//                    //ZoneController.Instance?.SetZonesState(warningRegions, ZoneState.WarningArea);
//                    OnZoneStateCycleChanged?.Invoke(warningRegions, ZoneState.WarningArea);
//                    Debug.Log($"[RestrictedZoneController] Warning Area Selected: {string.Join(", ", warningRegions)}");
//                }

//                CurrentTimer = warningDuration;
//                while (CurrentTimer > 0)
//                {
//                    OnTimerUpdated?.Invoke(CurrentTimer);
//                    yield return null;
//                    CurrentTimer -= Time.deltaTime;
//                }
//                CurrentTimer = 0;
//                OnTimerUpdated?.Invoke(0f);

//                // 3. 금지구역 확정 (Restriction Confirmation)
//                if (warningRegions.Count > 0)
//                {
//                    foreach (var region in warningRegions)
//                    {
//                        restrictedRegions.Add(region);
//                    }

//                    //ZoneController.Instance?.SetZonesState(warningRegions, ZoneState.RestrictedArea);
//                    OnZoneStateCycleChanged?.Invoke(warningRegions, ZoneState.RestrictedArea);
//                    Debug.Log($"[RestrictedZoneController] Restricted Area Confirmed: {string.Join(", ", warningRegions)}");

//                    warningRegions.Clear();
//                }

//                yield return null;
//            }
//        }

//        private void SelectWarningRegions()
//        {
//            warningRegions.Clear();

//            // 아직 금지구역이 되지 않은 구역 후보들 리스트업
//            List<Region> candidates = allRegions
//                .Where(r => !restrictedRegions.Contains(r))
//                .ToList();

//            int targetCount = restrictedCountPerCycle;

//            // 남은 구역이 최종 1개는 확보되도록 방어 로직 구현
//            // ex) 후보가 3개 남았고 3개를 다 금지하려 하면, 2개만 선택되도록 처리
//            if (candidates.Count - targetCount < 1)
//            {
//                targetCount = candidates.Count - 1;
//            }

//            if (targetCount <= 0) return;

//            // 랜덤하게 후보 셔플 및 선정
//            System.Random random = new System.Random();
//            var selected = candidates.OrderBy(c => random.Next()).Take(targetCount);

//            foreach (var region in selected)
//            {
//                warningRegions.Add(region);
//            }
//        }

//        public bool IsRestricted(Region region)
//        {
//            return restrictedRegions.Contains(region);
//        }

//        public bool IsWarning(Region region)
//        {
//            return warningRegions.Contains(region);
//        }
//    }
//}
