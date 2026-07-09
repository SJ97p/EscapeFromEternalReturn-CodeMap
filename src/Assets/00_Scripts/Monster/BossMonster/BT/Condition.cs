using System;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class Condition : IStrategy
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public BossMonsterNode.Status Process() => predicate() ? BossMonsterNode.Status.Success : BossMonsterNode.Status.Failure;
    }
}