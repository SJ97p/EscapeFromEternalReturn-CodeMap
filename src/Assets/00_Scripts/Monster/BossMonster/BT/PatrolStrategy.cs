using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class PatrolStrategy : IStrategy
    {
        BossBlackBoard blackBoard;
        BossContext context;
        BossBlackBoardKey actionTimer;
        BossBlackBoardKey monsterState;

        bool isEnter;
        Vector3 patrolPos;

        public PatrolStrategy(BossBlackBoard bb, BossContext con, BossBlackBoardKey timer, BossBlackBoardKey state)
        {
            blackBoard = bb;
            context = con;
            actionTimer = timer;
            monsterState = state;
        }

        public BossMonsterNode.Status Process()
        {
            float timer = blackBoard.GetValue<float>(actionTimer);

            if (context == null) 
                return BossMonsterNode.Status.Failure;

            if (context.runtimeMonsterData.isPatrol == false || context.runtimeMonsterData.nextActionTime > timer)
                return BossMonsterNode.Status.Failure;

            if (isEnter == false)
                OnEnter();

            if (context.movement.IsArrived(patrolPos) == true)
            {
                blackBoard.SetValue<float>(actionTimer, 0);
                return BossMonsterNode.Status.Success;
            }

            context.movement.LookAtTarget();
            return BossMonsterNode.Status.Running;
        }

        public void Reset()
        {
            isEnter = false;
            context.animation.EndPatrolAnim();
        }

        private void OnEnter()
        {
            blackBoard.SetValue<string>(monsterState, "Patrol");
            patrolPos = context.movement.SetRandomPoint();
            context.movement.Moveto(patrolPos);
            context.animation.StartPatrolAnim();
            patrolPos.y = 0;
            isEnter = true;
        }
    }
}