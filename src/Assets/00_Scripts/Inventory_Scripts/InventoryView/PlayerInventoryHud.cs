using HBDinosaur_ER_Project.InventoryLogic;
using UnityEngine;
using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class PlayerInventoryHud : UIPanel
    {
        [SerializeField] private Transform slotRoot;
        [SerializeField] private InventorySlotView slotPrefab;

        private InventorySlotView[] slotViews;


        public InventoryContainerAdapter ContainerAdapter { get; private set; }
        private bool initialized;


        public void Init(Storage storage)
        {
            if (initialized)
            {
                RefreshAll();
                return;
            }

            ContainerAdapter = new InventoryContainerAdapter(storage, this);
            UIItemMoveManager.Instance.RegisterContainer(ContainerAdapter);
            CreateSlots(10);
            RefreshAll();

            initialized = true;
            //  초기화가 끝난 시점에 만약 이 UI가 이미 켜져 있는 상태라면 매니저에 확실히 등록해 줌
            if (gameObject.activeInHierarchy)
            {
                UIItemMoveManager.Instance.SetUIActive(ContainerAdapter.ContainerType, true);
            }
        }

        private void CreateSlots(int count)
        {
            slotViews = new InventorySlotView[count];

            for (int i = 0; i < count; i++)
            {
                var view = Instantiate(slotPrefab, slotRoot);

                int x = i % 5;
                int y = i / 5;

                view.Initialize(x, y);
                slotViews[i] = view;
            }
        }

        public override void RefreshAll()
        {
            if (slotViews == null) return;

            for (int i = 0; i < slotViews.Length; i++)
                slotViews[i].Refresh();
        }
    }
}