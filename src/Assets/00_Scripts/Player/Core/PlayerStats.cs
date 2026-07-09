using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("캐릭터 이름")]
        public string Name;

        [Header("체력")]
        public float HP;
        public float MaxHP;
        public float GrowthHP;

        [Header("체력 재생")]
        public float RegenerationHP;
        public float GrowthRegenerationHP;

        [Header("공격력")]
        public float AttackDamage;
        public float GrowthAttackDamage;

        [Header("기본 공격 증폭")]
        public float AttackMultiplier;

        [Header("스킬증폭")]
        public float SkillDamage;
        public float GrodwthSkillDamage;

        [Header("방어력")]
        public float Defense;
        public float GrodwthDefense;

        [Header("레벨")]
        public int Level;
        public int MaxLevel;

        [Header("이동속도")]
        public float MoveSpeed;

        [Header("공격속도")]
        public float AttackSpeed;

        [Header("공격범위")]
        public float AttackRange;

        [Header("치명타")]
        public float CriticalProbability;
        public float CriticalDamage;

        [Header("흡혈")]
        public float LifeSteel;
    }
}