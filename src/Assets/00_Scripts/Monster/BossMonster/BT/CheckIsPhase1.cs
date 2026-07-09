namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class CheckIsPhase1 : IStrategy
    {
        private BossContext _context;

        public CheckIsPhase1(BossContext con)
        {
            _context = con;
        }

        public BossMonsterNode.Status Process()
        {
            if (_context == null)
                return BossMonsterNode.Status.Failure;

            if (_context.phase == 1)
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