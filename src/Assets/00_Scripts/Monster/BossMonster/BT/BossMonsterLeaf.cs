namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossMonsterLeaf : BossMonsterNode
    {
        readonly IStrategy strategy;

        public BossMonsterLeaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            Preconditions.CheckNotNull(strategy);
            this.strategy = strategy;
        }

        public override Status Process() => strategy.Process();

        public override void Reset() => strategy.Reset();
    }
}