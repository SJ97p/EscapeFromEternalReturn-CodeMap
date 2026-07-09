using HBDinosaur_ER_Project.Player.Skill;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Player
{
    public interface ISkillTargetQuery
    {
        List<IDamageable> Query(SkillContext context);
    }
}