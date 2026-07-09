using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class ReturnStrategy : IStrategy
    {
        private BossContext _context;
        private BossBlackBoard _blackboard;
        private BossBlackBoardKey _inbattle;
        private BossBlackBoardKey _startBattlePos;
        private BossBlackBoardKey _state;

        private bool _return = false;

        public ReturnStrategy(BossBlackBoard bb, BossContext con,  BossBlackBoardKey battlekey, BossBlackBoardKey poskey, BossBlackBoardKey state)
        {
            _blackboard = bb;
            _context = con;
            _inbattle = battlekey;
            _startBattlePos = poskey;
            _state = state;
        }

        public BossMonsterNode.Status Process()
        {
            if (_blackboard.GetValue<bool>(_inbattle) == false)
                return BossMonsterNode.Status.Failure;

            float dis = Vector3.Distance(_context.owner.transform.position, _blackboard.GetValue<Vector3>(_startBattlePos));

            if (dis > _context.runtimeMonsterData.chaseRange)
            {
                _blackboard.SetValue<string>(_state, "Return");
                _return = true;
            }

            if (_context.movement.IsArrived(_blackboard.GetValue<Vector3>(_startBattlePos)) == true)
            {
                _return = false;
                _blackboard.SetValue<bool>(_inbattle, false);
                _context.animation.EndPatrolAnim();
                return BossMonsterNode.Status.Success;
            }

            if (_return == true)
            {
                _context.movement.Moveto(_blackboard.GetValue<Vector3>(_startBattlePos));
                _context.animation.StartPatrolAnim();
                return BossMonsterNode.Status.Running;
            }
            
            return BossMonsterNode.Status.Failure;
        }
    }
}