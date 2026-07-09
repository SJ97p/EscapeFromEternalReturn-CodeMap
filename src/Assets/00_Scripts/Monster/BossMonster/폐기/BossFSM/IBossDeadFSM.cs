using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster
{
    public class IBossDeadFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        public IBossDeadFSM(BossBlackBoard bb)
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

        private MonsterNode BossIdleTree()
        {
            return new Selector(new List<MonsterNode>
            {

            });
        }
    }
}