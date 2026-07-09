namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class UseSkillStrategy : IStrategy
    {
        private BossMonsterSkillManager skillManager;

        private BossSkillType skillType;

        private bool started;

        public UseSkillStrategy(BossMonsterSkillManager manager, BossSkillType type)
        {
            skillManager = manager;
            skillType = type;
        }

        public BossMonsterNode.Status Process()
        {
            if (!started)
            {
                if (skillManager.TryUseSkill(skillType) == null)
                    return BossMonsterNode.Status.Failure;

                started = true;

                return BossMonsterNode.Status.Running;
            }

            if (skillManager.IsSkillRunning())
                return BossMonsterNode.Status.Running;

            return BossMonsterNode.Status.Success;
        }

        public void Reset()
        {
            started = false;
        }
    }
}
