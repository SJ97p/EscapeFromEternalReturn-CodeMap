namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class CheckIsPhase3 : IStrategy
    {
        private BossContext _context;

        public CheckIsPhase3(BossContext con)
        {
            _context = con;
        }

        public BossMonsterNode.Status Process()
        {
            if (_context == null)
                return BossMonsterNode.Status.Failure;

            if (_context.phase == 3)
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