using System;
using UnityEngine;
using HBDinosaur_ER_Project.Monster;

namespace HBDinosaur_ER_Project.ZoneSystem
{
    [Serializable]
    public class MonsterSpawnWeight
    {
        public MonsterType MonsterType;
        [Min(0)] public int Weight = 1;
    }
}