using HBDinosaur_ER_Project.Common;

namespace HBDinosaur_ER_Project.Player.Skill
{
    [SkillScript(SkillId.HyunwooActive2)]
    public class HyunwooActive2 : SkillContainer
    {
        private ISkillTargetVisualizer visualizer;
        private ISkillTargetQuery query;

        public override bool IsOccupying => false;

        protected override void OnInitialize()
        {
            float defAmount = 20f;
            if (Data != null)
            {
                defAmount = Data.BasicDamage[0];
            }

            BuffData buffData = new BuffData
            {
                Type = BuffType.DefenseUp,
                EffectType = Effects.EffectType.Buff,
                Duration = 2f,
                DefenseAmount = defAmount,
            };
            AddAction(new BuffAction(buffData));
        }
        protected override void OnCast()
        {
            base.OnCast();
            VoiceManager.Instance.PlayVoice(Sound.VoiceSituation.Skill2);
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Skill_02, Context.Caster.position);
        }
    }
}