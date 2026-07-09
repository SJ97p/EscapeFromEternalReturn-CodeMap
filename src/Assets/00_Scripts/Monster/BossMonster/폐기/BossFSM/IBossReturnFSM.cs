using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster
{
    public class IBossReturnFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        public IBossReturnFSM(BossBlackBoard bb)
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

        private MonsterNode BossReturnTree()
        {
            return new Selector(new List<MonsterNode>
            {

            });
        }
    }
}