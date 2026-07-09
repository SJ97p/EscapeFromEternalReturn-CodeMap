namespace HBDinosaur_ER_Project.Monster
{
    public class SetPatrolPos : MonsterNode
    {
        BossBlackBoard blackBoard;

        public SetPatrolPos(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            blackBoard.hasPatrolPos = true;
            return MonsterNodeState.Success;
        }
    }
}