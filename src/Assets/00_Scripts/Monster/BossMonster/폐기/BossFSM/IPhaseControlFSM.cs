using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster
{
    public class IPhaseControlFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        public IPhaseControlFSM(BossBlackBoard bb)
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

        private MonsterNode BossTree()
        {
            return new Selector(new List<MonsterNode>
            {

            });
        }
    }
}