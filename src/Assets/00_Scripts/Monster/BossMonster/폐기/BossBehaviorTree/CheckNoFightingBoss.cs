namespace HBDinosaur_ER_Project.Monster
{
    public class CheckNoFightingBoss : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckNoFightingBoss(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.isfight == true)
                return MonsterNodeState.Failure;

            return MonsterNodeState.Success;
        }
    }
}