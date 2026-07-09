using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI.DragDrop;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.InventoryView;
using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.InventoryRewrite
{
    public class EquipmentAdapter : IItemContainer
    {
        private readonly Storage storage;
        private readonly EquipmentHud hud;

        public ItemContainerType ContainerType => ItemContainerType.Equipment;
        public int Width => storage != null ? storage.Width : 0;
        public int Height => storage != null ? storage.Height : 0;

        public EquipmentAdapter(Storage storage, EquipmentHud hud)
        {
            this.storage = storage;
            this.hud = hud;
        }

        public bool IsValidSlot(int x, int y)
        {
            if (storage == null) return false;
            if (y != 0) return false;

            return storage.IsValidPosition(x, y);
        }

        public bool IsEmpty(int x, int y)
        {
            if (!IsValidSlot(x, y)) return true;

            var slot = storage.GetSlot(x, y);
            return slot == null || slot.IsEmpty;
        }

        public int GetItemId(int x, int y)
        {
            if (!IsValidSlot(x, y)) return -1;

            var slot = storage.GetSlot(x, y);
            if (slot == null || slot.IsEmpty) return -1;

            return slot.ItemId;
        }

        public int GetAmount(int x, int y)
        {
            if (!IsValidSlot(x, y)) return 0;

            var slot = storage.GetSlot(x, y);
            if (slot == null || slot.IsEmpty) return 0;

            return 1;
        }

        // 🛠️ storage.SetItem 및 ClearSlot을 직접 사용하도록 수정
        public bool SetSlot(int x, int y, int itemId, int amount)
        {
            if (!IsValidSlot(x, y)) return false;

            // 아이템 ID가 유효하지 않거나 개수가 0 이하인 경우 -> 장비 해제(Clear)
            if (itemId < 0 || amount <= 0)
            {
                int oldId = GetItemId(x, y);
                storage.ClearSlot(x, y);
                UpdatePlayerStats(oldId, -1);
                return true;
            }

            // ⭐️ 중요: 구조체(Struct) 형태라면 .HasValue 체크를 확실히 해야 합니다.
            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (!itemData.HasValue) return false;

            // ⭐️ 부위 검증 실패 시 절대 등록되면 안 됨
            if (!CanEquipItemToSlot(itemId, x, y))
            {
                Debug.LogWarning($"[EquipmentAdapter] {itemId}번 아이템은 ({x},{y}) 슬롯에 장착할 수 없습니다!");
                return false;
            }

            int oldItemId = GetItemId(x, y);

            // 원본 데이터에 장착
            storage.SetItem(x, y, itemId, 1);
            UpdatePlayerStats(oldItemId, itemId);
            return true;
        }

        // 🛠️ storage.ClearSlot을 직접 사용하도록 수정
        public bool ClearSlot(int x, int y)
        {
            if (!IsValidSlot(x, y)) return false;

            int oldItemId = GetItemId(x, y);

            // 원본 데이터 직접 지우기
            storage.ClearSlot(x, y);

            UpdatePlayerStats(oldItemId, -1);
            return true;
        }

        public bool CanDrop(UIDragPayload payload, int toX, int toY)
        {
            var fromContainer = UIItemMoveManager.Instance.GetContainer(payload.ContainerType);
            if (fromContainer == null) return false;

            int itemId = fromContainer.GetItemId(payload.X, payload.Y);
            if (!TryGetEquipSlotPosition(itemId, out int equipX, out int equipY))
                return false;

            return UIItemMoveManager.Instance.CanMove(
                payload.ContainerType, payload.X, payload.Y,
                ContainerType, equipX, equipY);
        }

        public bool HandleDrop(UIDragPayload payload, int toX, int toY)
        {
            var fromContainer = UIItemMoveManager.Instance.GetContainer(payload.ContainerType);
            if (fromContainer == null)
                return false;

            int fromItemId = fromContainer.GetItemId(payload.X, payload.Y);
            int fromAmount = fromContainer.GetAmount(payload.X, payload.Y);

            if (fromItemId < 0)
                return false;

            if (!TryGetEquipSlotPosition(fromItemId, out int equipX, out int equipY))
                return false;

            // 이미 장착된 것이 있으면 스왑 처리
            if (IsEmpty(equipX, equipY))
            {
                return UIItemMoveManager.Instance.TryMove(
                    payload.ContainerType, payload.X, payload.Y,
                    ContainerType, equipX, equipY);
            }

            // 스왑 대상 아이템 정보 획득
            int equippedItemId = GetItemId(equipX, equipY);
            int equippedAmount = GetAmount(equipX, equipY);

            // 1. 슬롯에 새로운 아이템 장착 (내부에서 storage.SetItem 호출됨)
            bool equipSuccess = SetSlot(equipX, equipY, fromItemId, fromAmount);
            if (!equipSuccess)
                return false;

            // 2. 원래 슬롯에 이전 장착 장비 돌려주기
            bool sourceSetSuccess = fromContainer.SetSlot(payload.X, payload.Y, equippedItemId, equippedAmount);
            if (!sourceSetSuccess)
            {
                // 실패 시 롤백
                SetSlot(equipX, equipY, equippedItemId, equippedAmount);
                return false;
            }

            fromContainer.RefreshUI();
            RefreshUI();
            return true;
        }

        public bool HandleClick(int x, int y)
        {
            // 나 장비창인데, (x, y) 클릭되었으니 인벤토리로 보내줘 요청
            return UIItemMoveManager.Instance.TryAutoMove(ContainerType, x, y);
        }

        private PlayerStat GetPlayerStat()
        {
            var playerObj = GameObject.FindWithTag("Player");
            return playerObj != null ? playerObj.GetComponent<PlayerStat>() : null;
        }

        private void UpdatePlayerStats(int oldItemId, int newItemId)
        {
            if (oldItemId == newItemId) return;

            var playerStat = GetPlayerStat();
            if (playerStat == null) return;

            // 1. 이전 아이템 스탯 차감
            if (oldItemId >= 0)
            {
                var oldItem = ItemDatabase.Instance.GetItemByID(oldItemId);
                if (oldItem.HasValue && oldItem.Value.IsEquippable)
                {
                    playerStat.RemoveEquipmentStats(oldItem.Value.EquipmentData);
                }
            }

            // 2. 새로운 아이템 스탯 반영
            if (newItemId >= 0)
            {
                var newItem = ItemDatabase.Instance.GetItemByID(newItemId);
                if (newItem.HasValue && newItem.Value.IsEquippable)
                {
                    playerStat.ApplyEquipmentStats(newItem.Value.EquipmentData);
                }
            }
        }

        public void ApplyInitialEquippedStats()
        {
            var playerStat = GetPlayerStat();
            if (playerStat == null) return;

            for (int x = 0; x < storage.Width; x++)
            {
                int itemId = GetItemId(x, 0);
                if (itemId >= 0)
                {
                    var item = ItemDatabase.Instance.GetItemByID(itemId);
                    if (item.HasValue && item.Value.IsEquippable)
                    {
                        playerStat.ApplyEquipmentStats(item.Value.EquipmentData);
                    }
                }
            }
        }

        public bool CanEquipItemToSlot(int itemId, int x, int y)
        {
            if (!IsValidSlot(x, y)) return false;
            if (itemId < 0) return true;

            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (!itemData.HasValue) return false; // .HasValue 검증으로 변경

            var equipType = itemData.Value.EquipmentData.EquipmentSlotType;
            if (equipType == EquipmentSlotType.NONE) return false;

            // 슬롯 인덱스별 허용 부위 체크
            switch (x)
            {
                case 0: return equipType == EquipmentSlotType.HELMET;
                case 1: return equipType == EquipmentSlotType.ACCESSORY;
                case 2: return equipType == EquipmentSlotType.ARMOR;
                case 3: return equipType == EquipmentSlotType.WEAPON;
                case 4: return equipType == EquipmentSlotType.SHOES;
                default: return false;
            }
        }

        private bool TryGetEquipSlotPosition(int itemId, out int targetX, out int targetY)
        {
            targetX = -1;
            targetY = 0;

            if (itemId < 0) return false;

            var itemData = ItemDatabase.Instance.GetItemByID(itemId);
            if (itemData == null) return false;

            var equipType = itemData.Value.EquipmentData.EquipmentSlotType;
            if (equipType == EquipmentSlotType.NONE) return false;

            switch (equipType)
            {
                case EquipmentSlotType.HELMET:
                    targetX = 0;
                    return true;
                case EquipmentSlotType.ACCESSORY:
                    targetX = 1;
                    return true;
                case EquipmentSlotType.ARMOR:
                    targetX = 2;
                    return true;
                case EquipmentSlotType.WEAPON:
                    targetX = 3;
                    return true;
                case EquipmentSlotType.SHOES:
                    targetX = 4;
                    return true;
                default:
                    return false;
            }
        }

        public void RefreshUI()
        {
            hud.RefreshAll();
        }
    }
}