using System.Collections;
using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI;
using HBDinosaur_ER_Project.ZoneSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HBDinosaur_ER_Project.SceneSystem
{
    public class InGameSceneController : SceneController
    {
        public override GameScene SceneType { get => GameScene.InGame; }
        public static InGameSceneController Instance { get; private set; }

        [SerializeField] private EquipmentHud equipHud;
        [SerializeField] private PlayerInventoryHud inventoryHud;

        [Header("금지구역 관련")]
        [SerializeField] private List<Region> excludeRegions = new() { Region.None, Region.Laboratory };
        [SerializeField] private int restrictedCountPerCycle = 1;
        [SerializeField] private int escapeableRegionsCount = 5;
        private bool isUIInitialized = false;
        private bool isEntered = false;

        private List<Region> allRegions = new();
        private HashSet<Region> restrictedRegions = new();
        private HashSet<Region> escapeableRegions = new();

        public event Action<HashSet<Region>, ZoneState> OnZoneStateChanged;
        private bool isQuitting;

        private void OnEnable()
        {
            TimeManager.Instance.OnStateChanged += SelectRestrictedRegions;
            Application.wantsToQuit += OnWantsToQuit;
        }

        private void OnDisable()
        {
            Application.wantsToQuit -= OnWantsToQuit;
        }
        private bool OnWantsToQuit()
        {
            if (isQuitting)
                return true;

            StartCoroutine(QuitRoutine());
            return false; // 일단 종료 막기
        }
        private IEnumerator QuitRoutine()
        {
            isQuitting = true;

            // 종료 전에 실행할 함수

            yield return new WaitForSeconds(0.1f); // 필요하면 저장/연출 대기

            Application.Quit();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            TimeManager.Instance.OnStateChanged -= SelectRestrictedRegions;
        }

        private IEnumerator Start()
        {
            // 실제 게임 흐름(다른 씬에서 진입)이라면 Enter()가 먼저 실행되어 isEntered가 true가 됩니다.
            // 하지만 에디터에서 바로 시작했다면 Enter()가 호출되지 않아 기동되지 않으므로, 
            // 한 프레임만 쉬고 스스로 Enter()를 때려줍니다.
            yield return null;

            InitializeRegions();

            if (!isEntered)
            {
                Debug.Log("[InGameSceneController] 에디터 직시작 감지 - 스스로 Enter()를 호출합니다.");
                Enter();
            }
        }

        private void InitializeRegions()
        {
            NewUIManager.Instance.Close(UIPanelId.HyperloopUI);
            NewUIManager.Instance.Close(UIPanelId.Minimap);

            allRegions = Enum.GetValues(typeof(Region))
                .Cast<Region>()
                .Where(r => !excludeRegions.Contains(r))
                .ToList();

            restrictedRegions.Clear();
            escapeableRegions.Clear();

            SelectEscapeableRegions();
        }

        private void SelectEscapeableRegions()
        {
            List<Region> candidates = allRegions
                .Where(r => !restrictedRegions.Contains(r))
                .ToList();

            System.Random random = new System.Random();
            var selected = candidates.OrderBy(c => random.Next()).Take(escapeableRegionsCount);

            foreach (var region in selected)
            {
                escapeableRegions.Add(region);
                Debug.Log($"[InGameSceneController] {region}이 탈출 구역으로 선정됨.");
            }
        }

        private void SelectRestrictedRegions(int day, InGameState state)
        {
            //restrictedRegions.Clear();

            List<Region> candidates = allRegions
                .Where(r => !restrictedRegions.Contains(r))
                .ToList();
            int targetCount = restrictedCountPerCycle;

            if (candidates.Count - targetCount < 1)
            {
                targetCount = candidates.Count - 1;
            }

            if (targetCount <= 0) return;

            System.Random random = new System.Random();
            var selected = candidates.OrderBy(c => random.Next()).Take(targetCount);

            foreach (var region in selected)
            {
                Debug.Log($"[InGameSceneController] {region}이 금지구역으로 설정됨");
                restrictedRegions.Add(region);
            }

            OnZoneStateChanged?.Invoke(restrictedRegions, ZoneState.RestrictedArea);
        }

        public bool IsRestricted(Region region)
        {
            return restrictedRegions.Contains(region);
        }

        public bool IsEscapable(Region region)
        {
            return escapeableRegions.Contains(region);
        }

        public override void Enter()
        {
            // 중복 실행 방지 (Start에서 호출했든, 외부 매니저가 호출했든 최초 1회만 수행)
            if (isEntered) return;

            base.Enter();

            // 씬에 들어왔을 때, 기존에 켜져 있던 플래그를 초기화 (씬 재진입 대비)
            isUIInitialized = false;

            // 데이터가 준비될 때까지 기다렸다가 UI를 켜는 코루틴을 StartCoroutine으로 실행
            StartCoroutine(WaitAndInitUI());
        }
        private IEnumerator WaitAndInitUI()
        {
            // 1. 스토리지 데이터가 준비될 때까지 안전하게 대기
            yield return WaitUntilStorageReady();

            // 2. 준비 완료 후 초기화 실행
            InitUI();
        }

        private IEnumerator WaitUntilStorageReady()
        {
            while (NewStorageManager.Instance == null ||
                   !NewStorageManager.Instance.IsStorageLoaded)
            {
                yield return null;
            }
        }

        private void InitUI()
        {
            if (isUIInitialized)
                return;

            equipHud.Init(NewStorageManager.Instance.EquipmentStorage);
            inventoryHud.Init(NewStorageManager.Instance.PlayerInventory);

            isUIInitialized = true;
        }

        public void LoadLobby()
        {
            SaveManager.Instance.SaveCurrentSave();
            GameSceneManager.Instance.ChangeScene(GameScene.Lobby);
        }

        public void QuitGame()
        {
            SaveManager.Instance.SaveCurrentSave();
            GameSceneManager.Instance.QuitGame();
        }
    }
}
