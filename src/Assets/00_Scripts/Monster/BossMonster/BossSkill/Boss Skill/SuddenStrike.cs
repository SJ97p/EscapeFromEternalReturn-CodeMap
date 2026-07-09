using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/SuddenStrikeSkill")]
    public class SuddenStrike : BossMonsterSkill
    {
        private bool _isMove = false;
        private bool _isAttack = false;
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
            if (_isMove == false && _isAttack == false && skillSlot.isRunning == false)
                return true;

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                _isMove = false;
                _isAttack = false;
                skillSlot.isRunning = false;
                skillSlot.instantiatedHitbox.SetActive(false);
            }

            if (_isMove == true)
            {
                con.movement.Moveto(con.target.transform.position);
                
                con.movement.LookAtTarget();

                if (Vector3.Distance(con.owner.transform.position, con.target.transform.position) < 2f)
                {
                    con.animation.EndMoveSkillAnim();
                    con.movement.MoveStop();
                    con.animation.NomalAttack2Anim();
                    skillSlot.instantiatedHitbox.transform.position = con.target.position;
                    skillSlot.instantiatedHitbox.SetActive(true);

                    _isMove = false;
                    _isAttack = true;
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            con.animation.StartMoveSkillAnim();

            _isMove = true;

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            skillSlot.instantiatedHitbox.transform.position = con.owner.transform.position + Vector3.forward * 2f;

            skillSlot.instantiatedHitbox.SetActive(true);

            if (skillSlot.hitboxComponent.Target != null)
            {
                if (skillSlot.hitboxComponent.Target.TryGetComponent(out IStunnable stunnable))
                {
                    stunnable.ApplyStun(3f);
                }
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

            _isAttack = false;
            skillSlot.isRunning = false;
            skillSlot.instantiatedHitbox.SetActive(false);
            skillSlot.Reset();
        }
    }
}