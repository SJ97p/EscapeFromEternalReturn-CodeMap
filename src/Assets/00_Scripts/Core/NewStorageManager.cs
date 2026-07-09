using System.Collections;
using System.Collections.Generic;
using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using SingletonPattern_Scripts;
using UnityEngine;
using System.Linq;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class NewStorageManager : Singleton<NewStorageManager>
    {
        private Storage playerInventory;
        private Storage equipmentStorage;
        private Storage storage;

        private bool storagesCreated;
        private bool storageLoaded;

        public Storage PlayerInventory => playerInventory;
        public Storage EquipmentStorage => equipmentStorage;
        public Storage Storage => storage;

        public bool IsStorageLoaded => storageLoaded;

        protected override void Awake()
        {
            base.Awake();
            CreateStoragesIfNeeded();
        }

        private IEnumerator Start()
        {
            yield return LoadStorageFromDBWhenReady();
        }

        private void CreateStoragesIfNeeded()
        {
            if (storagesCreated)
                return;

            storage = new Storage(7, 10);
            playerInventory = new Storage(5, 2);
            equipmentStorage = new Storage(5, 1);

            storagesCreated = true;
        }

        private IEnumerator LoadStorageFromDBWhenReady()
        {
            if (storageLoaded)
                yield break;

            while (GameDataStore.Instance == null)
                yield return null;

            SaveManager.Instance.LoadSave(0);

            // 최초 로드 시 DB에서 읽어온 데이터를 각 가방에 분배
            RefreshAllFromDB_Direct(SaveManager.Instance.CurrentStorageData);

            storageLoaded = true;
        }

        /// <summary>
        /// 인게임 런타임 저장소 데이터를 모두 긁어모아 SaveManager의 리스트를 최신화합니다.
        /// </summary>
        public void SaveAllStoragesToMemory()
        {
            if (SaveManager.Instance == null) return;

            int currentSaveId = SaveManager.Instance.CurrentSlotId;

            // 데이터 수집 시작
            List<StorageData> freshData = PrepareCurrentStorageData(currentSaveId);

            // SaveManager의 메모리 버퍼를 최신 데이터로 완전히 교체
            SaveManager.Instance.CurrentStorageData = freshData;
        }

        /// <summary>
        /// 각 가방(Storage)들로부터 현재 배치된 최신 아이템 정보를 추출하여 하나의 리스트로 취합합니다.
        /// </summary>
        public List<StorageData> PrepareCurrentStorageData(int currentSaveId)
        {
            List<StorageData> collectedData = new List<StorageData>();

            // 1. 플레이어 인벤토리 데이터 추출 및 추가
            var inventoryData = playerInventory.ExportToStorageData(StorageType.Inventory, currentSaveId);
            collectedData.AddRange(inventoryData);

            // 2. 일반 창고 데이터 추출 및 추가
            var storageData = storage.ExportToStorageData(StorageType.Storage, currentSaveId);
            collectedData.AddRange(storageData);

            // 3. 장착 장비 창고 데이터 추출 및 추가
            var equipmentData = equipmentStorage.ExportToStorageData(StorageType.EquipmentStorage, currentSaveId);
            collectedData.AddRange(equipmentData);

            Debug.Log($"[NewStorageManager] 인게임 배치 데이터 취합 완료. 총 {collectedData.Count}개 아이템.");
            return collectedData;
        }

        /// <summary>
        /// 전달받은 원본 데이터를 기준으로 인게임 가방(UI 및 드래그 상태)을 실시간 리로드합니다.
        /// </summary>
        public void RefreshAllFromDB_Direct(List<StorageData> sourceData)
        {
            if (sourceData == null) return;

            // 각각의 가방 타입에 맞게 필터링하여 분배 데이터 주입
            playerInventory.LoadFromDB(sourceData.Where(x => x.StorageType == StorageType.Inventory).ToList());
            storage.LoadFromDB(sourceData.Where(x => x.StorageType == StorageType.Storage).ToList());
            equipmentStorage.LoadFromDB(sourceData.Where(x => x.StorageType == StorageType.EquipmentStorage).ToList());

            Debug.Log("[NewStorageManager] 모든 인게임 저장소 가방이 최신 메모리 데이터로 동기화(리로드)되었습니다.");
        }
    }
}