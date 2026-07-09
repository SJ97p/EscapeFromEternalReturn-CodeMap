using HBDinosaur_ER_Project.Player.Skill;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Player
{
    public class ContextTargetQuery : ISkillTargetQuery
    {
        public List<IDamageable> Query(SkillContext context)
        {
            return context.HitTargets;
        }
    }
}
