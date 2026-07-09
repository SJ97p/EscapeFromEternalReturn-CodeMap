namespace HBDinosaur_ER_Project.Monster
{
    public class CheckTeleportCooldown : MonsterNode
    {
        BossBlackBoard blackBoard;

        public CheckTeleportCooldown(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public override MonsterNodeState Evaluate()
        {
            if (blackBoard.teleportTime > blackBoard.runtimeData.teleportTime)
                return MonsterNodeState.Success;

            return MonsterNodeState.Failure;
        }
    }
}