using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SQLite;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public sealed class GameRepositories
    {
        public ItemRepository Items { get; }
        public MonsterSpawnPointRepository MonsterSpawns { get; }
        public ItemContainerSpawnPointRepository ItemContainerSpawns { get; }
        public StorageRepository Storages { get; }
        public SaveFileRepository SaveFiles { get; }

        public GameRepositories(DBLoader dbLoader)
        {
            Items = new ItemRepository(dbLoader.GetConnection("itemdb"));
            MonsterSpawns = new MonsterSpawnPointRepository(dbLoader.GetConnection("monsterspawndatadb"));
            ItemContainerSpawns = new ItemContainerSpawnPointRepository(dbLoader.GetConnection("itemcontainerspawndatadb"));

            // ⭕ 변경: 커넥션을 미리 꺼내지 않고, 실시간 검사를 위해 dbLoader를 원본 그대로 넘깁니다.
            Storages = new StorageRepository(dbLoader);

            SaveFiles = new SaveFileRepository(dbLoader.GetConnection("savefiledb"));
        }
    }
}