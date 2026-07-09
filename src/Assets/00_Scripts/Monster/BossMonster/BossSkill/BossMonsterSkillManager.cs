using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public enum BossSkillType
    {
        PoisonNiddle,
        Earthquake,
        Tekkai,
        ChaseAttack,
        SuddenStrike,
        SpeedMovement,
        Teleport,
        RushAttack,
        Summon,
        NomalAttack1,
        NomalAttack2,
        Delay
    }

    public class BossMonsterSkillManager : MonoBehaviour
    {
        [SerializeField] private List<BossMonsterSkillSlot> monsterSkills;
        private BossMonsterSkillSlot currentSkillSlot;
        private BossContext context;

        public BossMonsterSkillSlot CurrentSkillSlot { get { return currentSkillSlot; } }

        public void Init(BossContext text)
        {
            context = text;

            foreach (var slot in monsterSkills)
            {
                slot.Init(text);
            }
        }

        private void Update()
        {
            if (context.runtimeMonsterData.currentHp <= 0 && currentSkillSlot != null && currentSkillSlot.instantiatedHitbox != null)
                currentSkillSlot.instantiatedHitbox.SetActive(false);

            Tick();
        }

        private void Tick()
        {
            foreach (var monster in monsterSkills)
            {
                if (monster == null)
                    continue;

                if (monster.skill == null)
                    continue;

                monster.UpdateTime();
            }

            if (currentSkillSlot != null)
            {
                currentSkillSlot.skill?.SkillUpdate(context, currentSkillSlot);
                if (currentSkillSlot.skill == null || currentSkillSlot.skill.IsFinished(currentSkillSlot))
                {
                    currentSkillSlot = null;
                }
            }
        }

        public BossMonsterSkillSlot TryUseSkill(BossSkillType type)
        {
            if (currentSkillSlot != null)
            {
                if (currentSkillSlot.skill == null || !currentSkillSlot.skill.IsFinished(currentSkillSlot))
                    return null;
            }

            foreach (var monsterSkill in monsterSkills)
            {
                if (monsterSkill.skill.skillType != type)
                    continue;

                if (monsterSkill.isRunning == true)
                    continue;

                if (monsterSkill.usedOnce)
                    continue;

                if (!monsterSkill.IsReady())
                    continue;

                if (!monsterSkill.skill.CanUseSkill(context))
                    continue;

                currentSkillSlot = monsterSkill;

                context.movement.MoveStop();

                monsterSkill.skill.UseSkill(context, monsterSkill);

                if (monsterSkill.skill.IsOneShot())
                    monsterSkill.usedOnce = true;

                return monsterSkill;
            }

            return null;
        }

        public bool TryUseSkill()
        {
            foreach (var monsterSkill in monsterSkills)
            {
                if (monsterSkill.usedOnce == true)
                {
                    continue;
                }

                if (monsterSkill.IsReady() == false)
                {
                    continue;
                }
                if (monsterSkill.skill.CanUseSkill(context) == false)
                {
                    continue;
                }

                currentSkillSlot = monsterSkill;

                monsterSkill.skill.UseSkill(context, monsterSkill);

                if (monsterSkill.skill.IsOneShot() == true)
                    monsterSkill.usedOnce = true;

                monsterSkill.Reset();

                return true;
            }

            return false;
        }

        public void OnDamageEvent()
        {
            if (currentSkillSlot == null) return;

            currentSkillSlot.skill.OnDamage(context, currentSkillSlot);

            context.movement.MoveStart();
        }

        public bool IsSkillRunning()
        {
            if (currentSkillSlot == null)
                return false;

            if (currentSkillSlot.skill.IsFinished(currentSkillSlot) == false)
                return true;

            return false;
        }
    }
}