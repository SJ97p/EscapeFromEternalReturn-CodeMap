using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public enum BossProjectileType
    {
        None,
        PosionNiddle,
        FireBall,
        LaserBeam,
    }

    [System.Serializable]
    public class BossProjectile
    {
        public BossProjectileType type;
        public GameObject projectilePrefab;
        public int count;

        [HideInInspector]
        public Queue<GameObject> pool = new Queue<GameObject>();
    }
}