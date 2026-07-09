using HBDinosaur_ER_Project.InventoryData;
using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.InventoryView;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.StorageSystem;
using UnityEngine;
using HBDinosaur_ER_Project.InventoryRewrite;
using UnityEngine.EventSystems;

namespace HBDinosaur_ER_Project.InventoryLogic
{
    public class InventoryUI : MonoBehaviour
    {
        private PlayerInventoryHud playerInventoryHud;
        private EquipmentHud2 EquipmentHud2;
        private TargetInventoryHud targetInventoryHud;
        private DragFollowIcon dragGhost;
        [SerializeField] GameObject storage;

        public Inventory PlayerInventory { get; private set; }
        public Equipment2 Equipment { get; private set; }

        private DragPayload currentDrag;
        private bool dropHandledThisFrame;

        private void Awake()
        {
            playerInventoryHud = GetComponentInChildren<PlayerInventoryHud>();
            EquipmentHud2 = GetComponentInChildren<EquipmentHud2>();
            targetInventoryHud = GetComponentInChildren<TargetInventoryHud>();
            dragGhost = GetComponentInChildren<DragFollowIcon>(true);
        }

        private void OnDestroy()
        {
            if (InventoryEventBus.Instance == null) return;
            InventoryEventBus.Instance.OnDragStarted -= OnDragStarted;
            InventoryEventBus.Instance.OnDragEnded -= OnDragEnded;
            InventoryEventBus.Instance.OnSceneTransitionRequested -= OnSceneTransitionRequested;
        }

        public void Init(Inventory playerInventory, Equipment2 equipment)
        {
            PlayerInventory = playerInventory;
            Equipment = equipment;

            //playerInventoryHud.Init(playerInventory); // 1번
            EquipmentHud2.Init(equipment);
            //targetInventoryHud.Close();

            InventoryEventBus.Instance.OnDragStarted += OnDragStarted;
            InventoryEventBus.Instance.OnDragEnded += OnDragEnded;
            InventoryEventBus.Instance.OnSceneTransitionRequested += OnSceneTransitionRequested;
        }

        private void OnSceneTransitionRequested(string sceneName)
        {
            // Storage로 이동은 SceneTransitionButton에서 처리
        }

        public void RegisterDragStart(DragPayload payload)
        {
            currentDrag = payload;
            dropHandledThisFrame = false;
        }

        public void HandleDrop(InventoryButton targetSlot)
        {
            if (currentDrag == null) return;

            if (targetSlot.SlotIndex == currentDrag.slotIndex &&
                targetSlot.OwnerType == currentDrag.ownerType)
            {
                dropHandledThisFrame = true;
                currentDrag = null;
                return;
            }

            if (!targetSlot.CanAcceptDrop(currentDrag))
            {
                dropHandledThisFrame = true;
                currentDrag = null;
                return;
            }

            targetSlot.OnReceiveDrop(currentDrag);
            dropHandledThisFrame = true;
            currentDrag = null;
        }

        public void HandleDragEnd()
        {
            if (currentDrag == null) return;

            if (!dropHandledThisFrame && !EventSystem.current.IsPointerOverGameObject())
                HandleDropOutside(currentDrag);

            currentDrag = null;
            dropHandledThisFrame = false;
        }

        private void HandleDropOutside(DragPayload drag)
        {
            switch (drag.ownerType)
            {
                case InventoryOwnerType.PLAYER_INVEN:
                    PlayerInventory.RemoveItem(drag.slotIndex);
                    InventoryEventBus.Instance.PublishInteracted(InventoryInteractType.DROPITEM);
                    break;
                case InventoryOwnerType.EQUIPMENT_INVEN:
                    Equipment.Unequip(drag.slotIndex);
                    InventoryEventBus.Instance.PublishInteracted(InventoryInteractType.DROPITEM);
                    break;
                case InventoryOwnerType.TARGET_INVEN:
                    break;
            }
        }

        private void OnDragStarted(DragPayload payload)
        {
            currentDrag = payload;
            dropHandledThisFrame = false;
        }

        private void OnDragEnded(bool dropHandled)
        {
            if (currentDrag == null) return;
            if (!dropHandled) HandleDropOutside(currentDrag);
            currentDrag = null;
            dropHandledThisFrame = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                storage.SetActive(!storage.activeSelf);
            }
        }
    }
}