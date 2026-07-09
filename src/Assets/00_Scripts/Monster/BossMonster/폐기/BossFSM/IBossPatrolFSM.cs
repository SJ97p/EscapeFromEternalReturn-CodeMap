using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster
{
    public class IBossPatrolFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        MonsterNode rootNode;
        public IBossPatrolFSM(BossBlackBoard bb)
        {
            blackBoard = bb;
        }

        public void Enter()
        {
            rootNode = BossPatrolTree();
        }

        public void Exit()
        {
            blackBoard.patrolTime = 0;
        }

        public void Update()
        {
            rootNode.Evaluate();
        }

        private MonsterNode BossPatrolTree()
        {
            return new Sequence(new List<MonsterNode>
            {
                new CheckNoFightingBoss(blackBoard),
                new CheckIsPatrolBoss(blackBoard),
                new Selector(new List<MonsterNode>
                {
                    //new Sequence(new List<MonsterNode>
                    //{
                    //    new CheckNoPatrolPos(blackBoard),
                    //    new SetPatrolPos(blackBoard, context)
                    //}),
                    //new MoveToPos(blackBoard, context)
                })
            });
        }
    }
}