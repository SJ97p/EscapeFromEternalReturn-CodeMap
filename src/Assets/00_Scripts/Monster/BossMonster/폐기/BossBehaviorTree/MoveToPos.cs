namespace HBDinosaur_ER_Project.Monster
{
    public class MoveToPos : MonsterNode
    {
        BossBlackBoard blackBoard;

        bool isMove = false;

        public MoveToPos(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            //if (blackBoard.hasPatrolPos == false)
            //    return MonsterNodeState.Failure;

            //if (isMove == false)
            //{
            //    context.movement.Movto(blackBoard.patrolPos);
            //    context.animation.StartPatrolAnim();
            //    isMove = true;
            //}


            //if (context.movement.IsArrived(blackBoard.patrolPos))
            //{
            //    blackBoard.hasPatrolPos = false;
            //    isMove = false;
            //    context.animation.EndPatrolAnim();
            //    context.movement.MoveStop();
            //    return MonsterNodeState.Success;
            //}
            //else
                return MonsterNodeState.Running;
        }
    }
}