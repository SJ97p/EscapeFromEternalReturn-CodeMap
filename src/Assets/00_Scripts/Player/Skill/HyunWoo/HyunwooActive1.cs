using HBDinosaur_ER_Project.Common;

namespace HBDinosaur_ER_Project.Player.Skill
{
    [SkillScript(SkillId.HyunwooActive1)]
    public class HyunwooActive1 : SkillContainer
    {
        private ISkillTargetVisualizer visualizer;
        private ISkillTargetQuery query;

        // 타겟팅 쿼리
        private float angle;
        private float range;

        // 슬로우
        private float _slowPercentage = 40f;
        private float _duration = 2f;

        // 대미지
        private float _basicDamage = 50f;
        private float _additionalDamage;

        protected override void OnInitialize()
        {
            if (Data != null)
            {
                angle = Data.Angle;
                range = Data.Range;
                _basicDamage = Data.BasicDamage.Count > 0 ? Data.BasicDamage[0] : 0;
            }

            executionType = ActionExecutionType.Parallel;
            query = new ConeTargetQuery(angle, range);

            AddAction(new DamageAction(query, _basicDamage, _additionalDamage));
            AddAction(new ApplyStatusAction(query, AbnormalStatus.Slow, _slowPercentage, _duration));
        }

        protected override void OnCast()
        {
            base.OnCast();
            VoiceManager.Instance.PlayVoice(Sound.VoiceSituation.Skill1);
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Skill_01, Context.Caster.position);
            visualizer = new ConeVisualizer(angle, range);
            visualizer.Draw(Context, 1f);
        }
    }
}