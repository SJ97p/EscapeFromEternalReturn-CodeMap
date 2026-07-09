using System.Collections.Generic;
using System.Linq;
using HBDinosaur_ER_Project.Database;
using UnityEngine;

namespace HBDinosaur_ER_Project.Core
{
    public class SpawnMarkManager : MonoBehaviour
    {
        private List<MonsterSpawnPoint> _markers = new();
        private void Start()
        {
            _markers = FindObjectsByType<MonsterSpawnPoint>(FindObjectsSortMode.None).ToList();
            Debug.Log(_markers.Count);
        }

        public List<MonsterSpawnPoint> GetPoints()
        {
            return _markers;
        }
    }
}

