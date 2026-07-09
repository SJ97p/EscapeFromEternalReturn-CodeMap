namespace HBDinosaur_ER_Project.Monster
{
    public class CheckHasTargetBoss : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckHasTargetBoss(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.target == null)
            {
                return MonsterNodeState.Failure;
            }

            return MonsterNodeState.Success;
        }
    }
}