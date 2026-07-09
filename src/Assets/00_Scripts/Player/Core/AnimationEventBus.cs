using System;

namespace HBDinosaur_ER_Project.Player
{
    public static class AnimationEventBus
    {
        public static Action OnTeleportStart;
        public static void RaiseTelportStart()
        {
            OnTeleportStart?.Invoke();
        }

        public static Action OnTeleportEnd;
        public static void RaiseTelportEnd()
        {
            OnTeleportEnd?.Invoke();
        }

        public static Action<float> OnMove;
        public static void RaiseMove(float speed)
        {
            OnMove?.Invoke(speed);
        }
        public static Action<float> OnAttack;
        public static void RaiseAttack(float attackSpeed)
        {
            OnAttack?.Invoke(attackSpeed);
        }

        public static Action OnAttackHit;
        public static void RaiseAttackHit()
        {
            OnAttackHit?.Invoke();
        }
        public static Action<int> OnSkill;
        public static void RaiseSkill(int skill)
        {
            OnSkill?.Invoke(skill);
        }
        public static Action<int> OnSkillFinish;
        public static void RaiseSkillFinish(int skill)
        {
            OnSkillFinish?.Invoke(skill);
        }
        public static Action OnDead;
        public static void RaiseDead()
        {
            OnDead?.Invoke();
        }
    }
}