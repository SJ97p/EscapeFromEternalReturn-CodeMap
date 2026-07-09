using System.Collections.Generic;
using System.Linq;
using HBDinosaur_ER_Project.Monster;
using HBDinosaur_ER_Project.ZoneSystem;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class MonsterSpawnPoint : MonoBehaviour
    {
        [Header("Type")]
        [SerializeField] private MonsterType _monsterType;
        [SerializeField] private Region region;
        [SerializeField] private ZoneMonsterSpawnTable _spawnTable;

        public List<MonsterSpawnPointData> _monsterSpawnPoints = new();
        public MonsterSpawnPointData monsterData;
        public ItemContainerSpawnPointData itemBoxData;

        private bool _isLoaded = false;
        private void OnValidate()
        {
            if (!_isLoaded) return;
            if (gameObject.transform.parent.TryGetComponent<Zone>(out var zone))
            {
                region = zone.RegionType;
            }           
        }

        private void Start()
        {
            _isLoaded = true;
        }

        public void Init(ZoneMonsterSpawnTable spawnTable)
        {
            _spawnTable = spawnTable;
            _monsterType = GetRandomMonsterType(_spawnTable.Weights);
            SpawnMonsterByType(_monsterType);
            gameObject.SetActive(false);
        }
        public void SpawnMonsterByType(MonsterType monsterType)
        {
            _monsterType = monsterType;

            Vector3 center = transform.position;
            Vector3 leftPos = center + Vector3.left * 1f;
            Vector3 rightPos = center + Vector3.right * 1f;

            _monsterSpawnPoints.Add(new MonsterSpawnPointData(
    monsterType,
    region,
    leftPos,
    transform.rotation,
    transform.localScale
));

            _monsterSpawnPoints.Add(new MonsterSpawnPointData(
                monsterType,
                region,
                rightPos,
                transform.rotation,
                transform.localScale
            ));

            
        }
        private MonsterType GetRandomMonsterType(List<MonsterSpawnWeight> weights)
        {
            int totalWeight = weights.Where(x => x.Weight > 0).Sum(x => x.Weight);

            if (totalWeight <= 0)
            {
                Debug.LogWarning("MonsterSpawner : totalWeight가 0 이하입니다.");
                return weights[0].MonsterType;
            }

            int randomValue = Random.Range(0, totalWeight);

            foreach (var entry in weights)
            {
                if (entry.Weight <= 0)
                    continue;

                if (randomValue < entry.Weight)
                    return entry.MonsterType;

                randomValue -= entry.Weight;
            }

            return weights[weights.Count - 1].MonsterType;
        }
    }

}
