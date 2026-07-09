using HBDinosaur_ER_Project.Common;

namespace HBDinosaur_ER_Project.Player.Skill
{
    [SkillScript(SkillId.HyunwooActive4)]
    public class HyunwooActive4 : SkillContainer
    {
        private ISkillTargetVisualizer visualizer;
        private ISkillTargetQuery query;

        // 타겟팅 쿼리
        private float width = 1.5f;
        private float length = 3.5f;

        // 차징
        private float maxChargeTime = 5f;

        // 대미지
        private float _basicDamage;
        private float _additionalDamage;

        protected override void OnInitialize()
        {
            if (Data != null)
            {
                width = Data.Width;
                length = Data.Range;
                _basicDamage = Data.BasicDamage.Count > 0 ? Data.BasicDamage[0] : 0;
                // Note: maxChargeTime is not explicitly in SkillRawData yet.
            }

            executionType = ActionExecutionType.Sequential;
            query = new LineTargetQuery(width, length);
            AddAction(new ChargeAction(query, maxChargeTime));
            AddAction(new DamageAction(query, _basicDamage, _additionalDamage, 1.0f, 2.5f));
        }

        protected override void OnCast()
        {
            base.OnCast();
            VoiceManager.Instance.PlayVoice(Sound.VoiceSituation.Skill4);
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Skill_04_Charging, Context.Caster.position);
            visualizer = new LineVisualizer(width, length);
            visualizer.Draw(Context, 1.2f);
        }
        protected override void OnFinish()
        {
            base.OnFinish();
            SFXManager.Instance.StopSFX(Sound.SFXType.Skill_04_Charging);
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Skill_04_Hit, Context.Caster.position);
        }
    }
}