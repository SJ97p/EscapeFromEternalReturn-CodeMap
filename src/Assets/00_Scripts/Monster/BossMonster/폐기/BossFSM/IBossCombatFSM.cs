using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster
{
    public class IBossCombatFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        public IBossCombatFSM(BossBlackBoard bb)
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

        private MonsterNode BossCombatTree()
        {
            return new Selector(new List<MonsterNode>
            {
                /*
                 * 1 a b c
                 * 2 b c + @
                 * 3
                 * 
                 * 패턴은 매니저를 통해서가 아닌 개발자의 조합으로 사용할 수 있도록 한다. (PatternData 활용해야함)
                 * 등등 여러 패턴이 있고 페이즈에 따른 패턴 사용
                 */
            });
        }
    }
}