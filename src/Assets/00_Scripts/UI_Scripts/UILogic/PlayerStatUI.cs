using HBDinosaur_ER_Project.Player;
using SingletonPattern_Scripts;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HBDinosaur_ER_Project.UI
{
    public enum DisplayType
    {
        Integer,
        Decimal
    }
    public enum StatType
    {
        AttackDamage,
        AttackSpeed,
        Defense,
        MoveSpeed,
    }
    [System.Serializable]
    public class StatUIList
    {
        public TMP_Text Text;
        public DisplayType DisplayType;
        public StatType StatType;
    }
    public class PlayerStatUI : Singleton<PlayerStatUI>
    {
        [Header("스탯 텍스트 (TMP)")]
        [SerializeField] private List<StatUIList> data;

        // ── 라이프사이클 ──────────────────────────────────────────

        private void OnEnable()
        {
            StateEventBus.OnAttackDamageChanged += UpdateAttackDamage;
            StateEventBus.OnAttackSpeedChanged += UpdateAttackSpeed;
            StateEventBus.OnMoveSpeedChanged += UpdateMoveSpeed;
            StateEventBus.OnDefenseChanged += UpdateDefense;
        }

        private void OnDisable()
        {
            StateEventBus.OnAttackDamageChanged -= UpdateAttackDamage;
            StateEventBus.OnAttackSpeedChanged -= UpdateAttackSpeed;
            StateEventBus.OnMoveSpeedChanged -= UpdateMoveSpeed;
            StateEventBus.OnDefenseChanged -= UpdateDefense;
        }

        // ── 콜백 ─────────────────────────────────────────────────

        private void UpdateAttackDamage(float value)
        {
            foreach (var item in data)
            {
                if (item.StatType == StatType.AttackDamage)
                {
                    item.Text.text = Format(value, item.DisplayType);
                }
            }
            //attackDamageText.text = Format(value);
        }

        private void UpdateAttackSpeed(float value)
        {
            foreach (var item in data)
            {
                if (item.StatType == StatType.AttackSpeed)
                {
                    item.Text.text = Format(value, item.DisplayType);
                }
            }
        }

        private void UpdateMoveSpeed(float value)
        {
            foreach (var item in data)
            {
                if (item.StatType == StatType.MoveSpeed)
                {
                    item.Text.text = Format(value, item.DisplayType);
                }
            }
        }

        private void UpdateDefense(float value)
        {
            foreach (var item in data)
            {
                if (item.StatType == StatType.Defense)
                {
                    item.Text.text = Format(value, item.DisplayType);
                }
            }
        }

        // ── 유틸 ─────────────────────────────────────────────────

        private string Format(float value, DisplayType type)
        {
            int val = type == DisplayType.Integer ? 0 : 1;
            return val <= 0
                ? Mathf.RoundToInt(value).ToString()
                : value.ToString($"F{val}");
        }
    }
}
