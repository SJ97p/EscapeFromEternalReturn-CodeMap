namespace HBDinosaur_ER_Project.Monster
{
    public class CheckHasTelepoart : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckHasTelepoart(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.runtimeData.isTeleport == false)
                return MonsterNodeState.Failure;

            return MonsterNodeState.Success;
        }
    }
}