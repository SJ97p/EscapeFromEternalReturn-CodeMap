using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/TeleportSkill")]
    public class Teleport : BossMonsterSkill
    {
        private Vector3 _direction;
        private bool _isTeleporting = false;
        private bool _isBackTeleport = false;
        private bool _isFrontTeleport = false;
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
            if (_isTeleporting == false && _isAttack == false && skillSlot.isRunning == false)
                return true;

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                _isTeleporting = false;
                _isAttack = false;
                skillSlot.isRunning = false;
                skillSlot.instantiatedHitbox.SetActive(false);
            }

            if (con.target != null && _isTeleporting == true)
            {
                con.movement.Moveto(con.target.position);

                if (con.movement.IsArrived(con.target.position) == true && _isAttack == false)
                {
                    _direction = con.target.position - con.owner.transform.position;
                    _direction.y = 0f;
                    if (_isBackTeleport)
                    {
                        con.animation.NomalAttack1Anim();
                        skillSlot.instantiatedHitbox.transform.localScale = new Vector3(2f, 1f, 2f);
                        skillSlot.instantiatedHitbox.transform.rotation = Quaternion.LookRotation(_direction);
                        skillSlot.instantiatedHitbox.transform.position = con.owner.transform.position + con.owner.transform.forward * 2f;
                    }
                    else if (_isFrontTeleport)
                    {
                        con.animation.NomalAttack2Anim();
                        skillSlot.instantiatedHitbox.transform.localScale = new Vector3(1f, 1f, 3f);
                        skillSlot.instantiatedHitbox.transform.rotation = Quaternion.LookRotation(_direction);
                        skillSlot.instantiatedHitbox.transform.position = con.owner.transform.position + con.owner.transform.forward * 2f;
                    }
                    skillSlot.instantiatedHitbox.SetActive(true);
                    _isAttack = true;
                    _isTeleporting = false;
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            if (con.target == null)
                return;

            int randomValue = Random.Range(0, 2);

            switch (randomValue)
            {
                case 0:
                    con.movement.TelePort(con.target, TeleportType.BackOfTarget);
                    _isBackTeleport = true;
                    break;
                case 1:
                    con.movement.TelePort(con.target, TeleportType.FrontOfTarget);
                    _isFrontTeleport = true;
                    break;
            }

            _isTeleporting = true;

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (_isBackTeleport == true)
            {
                if (skillSlot.hitboxComponent.Target != null && skillSlot.hitboxComponent.Target.TryGetComponent(out IDamageable damageable))
                {
                    float damage;
                    damage = (int)con.runtimeMonsterData.damage;

                    if (con.combat.IsBackAttack(con.target.transform, con.owner.transform))
                    {
                        damage *= 1.5f;
                        Debug.Log("Back Attack! Damage increased to " + damage);
                    }

                    DamageContext Context = new DamageContext
                    {
                        Damage = (int)damage,
                        Attacker = con.owner.transform
                    };

                    damageable.TakeDamage(Context);
                }
                
                
                _isBackTeleport = false;
            }
            else if (_isFrontTeleport == true)
            {
                if (skillSlot.hitboxComponent.Target != null && skillSlot.hitboxComponent.Target.TryGetComponent(out IDamageable damageable))
                {
                    float damage;
                    damage = (int)con.runtimeMonsterData.damage;

                    DamageContext Context = new DamageContext
                    {
                        Damage = (int)damage,
                        Attacker = con.owner.transform
                    };

                    damageable.TakeDamage(Context);
                }

                if (con.target.TryGetComponent(out IKnockbackable knockbackable))
                {
                    knockbackable.ApplyKnockback(con.owner.transform.forward, 5f, 0.5f);
                }
                
                _isFrontTeleport = false;
            }

            _isAttack = false;
            skillSlot.isRunning = false;
            skillSlot.instantiatedHitbox.SetActive(false);
            skillSlot.Reset();
        }
    }
}