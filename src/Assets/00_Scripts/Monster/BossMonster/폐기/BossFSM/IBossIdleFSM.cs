namespace HBDinosaur_ER_Project.Monster
{
    public class IBossIdleFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        public IBossIdleFSM(BossBlackBoard bb) 
        {
            blackBoard = bb;
        }

        public void Enter()
        {

        }

        public void Exit()
        {
            
        }

        public void Update()
        {
            
        }
    }
}