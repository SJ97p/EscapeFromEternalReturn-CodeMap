namespace HBDinosaur_ER_Project.Monster
{
    public class CheckNoPatrolPos : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckNoPatrolPos(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.hasPatrolPos == false)
                return MonsterNodeState.Success;

            return MonsterNodeState.Failure;
        }
    }
}