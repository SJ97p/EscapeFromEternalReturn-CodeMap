using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class CheckAttackRange : IStrategy
    {
        private BossContext _context;

        public CheckAttackRange(BossContext con)
        {
            _context = con;
        }

        public BossMonsterNode.Status Process()
        {
            if (_context == null || _context.target == null)
                return BossMonsterNode.Status.Failure;

            float dis = Vector3.Distance(_context.owner.transform.position, _context.target.position);

            if (dis <= _context.runtimeMonsterData.attackRange)
                return BossMonsterNode.Status.Success;

            return BossMonsterNode.Status.Failure;
        }

        public void Reset()
        {

        }

        private void OnEnter()
        {

        }
    }
}