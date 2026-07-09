using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster
{
    public class IBossChaseFSM : IBossMonsterFSM
    {
        BossBlackBoard blackBoard;
        MonsterNode rootNode;
        public IBossChaseFSM(BossBlackBoard bb)
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
            rootNode.Evaluate();
        }

        private MonsterNode BossChaseTree()
        {
            return new Sequence(new List<MonsterNode>
            {
                new CheckFighingBoss(blackBoard),
                new CheckHasTargetBoss(blackBoard),
                new Selector(new List<MonsterNode>
                {
                    new Sequence(new List<MonsterNode>
                    {
                        new CheckHasTelepoart(blackBoard),
                        new CheckTeleportCooldown(blackBoard),
                        // 거리 확인 + 텔레포트 작동
                    }),
                    new Sequence(new List<MonsterNode>
                    {
                        // 추적 거리 안 인지 확인 + 추적
                    })
                })
            });
        }
    }
}