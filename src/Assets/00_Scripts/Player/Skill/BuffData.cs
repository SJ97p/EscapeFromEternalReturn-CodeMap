using HBDinosaur_ER_Project.Effects;

namespace HBDinosaur_ER_Project.Player
{
    public struct BuffData
    {
        public BuffType Type;
        public EffectType EffectType;
        public float Duration;

        public float DefenseAmount;
        public float SlowPercentage;
        public bool IsCCImmune;
    }
}