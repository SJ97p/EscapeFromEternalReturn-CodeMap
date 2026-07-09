using System.Collections;
using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI;
using HBDinosaur_ER_Project.UI.DragDrop;
using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.InventoryView
{
    public class TargetInventoryHud : UIPanel, IDataInjectable<Storage>
    {
        [Header("Grid")]
        [SerializeField] private LootSlotView slotPrefab;
        [SerializeField] private Transform slotRoot;

        [Header("UI")]
        [SerializeField] private Image imageCooldownProgress;

        private int width = 4;
        private int height = 3;
        private LootSlotView[] slotViews;
        private TargetInventoryContainerAdapter containerAdapter;
        private Storage _targetStorage;
        private Coroutine autoPickupCoroutine;
        private bool slotsCreated;

        public TargetInventoryContainerAdapter ContainerAdapter => containerAdapter;
        public Storage TargetStorage => _targetStorage;

        public void Bind(Storage storage)
        {
            if (storage == null)
                return;

            // 1. 기존에 등록된 것이 있다면 확실하게 언바인드 및 매니저 해제 유도
            Unbind();

            _targetStorage = storage;

            // 이 레이아웃의 고정 크기(width, height) 대신 실제 storage의 크기를 따라가도록 수정하는 것이 안전합니다.
            containerAdapter = new TargetInventoryContainerAdapter(_targetStorage, this, _targetStorage.Width, _targetStorage.Height);

            // 2. 매니저에 확실하게 등록
            UIItemMoveManager.Instance.RegisterContainer(containerAdapter);

            CreateSlots();
            BindSlots();
            StartAutoPickup();
            RefreshAll();
        }

        public void Unbind()
        {
            StopAutoPickup();

            if (imageCooldownProgress != null)
                imageCooldownProgress.fillAmount = 0f;

            // 3. 안전하게 매니저에서 제거 (Instance가 살아있는지 검증)
            if (UIItemMoveManager.Instance != null)
            {
                UIItemMoveManager.Instance.UnregisterContainer(ItemContainerType.Loot);
            }

            _targetStorage = null;
            containerAdapter = null;

            RefreshAll();
        }
        protected virtual void OnEnable()
        {
            // UI가 화면에 켜질 때 매니저에게 "나 지금 열렸어!" 라고 알림
            if (UIItemMoveManager.Instance != null)
            {
                UIItemMoveManager.Instance.SetUIActive(ItemContainerType.Loot, true);
            }
        }

        public override void Close()
        {
            // Close가 호출될 때도 확실하게 꺼짐 상태 전파 및 언바인드
            if (UIItemMoveManager.Instance != null)
            {
                UIItemMoveManager.Instance.SetUIActive(ItemContainerType.Loot, false);
            }
            Unbind();
            base.Close();
        }

        private void CreateSlots()
        {
            if (slotsCreated)
                return;

            int count = width * height;
            slotViews = new LootSlotView[count];

            for (int i = 0; i < count; i++)
            {
                LootSlotView view = Instantiate(slotPrefab, slotRoot);

                int x = i % width;
                int y = i / width;

                view.Init(x, y, containerAdapter);
                slotViews[i] = view;
            }

            slotsCreated = true;
        }

        private void BindSlots()
        {
            if (slotViews == null)
                return;

            for (int i = 0; i < slotViews.Length; i++)
            {
                int x = i % width;
                int y = i / width;
                slotViews[i].Init(x, y, containerAdapter);
            }
        }

        private void StartAutoPickup()
        {
            StopAutoPickup();
            autoPickupCoroutine = StartCoroutine(AutoPickupCoroutine());
        }

        private void StopAutoPickup()
        {
            if (autoPickupCoroutine == null)
                return;

            StopCoroutine(autoPickupCoroutine);
            autoPickupCoroutine = null;
        }

        private IEnumerator AutoPickupCoroutine()
        {
            if (_targetStorage == null)
                yield break;

            if (!HasExtractableItem())
                yield break;

            float localTimer = 0f;

            while (localTimer <= 1f)
            {
                localTimer += Time.deltaTime;

                if (imageCooldownProgress != null)
                    imageCooldownProgress.fillAmount = localTimer / 1f;

                yield return null;
            }

            if (imageCooldownProgress != null)
                imageCooldownProgress.fillAmount = 0f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    localTimer = 0f;

                    var slot = _targetStorage.GetSlot(x, y);
                    if (slot == null || slot.IsEmpty)
                        continue;

                    var itemData = ItemDatabase.Instance.GetItemByID(slot.ItemId);
                    if (itemData == null || !itemData.Value.IsExtractable)
                        continue;

                    if (!TryFindInventoryTargetSlot(x, y, out int toX, out int toY))
                        continue;

                    bool moved = UIItemMoveManager.Instance.TryMove(
                        ItemContainerType.Loot, x, y,
                        ItemContainerType.Inventory, toX, toY);

                    if (!moved)
                        continue;

                    while (localTimer <= 0.2f)
                    {
                        localTimer += Time.deltaTime;

                        if (imageCooldownProgress != null)
                            imageCooldownProgress.fillAmount = localTimer / 0.2f;

                        yield return null;
                    }

                    if (imageCooldownProgress != null)
                        imageCooldownProgress.fillAmount = 0f;

                    y = 0;
                    x = -1;
                }
            }

            if (imageCooldownProgress != null)
                imageCooldownProgress.fillAmount = 0f;

            autoPickupCoroutine = null;
        }

        private bool HasExtractableItem()
        {
            if (_targetStorage == null)
                return false;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var slot = _targetStorage.GetSlot(x, y);
                    if (slot == null || slot.IsEmpty)
                        continue;

                    var itemData = ItemDatabase.Instance.GetItemByID(slot.ItemId);
                    if (itemData == null)
                        continue;

                    if (itemData.Value.IsExtractable)
                        return true;
                }
            }

            return false;
        }

        private bool TryFindInventoryTargetSlot(int fromX, int fromY, out int toX, out int toY)
        {
            toX = -1;
            toY = -1;

            var inventoryContainer = UIItemMoveManager.Instance.GetContainer(ItemContainerType.Inventory);
            if (inventoryContainer == null)
                return false;

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (UIItemMoveManager.Instance.CanMove(
                        ItemContainerType.Loot, fromX, fromY,
                        ItemContainerType.Inventory, x, y))
                    {
                        toX = x;
                        toY = y;
                        return true;
                    }
                }
            }

            return false;
        }
        public void InjectData(Storage data)
        {
            // 상자나 몬스터의 저장소 데이터를 바인딩합니다.
            Bind(data);
        }

        public override void RefreshAll()
        {
            if (slotViews == null)
                return;

            for (int i = 0; i < slotViews.Length; i++)
                slotViews[i].Refresh();
        }
    }
}
