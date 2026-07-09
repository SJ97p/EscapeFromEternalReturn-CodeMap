using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public abstract class BossMonsterSkill : BossCombatAction
    {
        public GameObject hitboxPrefab;

        public BossSkillType skillType;
        public abstract bool CanUseSkill(BossContext context);
        public abstract bool IsOneShot();
        public abstract bool IsFinished(BossMonsterSkillSlot skillSlot);
        public abstract void SkillUpdate(BossContext context, BossMonsterSkillSlot skillSlot);
        public abstract void UseSkill(BossContext context, BossMonsterSkillSlot skillSlot);
        public abstract void OnDamage(BossContext context, BossMonsterSkillSlot skillSlot);

    }
}