using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.ItemLogic;
using HBDinosaur_ER_Project.Monster;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HBDinosaur_ER_Project.ZoneSystem
{
    public class Zone : MonoBehaviour
    {
        [Header("Zone Settings")]
        [SerializeField] private Region _regionType = Region.Factory;
        [SerializeField] private Transform _monsterRoot;
        [SerializeField] private Transform _itemContainerRoot;
        [SerializeField] private ZoneMonsterSpawnTable _spawnTable;
        [SerializeField] private ZoneState zoneState = ZoneState.NormalArea;

        private List<GameObject> _activateMonster = new();
        private List<GameObject> _activateContainer = new();

        private ItemContainerSpawner _itemContainerSpawner;
        private MonsterSpawner _monsterSpawner;
        private List<MonsterSpawnPoint> _monsterMarkers = new();
        private List<ItemContainerSpawnPoint> _itemMarkers = new();

        public Region RegionType => _regionType;

        private void Start()
        {
            if (!TryGetComponent<MonsterSpawner>(out _monsterSpawner))
            {
                _monsterSpawner = gameObject.AddComponent<MonsterSpawner>();
            }
            if (!TryGetComponent<ItemContainerSpawner>(out _itemContainerSpawner))
            {
                _itemContainerSpawner = gameObject.AddComponent<ItemContainerSpawner>();
            }

            _monsterMarkers = GetComponentsInChildren<MonsterSpawnPoint>(true).ToList();
            _itemMarkers = GetComponentsInChildren<ItemContainerSpawnPoint>(true).ToList();

            //Debug.Log($"{name} : {_markers.Count}");
            SpawnMonster();
            SpawnItemContainer();
        }

        private void SpawnMonster()
        {
            _activateMonster.Clear();
            foreach (var mark in _monsterMarkers)
            {
                mark.Init(_spawnTable);

                var monsters = _monsterSpawner.Spawn(mark._monsterSpawnPoints, _monsterRoot);

                _activateMonster.AddRange(monsters);

                foreach (var monsterObj in monsters)
                {
                    MonsterController mc = monsterObj.GetComponent<MonsterController>();

                    if (mc != null)
                    {
                        mc.OnDead += HandleMonsterDead;
                    }
                }
            }
        }

        private void SpawnItemContainer()
        {
            _activateContainer.Clear();

            foreach (var mark in _itemMarkers)
            {
                mark.Init();

                GameObject container = _itemContainerSpawner.SpawnOne(
                    mark.SpawnPointData,
                    _itemContainerRoot);
                container.GetComponent<LootContainer>()._containerType = mark.containerType;

                if (container != null)
                    _activateContainer.Add(container);
            }
        }

        public void SetZoneState(ZoneState state)
        {
            zoneState = state;
        }

        public ZoneState GetZoneState()
        {
            return zoneState;
        }

        private void HandleMonsterDead(MonsterController monster)
        {
            monster.OnDead -= HandleMonsterDead;

            _activateMonster.Remove(monster.gameObject);
        }

        private void DebugSpawnMonster()
        {
            int wolf = 0, bear = 0, bat = 0, chicken = 0, dog = 0;

            foreach (var monster in _activateMonster)
            {
                //if(_activateMonster.)
            }
        }
    }
}
