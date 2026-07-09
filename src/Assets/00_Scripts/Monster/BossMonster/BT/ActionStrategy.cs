using System;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class ActionStrategy : IStrategy
    {
        readonly Action doSomthing;

        public ActionStrategy(Action doaction)
        {
            doSomthing = doaction;
        }

        public BossMonsterNode.Status Process()
        {
            doSomthing();
            return BossMonsterNode.Status.Success;
        }
    }
}