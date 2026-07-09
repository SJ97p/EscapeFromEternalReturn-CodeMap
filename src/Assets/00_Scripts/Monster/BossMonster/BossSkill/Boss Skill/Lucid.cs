using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/LucidSkill")]
    public class Lucid : BossMonsterSkill
    {
        private int _lucidCount = 0;
        private int _randomIndex = 0;
        private float _nextAttackTime = 0f;
        private bool _isLucid = false;
        public override bool CanUseSkill(BossContext con)
        {
            if (con == null)
                return false;

            return true;
        }

        public override bool IsOneShot()
        {
            return false;
        }

        public override bool IsFinished(BossMonsterSkillSlot skillSlot)
        {
            if (_isLucid == false && _lucidCount <= 0 && skillSlot.isRunning == false)
                return true;

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                _isLucid = false;
                _lucidCount = 0;
                skillSlot.isRunning = false;
                skillSlot.instantiatedHitbox.SetActive(false);
            }

            if (_lucidCount > 0 && _isLucid == false && con.target != null)
            {
                if (Time.time >= _nextAttackTime)
                { 
                    switch (_randomIndex)
                    {
                        case 0:
                            con.animation.SetAttackAnimSpeed(1.5f);
                            con.animation.NomalAttack1Anim();
                            _isLucid = true;
                            skillSlot.instantiatedHitbox.transform.position = con.target.position;
                            skillSlot.instantiatedHitbox.transform.rotation = con.target.rotation;
                            skillSlot.instantiatedHitbox.SetActive(true);
                            break;
                        case 1:
                            con.animation.SetAttackAnimSpeed(1.5f);
                            con.animation.NomalAttack2Anim();
                            _isLucid = true;
                            skillSlot.instantiatedHitbox.transform.position = con.target.position;
                            skillSlot.instantiatedHitbox.transform.rotation = con.target.rotation * Quaternion.Euler(0, 90, 0);
                            skillSlot.instantiatedHitbox.SetActive(true);
                            break;
                    }
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            _isLucid = false;

            _nextAttackTime = 0f;

            _randomIndex = Random.Range(0, 2);

            _lucidCount = LucidCountSet(con.phase);

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.hitboxComponent.Target != null)
            {
                if (skillSlot.hitboxComponent.Target.TryGetComponent(out IDamageable damageable))
                {
                    float damage;
                    damage = (int)con.runtimeMonsterData.damage * 2f;

                    DamageContext Context = new DamageContext
                    {
                        Damage = (int)damage,
                        Attacker = con.owner.transform
                    };

                    damageable.TakeDamage(Context);
                }
            }
            skillSlot.instantiatedHitbox.SetActive(false);
            _randomIndex = Random.Range(0, 2);
            _nextAttackTime = Time.time + 0.5f;
            _isLucid = false;
            _lucidCount--;
            if (_lucidCount > 0)
            {
                con.animation.SetAttackAnimSpeed(1.5f);
            }
            else
            {
                con.animation.SetAttackAnimSpeed(0.4f);
                skillSlot.isRunning = false;
                skillSlot.Reset();
            }
        }

        private int LucidCountSet(int phase)
        {
            switch (phase)
            {
                case 1:
                    return 5;

                case 2:
                    return 7;

                case 3:
                    return 10;
            }

            return 5;
        }
    }
}