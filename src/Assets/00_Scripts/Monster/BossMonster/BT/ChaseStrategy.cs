using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class ChaseStrategy : IStrategy
    {
        BossBlackBoard blackBoard;
        BossContext context;
        bool isEnter;

        BossBlackBoardKey startBattlePos;
        BossBlackBoardKey inBattle;
        BossBlackBoardKey monsterState;

        public ChaseStrategy(BossBlackBoard bb, BossContext con, BossBlackBoardKey Pos, BossBlackBoardKey battlebool, BossBlackBoardKey state)
        {
            blackBoard = bb;
            context = con;
            startBattlePos = Pos;
            inBattle = battlebool;
            monsterState = state;
        }

        public BossMonsterNode.Status Process()
        {
            if (context == null || context.movement == null)
            {
                Reset();
                return BossMonsterNode.Status.Failure;
            }

            if (context.runtimeMonsterData.firstStrike == false || context.target == null)
            {
                Reset();
                return BossMonsterNode.Status.Failure;
            }

            float dis = Vector3.Distance(context.target.position, context.owner.transform.position);

            if (dis > context.runtimeMonsterData.chaseRange)
            {
                Reset();
                return BossMonsterNode.Status.Failure;
            }

            if (blackBoard.GetValue<bool>(inBattle) == false)
                blackBoard.SetValue<Vector3>(startBattlePos, context.owner.transform.position);

            if (isEnter == false)
                OnEnter();

            if (context.runtimeMonsterData.attackRange >= dis)
            {
                Reset();
                return BossMonsterNode.Status.Success;
            }

            context.movement.Moveto(context.target.position);
            context.movement.LookAtTarget();
            return BossMonsterNode.Status.Running;
        }

        public void Reset()
        {
            context.animation.SetWalkAnim(0);
            isEnter = false;
        }

        private void OnEnter()
        {
            blackBoard.SetValue<string>(monsterState, "Chase");
            blackBoard.SetValue<bool>(inBattle, true);
            context.animation.SetWalkAnim(1);
            isEnter = true;
        }
    }
}