using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.UI;
using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class StoragePanelUI : UIPanel
    {
        [SerializeField] private Transform slotRoot;
        [SerializeField] private StorageSlotView slotPrefab;

        private StorageSlotView[,] slotViews;
        private Storage storage;

        public StorageContainerAdapter ContainerAdapter { get; private set; }
        private bool initialized;

        public override void Open()
        {
            base.Open();
            // 1. 켜질 때마다 매니저로부터 최신 Storage 데이터를 안전하게 동기화합니다.
            FetchStorageReference();

            // 2. 슬롯 생성 및 어댑터 등록이 안 되어 있다면 진행합니다.
            InitializeIfNeeded();

            // 3. 이제 storage가 확실히 존재하므로 갱신이 정상적으로 작동합니다!
            RefreshAll();

            if (ContainerAdapter != null)
                UIItemMoveManager.Instance.SetUIActive(ContainerAdapter.ContainerType, true);
        }

        // 부모의 Close를 확장하여 매니저에 알림
        public override void Close()
        {
            base.Close(); // 부모의 원래 Close 로직 실행

            if (ContainerAdapter != null)
                UIItemMoveManager.Instance.SetUIActive(ContainerAdapter.ContainerType, false);
        }
        private void FetchStorageReference()
        {
            if (NewStorageManager.Instance == null)
            {
                Debug.LogError("[StoragePanelUI] NewStorageManager.Instance is null.");
                return;
            }

            storage = NewStorageManager.Instance.Storage;

            if (storage == null)
            {
                Debug.LogError("[StoragePanelUI] Storage data is null in StorageManager.");
            }
        }
        private void InitializeIfNeeded()
        {
            if (initialized)
                return;

            if (NewStorageManager.Instance == null)
            {
                Debug.LogError("[StoragePanelUI] StorageManager is null.");
                return;
            }

            storage = NewStorageManager.Instance.Storage;

            if (storage == null)
            {
                Debug.LogError("[StoragePanelUI] Storage is null.");
                return;
            }

            ContainerAdapter = new StorageContainerAdapter(storage, this);
            UIItemMoveManager.Instance.RegisterContainer(ContainerAdapter);

            CreateSlots();
            BindSlots();

            initialized = true;
        }

        private void CreateSlots()
        {
            slotViews = new StorageSlotView[storage.Width, storage.Height];

            for (int y = 0; y < storage.Height; y++)
            {
                for (int x = 0; x < storage.Width; x++)
                {
                    StorageSlotView view = Instantiate(slotPrefab, slotRoot);

                    slotViews[x, y] = view;
                }
            }
        }

        private void CreateSlotsIfNeeded()
        {
            if (slotViews != null) return;
            if (storage == null) return;

            slotViews = new StorageSlotView[storage.Width, storage.Height];

            for (int y = 0; y < storage.Height; y++)
            {
                for (int x = 0; x < storage.Width; x++)
                {
                    StorageSlotView view = Instantiate(slotPrefab, slotRoot);
                    slotViews[x, y] = view;
                }
            }
        }

        private void BindSlots()
        {
            Debug.Log(
    $"[StoragePanelUI] BindSlots. " +
    $"storage={storage}, adapter={ContainerAdapter}, slotViewsNull={slotViews == null}",
    this);

            if (storage == null || ContainerAdapter == null || slotViews == null) return;

            for (int y = 0; y < storage.Height; y++)
            {
                for (int x = 0; x < storage.Width; x++)
                {
                    slotViews[x, y].Init(x, y, ContainerAdapter);
                }
            }
        }

        public override void RefreshAll()
        {
            if (storage == null || slotViews == null) return;

            for (int y = 0; y < storage.Height; y++)
            {
                for (int x = 0; x < storage.Width; x++)
                {
                    slotViews[x, y].Refresh();
                }
            }
        }
    }
}