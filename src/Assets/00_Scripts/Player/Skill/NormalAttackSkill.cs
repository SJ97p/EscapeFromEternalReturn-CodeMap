namespace HBDinosaur_ER_Project.Player.Skill
{
    public class NormalAttackSkill : SkillContainer
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AddAction(new MeleeAttackAction());
        }
    }
}