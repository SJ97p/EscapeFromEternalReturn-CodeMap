using HBDinosaur_ER_Project.InventoryData;
using HBDinosaur_ER_Project.InventoryLogic;
using HBDinosaur_ER_Project.ItemData;
using System;
using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.ItemData
{
    public class InventoryEventBus
    {
        // E 001 InGame 씬에만 있는 싱글턴 -> 모든 씬이동에 살릴 지 없앨 지 판단해야 함.
        public static InventoryEventBus Instance { get; } = new InventoryEventBus();




        // E 002 아이템 추가
        public event Action<Item, int> OnItemAdded;
        public void PublishItemAdded(Item item, int slotIndex) => OnItemAdded?.Invoke(item, slotIndex);


        // E 003 아이템 제거
        public event Action<Item, int> OnItemRemoved;
        public void PublishItemRemoved(Item item, int slotIndex) => OnItemRemoved?.Invoke(item, slotIndex);


        // E 004 슬롯 단위 변경 (스왑, TakeItem 등)
        public event Action<Inventory, int> OnSlotChanged;
        public void PublishSlotChanged(Inventory inv, int slotIndex) => OnSlotChanged?.Invoke(inv, slotIndex);


        // 장비창 변경
        public event Action<int> OnEquipmentChanged;
        public void PublishEquipmentChanged(int slotIndex) => OnEquipmentChanged?.Invoke(slotIndex);


        // 상호작용 완료
        public event Action<InventoryInteractType> OnInteracted;
        public void PublishInteracted(InventoryInteractType type) => OnInteracted?.Invoke(type);


        // 드래그 시작
        public event Action<DragPayload> OnDragStarted;
        public void PublishDragStarted(DragPayload payload) => OnDragStarted?.Invoke(payload);


        // 드래그 종료
        // dropHandled: true → 슬롯에 정상 드롭됨
        //              false → UI 밖으로 드롭 (버리기 판단 필요)
        public event Action<bool> OnDragEnded;
        public void PublishDragEnded(bool dropHandled) => OnDragEnded?.Invoke(dropHandled);

        // 씬 전환 요청 이벤트
        public event Action<string> OnSceneTransitionRequested;
        public void PublishSceneTransitionRequested(string sceneName) => OnSceneTransitionRequested?.Invoke(sceneName);



        // 씬전환할 때 전부 구독 해제.
        public void Reset()
        {
            OnItemAdded = null;
            OnItemRemoved = null;
            OnSlotChanged = null;
            OnEquipmentChanged = null;
            OnInteracted = null;
            OnDragStarted = null;
            OnDragEnded = null;
            OnSceneTransitionRequested = null;
        }
    }
    public class DragPayload
    {
        public Item item;
        public int slotIndex;
        public InventoryOwnerType ownerType;

        public DragPayload(Item item, int slotIndex, InventoryOwnerType ownerType)
        {
            this.item = item;
            this.slotIndex = slotIndex;
            this.ownerType = ownerType;
        }
    }

    public enum InventoryOwnerType
    {
        PLAYER_INVEN,
        EQUIPMENT_INVEN,
        TARGET_INVEN
    }
}