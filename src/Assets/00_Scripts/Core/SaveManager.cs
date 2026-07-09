using System.Collections.Generic;
using System.Linq;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.StorageSystem;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.Core
{
    public class SaveManager : Singleton<SaveManager>
    {
        private GameRepositories _repos;
        private int _currentSlotId = 0;

        public int CurrentSlotId => _currentSlotId;

        // 인게임 런타임에서 사용할 현재 슬롯의 창고 데이터 리스트
        public List<StorageData> CurrentStorageData { get; set; } = new();

        public void Initialize(GameRepositories repos)
        {
            _repos = repos;
        }

        /// <summary>
        /// 특정 슬롯(SaveId)에 해당하는 창고 데이터를 SQLite DB에서 로드합니다.
        /// </summary>
        public void LoadSave(int slotId)
        {
            if (_repos == null)
            {
                Debug.LogError("[SaveManager] 레포지토리가 초기화되지 않았습니다.");
                return;
            }

            _currentSlotId = slotId;

            // SQLite DB에서 해당 SaveId를 가진 데이터만 조회해서 리스트로 보관
            CurrentStorageData = _repos.Storages.GetBySaveId(slotId).ToList();

            Debug.Log($"[SaveManager] {slotId}번 슬롯 창고 데이터 로드 완료. 아이템 개수: {CurrentStorageData.Count}");
        }

        /// <summary>
        /// 인게임에서 변경된 현재 창고 데이터를 SQLite DB에 덮어씁니다.
        /// </summary>
        public void SaveCurrentSave()
        {
            if (_repos == null) return;

            // 빌드 혹은 일반 저장 상황에서 꼬이지 않도록, 저장 직전에 인게임 런타임 가방 데이터를 메모리로 긁어옵니다.
            if (NewStorageManager.Instance != null)
            {
                NewStorageManager.Instance.SaveAllStoragesToMemory();
            }

            if (CurrentStorageData == null) return;

            // 데이터 일관성을 위해 기존 DB에 있던 이 슬롯의 데이터들을 한번 밀어버립니다.
            _repos.Storages.DeleteAllBySaveId(_currentSlotId);

            // 현재 런타임에 들고 있는 데이터를 리스트 순회하며 새로 Insert(Add)
            foreach (var data in CurrentStorageData)
            {
                _repos.Storages.Add(data);
            }

            Debug.Log($"[SaveManager] {_currentSlotId}번 슬롯 DB 최종 저장 완료.");
        }

        /// <summary>
        /// 새 슬롯을 위한 빈 데이터 세팅
        /// </summary>
        public void CreateNewGame(int slotId)
        {
            _currentSlotId = slotId;
            CurrentStorageData = new List<StorageData>();

            // 초기 지급 아이템 등이 있다면 여기에 _repos.Storages.Add() 혹은 리스트에 추가 가능

            SaveCurrentSave();
        }

        /// <summary>
        /// 해당 슬롯의 창고 데이터를 DB에서 통째로 지웁니다.
        /// </summary>
        public void DeleteSlot(int slotId)
        {
            if (_repos == null) return;

            _repos.Storages.DeleteAllBySaveId(slotId);

            if (_currentSlotId == slotId)
            {
                CurrentStorageData.Clear();
            }
            Debug.Log($"[SaveManager] {slotId}번 슬롯 DB 데이터 삭제 완료.");
        }

        public void LoadAndRefreshOnSceneChanged()
        {
            Debug.Log("[SaveManager] 씬 전환이 감지되어 파일로부터 데이터를 로드하고 시스템을 완전 초기화합니다.");

            SaveCurrentSave();

            // 2. 저장소 매니저가 존재한다면 DB에서 데이터를 역으로 로드하여 드래그앤드롭 및 슬롯 컴포넌트들을 완전 리셋/정렬합니다.
            if (NewStorageManager.Instance != null)
            {
                // 보낸 데이터(CurrentStorageData 등 명칭에 맞게 괄호 안에 대입) 기반 리프레시
                NewStorageManager.Instance.RefreshAllFromDB_Direct(CurrentStorageData);
                Debug.Log("[SaveManager] NewStorageManager.RefreshAllFromDB_Direct 호출 성공. 씬 데이터 동기화 완료.");
            }
            else
            {
                Debug.LogWarning("[SaveManager] NewStorageManager 인스턴스를 찾을 수 없어 UI 동기화를 건너跳니다.");
            }
        }
    }
}