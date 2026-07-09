using System.Collections.Generic;
using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.ItemData;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class UIItemMoveManager : Singleton<UIItemMoveManager>
    {
        private readonly Dictionary<ItemContainerType, IItemContainer> containers = new();
        private readonly HashSet<ItemContainerType> activeUIStates = new();

        public void RegisterContainer(IItemContainer container)
        {
            if (container == null)
            {
                Debug.LogWarning("[UIItemMoveManager] Register failed. container is null.", this);
                return;
            }

            containers[container.ContainerType] = container;

            Debug.Log(
                $"[UIItemMoveManager] RegisterContainer type={container.ContainerType}, " +
                $"container={container}, count={containers.Count}",
                this);
        }

        public void UnregisterContainer(ItemContainerType containerType)
        {
            bool removed = containers.Remove(containerType);

            Debug.Log(
                $"[UIItemMoveManager] UnregisterContainer type={containerType}, removed={removed}, count={containers.Count}",
                this);
        }
        public void SetUIActive(ItemContainerType type, bool isActive)
        {
            if (isActive)
            {
                activeUIStates.Add(type);
                Debug.Log($"[UIItemMoveManager] UI 활성화됨: {type}");
            }
            else
            {
                activeUIStates.Remove(type);
                Debug.Log($"[UIItemMoveManager] UI 비활성화됨: {type}");
            }
        }

        public IItemContainer GetContainer(ItemContainerType type)
        {
            bool found = containers.TryGetValue(type, out var container);

            Debug.Log(
                $"[UIItemMoveManager] GetContainer type={type}, found={found}, container={container}",
                this);

            return container;
        }

        public bool CanMove(
            ItemContainerType fromType, int fromX, int fromY,
            ItemContainerType toType, int toX, int toY)
        {
            Debug.Log(
                $"[UIItemMoveManager] CanMove {fromType}({fromX},{fromY}) -> {toType}({toX},{toY})",
                this);

            IItemContainer from = GetContainer(fromType);
            IItemContainer to = GetContainer(toType);

            if (from == null || to == null)
            {
                Debug.LogWarning($"[UIItemMoveManager] CanMove false. from={from}, to={to}", this);
                return false;
            }

            if (!from.IsValidSlot(fromX, fromY))
            {
                Debug.LogWarning("[UIItemMoveManager] CanMove false. invalid from slot.", this);
                return false;
            }

            if (!to.IsValidSlot(toX, toY))
            {
                Debug.LogWarning("[UIItemMoveManager] CanMove false. invalid to slot.", this);
                return false;
            }

            if (from.IsEmpty(fromX, fromY))
            {
                Debug.LogWarning("[UIItemMoveManager] CanMove false. from is empty.", this);
                return false;
            }

            if (fromType == toType && fromX == toX && fromY == toY)
            {
                Debug.LogWarning("[UIItemMoveManager] CanMove false. same slot.", this);
                return false;
            }

            if (CanMerge(from, fromX, fromY, to, toX, toY))
            {
                Debug.Log("[UIItemMoveManager] CanMove true. merge.", this);
                return true;
            }

            if (fromType == toType)
            {
                Debug.Log("[UIItemMoveManager] CanMove true. same container swap.", this);
                return true;
            }
            if (toType == ItemContainerType.Equipment && to is EquipmentAdapter equipmentTo)
            {
                int fromItemId = from.GetItemId(fromX, fromY);

                if (!equipmentTo.CanEquipItemToSlot(fromItemId, toX, toY))
                {
                    Debug.LogWarning($"[UIItemMoveManager] CanMove false. Item {fromItemId} cannot equip to slot ({toX},{toY}).", this);
                    return false;
                }
            }

            if (fromType == ItemContainerType.Equipment && from is EquipmentAdapter equipmentFrom && !to.IsEmpty(toX, toY))
            {
                int toItemId = to.GetItemId(toX, toY);

                if (!equipmentFrom.CanEquipItemToSlot(toItemId, fromX, fromY))
                {
                    Debug.LogWarning($"[UIItemMoveManager] CanMove false. Item {toItemId} cannot equip to source equipment slot ({fromX},{fromY}).", this);
                    return false;
                }
            }

            // 목적지 슬롯이 비어있지 않은 경우 (스왑이 필요한 상황)
            if (!to.IsEmpty(toX, toY))
            {
                // 인벤토리 <-> 장비창 간의 스왑인지 확인
                bool isInventoryEquipmentSwap =
                    (fromType == ItemContainerType.Inventory && toType == ItemContainerType.Equipment) ||
                    (fromType == ItemContainerType.Equipment && toType == ItemContainerType.Inventory);

                if (isInventoryEquipmentSwap)
                {
                    int fromItemId = from.GetItemId(fromX, fromY);
                    int toItemId = to.GetItemId(toX, toY);

                    // 장비창(Equipment) 방향으로 들어가는 아이템이 장착 가능한 장비인지 검증
                    if (toType == ItemContainerType.Equipment && !IsEquippableItem(fromItemId))
                    {
                        Debug.LogWarning($"[UIItemMoveManager] CanMove false. Item {fromItemId} is not equippable.", this);
                        return false;
                    }

                    if (fromType == ItemContainerType.Equipment && !IsEquippableItem(toItemId))
                    {
                        Debug.LogWarning($"[UIItemMoveManager] CanMove false. Item {toItemId} is not equippable.", this);
                        return false;
                    }

                    Debug.Log("[UIItemMoveManager] CanMove true. Inventory <-> Equipment swap matching.", this);
                    return true;
                }

                Debug.LogWarning("[UIItemMoveManager] CanMove false. target not empty.", this);
                return false;
            }

            Debug.Log("[UIItemMoveManager] CanMove true. move to empty slot.", this);
            return true;
        }

        public bool TryMove(
            ItemContainerType fromType, int fromX, int fromY,
            ItemContainerType toType, int toX, int toY)
        {
            Debug.Log(
                $"[UIItemMoveManager] TryMove {fromType}({fromX},{fromY}) -> {toType}({toX},{toY})",
                this);

            IItemContainer from = GetContainer(fromType);
            IItemContainer to = GetContainer(toType);

            if (from == null || to == null)
            {
                Debug.LogWarning($"[UIItemMoveManager] TryMove false. from={from}, to={to}", this);
                return false;
            }

            if (TryMerge(from, fromX, fromY, to, toX, toY))
            {
                Debug.Log("[UIItemMoveManager] TryMove success. merged.", this);
                return true;
            }

            if (!CanMove(fromType, fromX, fromY, toType, toX, toY))
            {
                Debug.LogWarning("[UIItemMoveManager] TryMove false. CanMove failed.", this);
                return false;
            }

            int fromItemId = from.GetItemId(fromX, fromY);
            int fromAmount = from.GetAmount(fromX, fromY);
            int toItemId = to.GetItemId(toX, toY);
            int toAmount = to.GetAmount(toX, toY);

            // 같은 창끼리 움직이거나, 목적지 슬롯이 비어있지 않다면 '스왑(Swap)' 연산 수행
            if (fromType == toType || !to.IsEmpty(toX, toY))
            {
                bool setTo = to.SetSlot(toX, toY, fromItemId, fromAmount);
                if (!setTo)
                    return false;

                bool setFrom = from.SetSlot(fromX, fromY, toItemId, toAmount);
                if (!setFrom)
                {
                    // rollback
                    to.SetSlot(toX, toY, toItemId, toAmount);
                    return false;
                }

                from.RefreshUI();
                if (from != to)
                    to.RefreshUI();

                return true;
            }

            // 목적지가 완전히 비어있는 순수 이동 (기존 로직 유지)
            bool setTarget = to.SetSlot(toX, toY, fromItemId, fromAmount);
            if (!setTarget)
            {
                Debug.LogWarning("[UIItemMoveManager] TryMove false. setTarget failed.", this);
                return false;
            }

            bool clearSource = from.ClearSlot(fromX, fromY);
            if (!clearSource)
            {
                Debug.LogWarning("[UIItemMoveManager] TryMove false. clearSource failed. rollback.", this);
                to.ClearSlot(toX, toY);
                return false;
            }

            from.RefreshUI();
            to.RefreshUI();
            return true;
        }


        private int GetMaxStack(int itemId)
        {
            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (itemData == null) return 1;

            if (!itemData.Value.IsStackable)
                return 1;

            return itemData.Value.MaxStack;
        }
        private bool IsStackable(int itemId)
        {
            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (itemData == null) return false;

            return itemData.Value.IsStackable;
        }

        private bool CanMerge(IItemContainer from, int fromX, int fromY, IItemContainer to, int toX, int toY)
        {
            if (from == null || to == null)
                return false;

            if (!from.IsValidSlot(fromX, fromY) || !to.IsValidSlot(toX, toY))
                return false;

            if (from.IsEmpty(fromX, fromY) || to.IsEmpty(toX, toY))
                return false;

            int fromItemId = from.GetItemId(fromX, fromY);
            int toItemId = to.GetItemId(toX, toY);

            if (fromItemId < 0 || toItemId < 0)
                return false;

            if (fromItemId != toItemId)
                return false;

            if (!IsStackable(fromItemId))
                return false;

            int toAmount = to.GetAmount(toX, toY);
            int maxStack = GetMaxStack(toItemId);

            return toAmount < maxStack;
        }

        private bool TryMerge(IItemContainer from, int fromX, int fromY, IItemContainer to, int toX, int toY)
        {
            if (!CanMerge(from, fromX, fromY, to, toX, toY))
                return false;

            int itemId = from.GetItemId(fromX, fromY);
            int fromAmount = from.GetAmount(fromX, fromY);
            int toAmount = to.GetAmount(toX, toY);
            int maxStack = GetMaxStack(itemId);

            int space = maxStack - toAmount;
            int moveAmount = fromAmount < space ? fromAmount : space;

            int newToAmount = toAmount + moveAmount;
            int remainAmount = fromAmount - moveAmount;

            bool targetSet = to.SetSlot(toX, toY, itemId, newToAmount);
            if (!targetSet)
                return false;

            if (remainAmount <= 0)
            {
                if (!from.ClearSlot(fromX, fromY))
                    return false;
            }
            else
            {
                if (!from.SetSlot(fromX, fromY, itemId, remainAmount))
                    return false;
            }

            from.RefreshUI();
            to.RefreshUI();
            return true;
        }
        /// <summary>
        /// 아이템 클릭 시 현재 열린 UI 상태와 아이템 종류에 따라 적절한 목적지를 찾아 이동시킵니다.
        /// </summary>
        public bool TryAutoMove(ItemContainerType fromType, int fromX, int fromY)
        {
            IItemContainer fromContainer = GetContainer(fromType);
            if (fromContainer == null || !fromContainer.IsValidSlot(fromX, fromY) || fromContainer.IsEmpty(fromX, fromY))
                return false;

            int itemId = fromContainer.GetItemId(fromX, fromY);
            List<ItemContainerType> targetCandidates = new();

            switch (fromType)
            {
                case ItemContainerType.Loot:     // 1. 루팅 창에서 클릭
                case ItemContainerType.Storage:  // 2. 창고 창에서 클릭
                case ItemContainerType.Equipment:// 3. 장비 창에서 클릭 (무조건 인벤토리로 해제)
                    targetCandidates.Add(ItemContainerType.Inventory);
                    break;

                case ItemContainerType.Inventory: // 4. 인벤토리에서 클릭
                    // 규칙 A: Loot(루팅창)가 열려 있다면 최우선
                    if (IsUIOpen(ItemContainerType.Loot))
                    {
                        targetCandidates.Add(ItemContainerType.Loot);
                    }
                    // 규칙 B: 창고(Storage)가 열려 있다면 창고가 다음 순위
                    else if (IsUIOpen(ItemContainerType.Storage))
                    {
                        targetCandidates.Add(ItemContainerType.Storage);
                    }
                    // 규칙 C: 창고/루팅창이 다 닫혀있고 장착 가능한 아이템이면 장비창(Equipment)으로 이동
                    else if (IsEquippableItem(itemId))
                    {
                        targetCandidates.Add(ItemContainerType.Equipment);
                    }
                    break;
            }

            // 결정된 후보지들을 순서대로 돌며 빈자리 탐색 및 스왑 가능 공간 확인
            foreach (var toType in targetCandidates)
            {
                if (!IsUIOpen(toType)) continue;

                IItemContainer toContainer = GetContainer(toType);
                if (toContainer == null) continue;

                int targetWidth = toContainer.Width;
                int targetHeight = toContainer.Height;

                // 인벤토리 -> 장비창으로 자동 장착(스왑 포함) 처리를 하려는 경우
                if (fromType == ItemContainerType.Inventory && toType == ItemContainerType.Equipment)
                {
                    for (int y = 0; y < targetHeight; y++)
                    {
                        for (int x = 0; x < targetWidth; x++)
                        {
                            if (toContainer.IsValidSlot(x, y))
                            {
                                // IsEmpty 체크를 건너뛰고 장비창 어댑터 세부 검증(CanMove)에 합치하면 바로 교체 장착 수행
                                if (CanMove(fromType, fromX, fromY, toType, x, y))
                                {
                                    if (TryMove(fromType, fromX, fromY, toType, x, y))
                                    {
                                        Debug.Log($"[UIItemMoveManager] 장비 교체 장착(AutoMove-Swap) 성공: {fromType}({fromX},{fromY}) <-> {toType}({x},{y})");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 일반적인 경우: 목적지의 비어있는 슬롯을 찾아 이동 (루팅, 창고 입고, 장비 해제 등)
                    for (int y = 0; y < targetHeight; y++)
                    {
                        for (int x = 0; x < targetWidth; x++)
                        {
                            if (toContainer.IsValidSlot(x, y) && toContainer.IsEmpty(x, y))
                            {
                                if (CanMove(fromType, fromX, fromY, toType, x, y))
                                {
                                    if (TryMove(fromType, fromX, fromY, toType, x, y))
                                    {
                                        Debug.Log($"[UIItemMoveManager] AutoMove 성공: {fromType}({fromX},{fromY}) -> {toType}({x},{y})");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.LogWarning($"[UIItemMoveManager] {fromType}에서 보낼 수 있는 적절한 공간이 목적지 후보군에 없습니다.");
            return false;
        }

        /// <summary>
        /// 아이템 데이터베이스를 조회하여 해당 아이템이 장착 가능한지 확인하는 헬퍼 메서드
        /// </summary>
        private bool IsEquippableItem(int itemId)
        {
            if (itemId < 0) return false;
            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            return itemData.HasValue && itemData.Value.IsEquippable;
        }

        public bool IsUIOpen(ItemContainerType type)
        {
            if (type == ItemContainerType.Inventory) return true;
            return activeUIStates.Contains(type);
        }
    }
}