using System;

namespace HBDinosaur_ER_Project.Player
{
    public static class StateEventBus
    {
        public static Action<float> OnMaxHpChanged;
        public static void RaiseMaxHpChanged(float maxHp)
        {
            OnMaxHpChanged?.Invoke(maxHp);
        }

        public static Action<float> OnHpChanged;
        public static void RaiseHpChanged(float hp)
        {
            OnHpChanged?.Invoke(hp);
        }

        public static Action OnZoneTimerZero;
        public static void RaiseZoneTimerZero()
        {
            OnZoneTimerZero?.Invoke();
        }

        // ── 스탯 UI 연동 이벤트 ──────────────────────────────────
        public static Action<float> OnAttackDamageChanged;
        public static void RaiseAttackDamageChanged(float value)
        {
            OnAttackDamageChanged?.Invoke(value);
        }

        public static Action<float> OnAttackSpeedChanged;
        public static void RaiseAttackSpeedChanged(float value)
        {
            OnAttackSpeedChanged?.Invoke(value);
        }

        public static Action<float> OnMoveSpeedChanged;
        public static void RaiseMoveSpeedChanged(float value)
        {
            OnMoveSpeedChanged?.Invoke(value);
        }

        public static Action<float> OnDefenseChanged;
        public static void RaiseDefenseChanged(float value)
        {
            OnDefenseChanged?.Invoke(value);
        }
    }
}

