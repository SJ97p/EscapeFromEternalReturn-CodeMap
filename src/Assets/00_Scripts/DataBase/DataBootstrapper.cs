using HBDinosaur_ER_Project.Core;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class DataBootstrapper : MonoBehaviour
    {
        private DBLoader _dbLoader;
        public GameDataStore GameData { get; private set; }

        private bool isLoaded;
        void Awake()
        {
            if (isLoaded) return;

            // ⭕ [추가] 인게임 씬 등 다른 씬으로 넘어가도 이 매니저 오브젝트가 파괴되지 않도록 보호합니다.
            //DontDestroyOnLoad(gameObject);

            _dbLoader = GetComponent<DBLoader>();
            _dbLoader.ConnectSQLite();

            // 만약 SQLite에서 긁어온 정적 테이블(마스터 데이터)이 필요하다면 여기서 생성
            var repos = new GameRepositories(_dbLoader);
            // 세이브 매니저에 레포지토리 주입
            SaveManager.Instance.Initialize(repos);

            // TODO: 만약 기획 데이터 테이블(아이템 스탯, 몬스터 스탯 등)이 필요하다면 
            // 별도의 StaticDataManager.Instance.Initialize(repos) 형태로 넘겨주는 것이 좋습니다.

            isLoaded = true;
        }
    }
}