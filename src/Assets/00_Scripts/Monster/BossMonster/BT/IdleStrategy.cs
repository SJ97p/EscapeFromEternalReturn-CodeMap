using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class IdleStrategy : IStrategy
    {
        private BossBlackBoard _blackBoard;
        private BossContext _context;
        private BossBlackBoardKey _inbattle;
        private BossBlackBoardKey _patrolTimer;
        private BossBlackBoardKey _monsterState;

        bool isEnter;
        float timer;
        public IdleStrategy(BossBlackBoard bb, BossContext con, BossBlackBoardKey battle, BossBlackBoardKey timer, BossBlackBoardKey state)
        {
            _blackBoard = bb;
            _context = con;
            _inbattle = battle;
            _patrolTimer = timer;
            _monsterState = state;
        }

        public BossMonsterNode.Status Process()
        {
            if (isEnter == false)
                OnEnter();

            if (_context.target != null)
            {
                float dis = Vector3.Distance(_context.target.position, _context.owner.transform.position);

                if (dis <= _context.runtimeMonsterData.chaseRange)
                    return BossMonsterNode.Status.Failure;
            }

            if (_blackBoard.GetValue<float>(_patrolTimer) >= _context.runtimeMonsterData.nextActionTime)
                return BossMonsterNode.Status.Success;

            return BossMonsterNode.Status.Running;
        }

        public void Reset()
        {
            isEnter = false;
        }

        private void OnEnter()
        {
            _blackBoard.SetValue<string>(_monsterState, "Idle");
            _blackBoard.SetValue<bool>(_inbattle, false);
            _context.movement.MoveStop();
            isEnter = true;
        }
    }
}