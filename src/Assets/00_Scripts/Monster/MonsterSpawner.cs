using System.Collections.Generic;
using System.Linq;
using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

namespace HBDinosaur_ER_Project.Monster
{
    public class MonsterSpawner : MonoBehaviour
    {
        public List<GameObject> Spawn(List<MonsterSpawnPointData> spawnPoints, Transform parent)
        {
            List<GameObject> result = new();

            foreach (var spawnPoint in spawnPoints)
            {
                GameObject monster = SpawnOne(spawnPoint);
                if (monster == null)
                    continue;

                monster.transform.SetParent(parent, true);
                result.Add(monster);
            }

            return result;
        }

        private GameObject SpawnOne(MonsterSpawnPointData spawnPoint)
        {
            var monsterData = GameDataStore.Instance.MonsterDatabase.monsters
        .FirstOrDefault(x => x.Type == spawnPoint.MonsterType);

            if (monsterData == null)
            {
                Debug.LogError($"MonsterType {spawnPoint.MonsterType} not found.");
                return null;
            }

            GameObject monsterObj = PoolManager.Instance.Get(monsterData.Prefab);


            if (!monsterObj.TryGetComponent<MonsterController>(out var controller))
            {
                Debug.Log("monsterObj에 MonsterController가 없습니다.");
                monsterObj.transform.SetPositionAndRotation(spawnPoint.Position, spawnPoint.Rotation);
            }
            controller.InitializeData(spawnPoint);

            return monsterObj;
        }
    }
}

