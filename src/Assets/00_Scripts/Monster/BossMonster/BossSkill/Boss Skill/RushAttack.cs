using HBDinosaur_ER_Project.Player;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/RushAttackSkill")]
    public class RushAttack : BossMonsterSkill
    {
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private float _maxRushDistance = 10f;
        private HashSet<Collider> _hitTargets = new();
        private bool _isReady = false;
        private bool _isRushing = false;
        private float _readyTime = 3f;
        private float _rushTimer = 0f;
        private float _currentRushDistance = 0f;
        private Vector3 _rushDirection;

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
            if (skillSlot != null)
            {
                if (_isRushing == false && _isReady == false && skillSlot.isRunning == false)
                    return true;
            }

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                _isReady = false;
                _isRushing = false;
                _rushTimer = 0f;
                skillSlot.isRunning = false;
                skillSlot.instantiatedHitbox.SetActive(false);
            }

            if (_isReady == true)
            {
                _rushTimer += Time.deltaTime;

                if (con.phase == 3)
                {
                    SetRushDirection(con);
                    UpdateIndicator(con, skillSlot);
                }

                if (_rushTimer > _readyTime)
                {
                    _isReady = false;
                    _isRushing = true;
                    _rushTimer = 0f;
                }
            }

            if (_isRushing == true)
            {
                skillSlot.instantiatedHitbox.SetActive(false);

                if (CheckWall(con))
                {
                    EndRush(con, skillSlot);
                    return;
                }

                float moveSpeed = con.runtimeMonsterData.moveSpeed * 2f * Time.deltaTime;

                con.owner.transform.position += _rushDirection * moveSpeed;

                _currentRushDistance += moveSpeed;

                CheckRushHit(con);

                if (_currentRushDistance >= _maxRushDistance)
                {
                    EndRush(con, skillSlot);
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

            con.movement.Agent.enabled = false;

            con.animation.StartMoveSkillAnim();

            SetReadyTime(con);

            SetRushDirection(con);

            UpdateIndicator(con, skillSlot);

            _hitTargets.Clear();

            _isReady = true;

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {

        }

        private void UpdateIndicator(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            skillSlot.instantiatedHitbox.SetActive(true);

            skillSlot.instantiatedHitbox.transform.position = con.owner.transform.position + _rushDirection * (_maxRushDistance * 0.5f);

            skillSlot.instantiatedHitbox.transform.rotation = Quaternion.LookRotation(_rushDirection);

            skillSlot.instantiatedHitbox.transform.localScale = new Vector3(3f, 1f, _maxRushDistance);
        }

        private void SetReadyTime(BossContext con)
        {
            switch(con.phase)
            {
                case 1:
                    _readyTime = 5f;
                    break;
                case 2:
                    _readyTime = 3f;
                    break;
                case 3:
                    _readyTime = 1.5f;
                    break;
            }
        }

        private void SetRushDirection(BossContext con)
        {
            if (con.target == null)
                return;

            _rushDirection = con.target.position - con.owner.transform.position;

            _rushDirection.y = 0f;

            _rushDirection.Normalize();

            con.owner.transform.rotation = Quaternion.LookRotation(_rushDirection);
        }

        private void CheckRushHit(BossContext con)
        {
            Vector3 point1 = con.owner.transform.position + Vector3.up * 0.5f;

            Vector3 point2 = con.owner.transform.position + Vector3.up * 2f;

            Collider[] hits = Physics.OverlapCapsule(point1, point2, 1.5f, _targetLayer);

            foreach (Collider hit in hits)
            {
                if (_hitTargets.Contains(hit))
                    continue;

                _hitTargets.Add(hit);

                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    DamageContext context = new DamageContext
                    {
                        Damage = (int)con.runtimeMonsterData.damage,
                        Attacker = con.owner.transform
                    };

                    damageable.TakeDamage(context);
                }

                if (hit.TryGetComponent(out IKnockbackable knockbackable))
                {
                    knockbackable.ApplyKnockback(con.owner.transform.forward, 5f, 0.5f);
                }
            }
        }

        private bool CheckWall(BossContext con)
        {
            Vector3 origin = con.owner.transform.position + Vector3.up;

            float checkDistance = 1.5f;

            return Physics.Raycast(origin, _rushDirection,  checkDistance, _wallLayer);
        }

        private void EndRush(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            _currentRushDistance = 0f;

            con.movement.Agent.enabled = true;

            con.movement.Agent.Warp(con.owner.transform.position);

            con.animation.EndMoveSkillAnim();

            _isRushing = false;

            skillSlot.isRunning = false;

            skillSlot.Reset();
        }
    }
}