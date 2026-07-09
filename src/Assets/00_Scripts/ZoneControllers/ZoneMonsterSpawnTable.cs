using System.Collections.Generic;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.ZoneSystem;
using UnityEngine;

namespace HBDinosaur_ER_Project.ZoneSystem
{
    [CreateAssetMenu(fileName = "ZoneMonsterSpawnTable", menuName = "Game/Zone/Monster Spawn Table")]
    public class ZoneMonsterSpawnTable : ScriptableObject
    {
        //public Region Region;
        public List<MonsterSpawnWeight> Weights = new();
    }
}