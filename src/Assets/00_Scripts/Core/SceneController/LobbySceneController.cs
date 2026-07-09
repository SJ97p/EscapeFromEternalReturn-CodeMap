using System.Collections;
using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.StorageSystem;
using UnityEngine;

namespace HBDinosaur_ER_Project.SceneSystem
{
    public class LobbySceneController : SceneController
    {
        public override GameScene SceneType => GameScene.Lobby;

        [SerializeField] private EquipmentHud equipHud;
        [SerializeField] private PlayerInventoryHud inventoryHud;

        private bool isUIInitialized=false;
        private bool isEntered = false;
        private bool isQuitting;

        private void OnEnable()
        {
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
            SaveManager.Instance.SaveCurrentSave();

            yield return new WaitForSeconds(0.1f); // 필요하면 저장/연출 대기

            Application.Quit();
        }


        private IEnumerator Start()
        {
            yield return WaitUntilStorageReady();

            InitUI();
        }

        public override void Enter()
        {
            // 중복 실행 방지
            if (isEntered) return;

            // base.Enter() 위에서 플래그를 세워줘야 base 내부 로직이나 중복 진입을 완벽히 막습니다.
            isEntered = true;

            base.Enter();

            isUIInitialized = false;

            // 데이터 동기화와 UI 초기화를 담당하는 코루틴만 실행하고, 
            // Enter 직후 즉시 실행되던 SaveManager 호출 코드는 여기서 지웁니다!
            StartCoroutine(WaitAndInitUI());
        }

        private IEnumerator WaitUntilStorageReady()
        {
            while (NewStorageManager.Instance == null ||
                   !NewStorageManager.Instance.IsStorageLoaded)
            {
                yield return null;
            }
        }
        private IEnumerator WaitAndInitUI()
        {
            // 1. 스토리지 데이터가 준비될 때까지 안전하게 대기
            yield return WaitUntilStorageReady();

            // 2. 준비 완료 후 UI 및 슬롯 인스턴스 초기화 실행
            InitUI();

            // ====================================================================
            // [위치 변경] 데이터 세팅과 UI 바인딩이 완벽히 끝난 바로 이 시점!!!
            // 이때 세이브를 해줘야 메모리의 온전한 아이템 데이터가 파일로 직행합니다.
            // ====================================================================
            if (SaveManager.Instance != null)
            {
                Debug.Log("[InGameSceneController] UI 초기화 완료 확인. 안전하게 세이브 및 DB 리프레시를 진행합니다.");
                SaveManager.Instance.LoadAndRefreshOnSceneChanged();
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

        public void LoadInGame()
        {
            SaveManager.Instance.SaveCurrentSave();
            GameSceneManager.Instance.ChangeScene(GameScene.InGame);
        }

        public void QuitGame()
        {
            SaveManager.Instance.SaveCurrentSave();
            GameSceneManager.Instance.QuitGame();
        }
    }
}