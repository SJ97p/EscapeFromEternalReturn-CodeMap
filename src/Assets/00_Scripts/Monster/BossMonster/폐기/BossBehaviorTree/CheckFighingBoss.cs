namespace HBDinosaur_ER_Project.Monster
{
    public class CheckFighingBoss : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckFighingBoss(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.isfight == true)
                return MonsterNodeState.Success;

            return MonsterNodeState.Failure;
        }
    }
}