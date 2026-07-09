using HBDinosaur_ER_Project.Common;

namespace HBDinosaur_ER_Project.Player.Skill
{

    [SkillScript(SkillId.HyunwooActive3)]
    public class HyunwooActive3 : SkillContainer
    {
        private ISkillTargetVisualizer visualizer;

        private float _radius = 0.5f;
        private float width = 1f;
        private float speed = 7f;
        private float distance = 3.5f;

        private float _basicDamge;
        private float _additionalDamage;

        protected override void OnInitialize()
        {
            if (Data != null)
            {
                _radius = Data.Radius;
                width = Data.Width;
                distance = Data.Range;
                _basicDamge = Data.BasicDamage.Count > 0 ? Data.BasicDamage[0] : 0;
                // TODO : AdditionalDamage Ãß°¡
            }

            executionType = ActionExecutionType.Parallel;
            AddAction(new RushDamageAction(distance, speed, _radius, _basicDamge, _additionalDamage));
        }

        protected override void OnCast()
        {
            base.OnCast();
            VoiceManager.Instance.PlayVoice(Sound.VoiceSituation.Skill3);
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Skill_03_Slide, Context.Caster.position);
            visualizer = new LineVisualizer(width, distance);
            visualizer.Draw(Context, 1f);
        }
    }
}