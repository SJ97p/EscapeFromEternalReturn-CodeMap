namespace HBDinosaur_ER_Project.Monster
{
    public class CheckIsPatrolBoss : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckIsPatrolBoss(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.runtimeData.isPatrol == true)
                return MonsterNodeState.Success;

            return MonsterNodeState.Failure;
        }
    }
}