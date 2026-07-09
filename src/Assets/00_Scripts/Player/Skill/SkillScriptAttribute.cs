using System;

namespace HBDinosaur_ER_Project.Player.Skill
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkillScriptAttribute : Attribute
    {
        public readonly SkillId skillId;

        public SkillScriptAttribute(SkillId Id)
        {
            skillId = Id;
        }
    }

}