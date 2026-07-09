using UnityEngine;

namespace HBDinosaur_ER_Project.Monster
{
    public class BossBlackBoard
    {
        public Transform target;
        public MonsterData runtimeData;
        public Vector3 patrolPos;
        public int phase;
        public float currentHp;
        public float patrolTime;
        public float battleTime;
        public float teleportTime;
        public bool hasPatrolPos = false;
        public bool isfight = false;
        public bool isAttacking = false;
        public bool isAttackFinished = false;
        public bool isUsingSkill = false;
    }
}