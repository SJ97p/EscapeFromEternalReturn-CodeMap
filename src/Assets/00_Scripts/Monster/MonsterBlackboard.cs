using UnityEngine;

namespace HBDinosaur_ER_Project.Monster
{
    public class MonsterBlackboard
    {
        public Transform target;
        public Vector3 originPos;
        public Vector3 patrolPos;
        public float currentHp;
        public float attackRange;
        public float maxChaseRange;
        public float patrolTime;
        public float battleTime;
        public bool isDead = false;
        public bool isReturn = false;
        public bool hasPatrolPos = false;
        public bool isfight = false;
        public bool isAttacking = false;
        public bool isAttackFinished = false;
        public bool isUsingSkill = false;
        public bool startedSkill = false;
    }
}