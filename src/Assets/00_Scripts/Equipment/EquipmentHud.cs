using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class EquipmentHud : UIPanel
    {
        [SerializeField] private Transform slotRoot;
        [SerializeField] private EquipmentSlotView slotPrefab;

        private EquipmentSlotView[] slotViews;

        public Storage Equipment { get; private set; }
        public EquipmentAdapter ContainerAdapter { get; private set; }
        private bool initialized;

        public void Init(Storage storage)
        {
            if (initialized)
            {
                RefreshAll();
                return;
            }

            Equipment = storage;
            ContainerAdapter = new EquipmentAdapter(storage, this);

            UIItemMoveManager.Instance.RegisterContainer(ContainerAdapter);

            CreateSlotsIfNeeded();
            BindSlots();
            RefreshAll();

            ContainerAdapter.ApplyInitialEquippedStats();
            initialized = true;

            //  초기화가 끝난 시점에 만약 이 UI가 이미 켜져 있는 상태라면 매니저에 확실히 등록해 줌
            if (gameObject.activeInHierarchy)
            {
                UIItemMoveManager.Instance.SetUIActive(ContainerAdapter.ContainerType, true);
            }
        }

        private void CreateSlotsIfNeeded()
        {
            if (Equipment == null) return;
            if (slotViews != null) return;

            int count = Equipment.Width;
            slotViews = new EquipmentSlotView[count];

            for (int i = 0; i < count; i++)
            {
                var view = Instantiate(slotPrefab, slotRoot);
                slotViews[i] = view;
            }
        }

        private void BindSlots()
        {
            if (Equipment == null || ContainerAdapter == null || slotViews == null) return;

            for (int i = 0; i < slotViews.Length; i++)
            {
                int x = i;
                int y = 0;

                slotViews[i].Init(x, y, ContainerAdapter);
            }
        }

        public override void RefreshAll()
        {
            if (slotViews == null) return;

            for (int i = 0; i < slotViews.Length; i++)
            {
                slotViews[i].Refresh();
            }
        }
        //  부모의 Open을 확장하여 매니저에 알림
        public override void Open()
        {
            base.Open(); // 부모의 원래 Open 로직 실행 (예: 트윈 애니메이션, SetActive 등)

            if (ContainerAdapter != null)
                UIItemMoveManager.Instance.SetUIActive(ContainerAdapter.ContainerType, true);
        }

        //  부모의 Close를 확장하여 매니저에 알림
        public override void Close()
        {
            base.Close(); // 부모의 원래 Close 로직 실행

            if (ContainerAdapter != null)
                UIItemMoveManager.Instance.SetUIActive(ContainerAdapter.ContainerType, false);
        }
    }
}