using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossContext
    {
        public MonsterData runtimeMonsterData;
        public BossComboManager combo;
        public BossMovementController movement;
        public BossAnimationController animation;
        public BossProjectilePool projectilePool;
        public BossMonsterCombat combat;
        public Transform target;
        public Transform firePosition;
        public GameObject owner;
        public bool dead;
        public int phase;
    }
}