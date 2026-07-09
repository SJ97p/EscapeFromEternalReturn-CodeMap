namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class UseComboStrategy : IStrategy
    {
        private BossComboManager comboManager;

        private BossComboSlot comboSlot;

        private bool started;

        public UseComboStrategy(BossComboManager manager, BossComboSlot slot)
        {
            comboManager = manager;
            comboSlot = slot;
        }

        public BossMonsterNode.Status Process()
        {
            if (started == false)
            {
                if (!comboManager.TryUseCombo(comboSlot))
                    return BossMonsterNode.Status.Failure;

                started = true;
            }

            if (comboManager.IsComboRunning())
                return BossMonsterNode.Status.Running;

            return BossMonsterNode.Status.Success;
        }

        public void Reset()
        {
            started = false;
        }
    }
}
