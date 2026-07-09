using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Effects;
using HBDinosaur_ER_Project.Player;
using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossMonsterCombat : MonoBehaviour, IDamageable, IStunnable, IKnockbackable, ISlowable
    {
        private BossMonsterController controller;
        private BossContext context;

        private int _cumulativeDamage;

        private float _slowMultiplier = 1f;
        private float _originalMoveSpeed;

        private bool _reflection;
        private bool _summon;
        private bool _isStunned;
        private bool _isKnockedback;

        private Coroutine _tekkaiCoroutine;
        private Coroutine _summonCoroutine;
        private Coroutine _slowCoroutine;

        private void Awake()
        {
            controller = GetComponent<BossMonsterController>();
        }

        private void Start()
        {
            context = controller.Context;
            context.combat = this;
            _originalMoveSpeed = context.runtimeMonsterData.moveSpeed;
        }

        private void ReflectionOn()
        {
            _reflection = true;
        }

        private void ReflectionOff()
        {
            _reflection = false;
            context.animation.EndSkillAttack3Anim();
        }

        private void SummonOn()
        {
            _summon = true;
        }

        private void SummonOff()
        {
            _summon = false;
        }

        public int GetCumulativeDamage()
        {
            return _cumulativeDamage;
        }

        public void TakeDamage(DamageContext con)
        {
            int finalDamage = con.Damage - (int)context.runtimeMonsterData.defense;

            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Hit, gameObject.transform.position);

            if (_reflection)
            {
                finalDamage /= 2;

                if (con.Attacker.TryGetComponent(out IDamageable damageable))
                {
                    DamageContext reflectContext = new DamageContext
                    {
                        Damage = finalDamage,
                        Attacker = transform
                    };

                    damageable.TakeDamage(reflectContext);

                    Debug.Log($"{name} reflected " + $"{reflectContext.Damage} damage " + $"back to {con.Attacker.name}.");
                }
            }

            context.runtimeMonsterData.currentHp -= finalDamage;

            if (_summon)
            {
                _cumulativeDamage += finalDamage;
            }

            if (context.runtimeMonsterData.currentHp <= 0)
            {
                context.runtimeMonsterData.currentHp = 0;

                controller.Hpcanavas.gameObject.SetActive(false);

                context.animation.StartDeadAnim();

                context.dead = true;

                return;
            }

            if (context.runtimeMonsterData.currentHp <= (context.runtimeMonsterData.maxHp / 100) * 30)
                context.phase = 3;
            else if (context.runtimeMonsterData.currentHp <= (context.runtimeMonsterData.maxHp / 100) * 70)
                context.phase = 2;


            controller.MonsterHpUI.UpdateHpBar(context.runtimeMonsterData.currentHp);
        }

        public void Tekkai(float duration)
        {
            if (_tekkaiCoroutine == null)
            {
                _tekkaiCoroutine = StartCoroutine(TekkaiSkillRoutine(duration));
            }
        }

        public void Summon(float duration)
        {
            if (_summonCoroutine == null)
            {
                _summonCoroutine = StartCoroutine(SummonSkillRoutine(duration));
            }
        }

        public void CancelSummon()
        {
            StopCoroutine(SummonSkillRoutine(0));
            SummonOff();
        }

        public void AttackCalculation()
        {
            if (context.target == null)
                return;

            DamageContext damageContext = new DamageContext
            {
                Damage = (int)context.runtimeMonsterData.damage,
                Attacker = transform
            };

            if (context.target.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageContext);
            }

            Debug.Log($"{name} Attacking {context.target.name} " + $"for {damageContext.Damage} damage.");
        }

        public bool IsBackAttack(Transform target, Transform attacker)
        {
            Vector3 toAttacker = (attacker.position - target.position).normalized;
            float dotProduct = Vector3.Dot(target.forward, toAttacker);
            return dotProduct < -0.5f;
        }

        public void ApplyStun(float duration)
        {
            if (_isStunned)
                return;

            StartCoroutine(StunCoroutine(duration));
        }

        public void ApplySlow(float slowAmount, float duration)
        {
            if (_slowCoroutine != null)
            {
                StopCoroutine(_slowCoroutine);
            }

            _slowCoroutine = StartCoroutine(SlowCoroutine(slowAmount, duration));
        }

        public void ApplyKnockback(Vector3 direction, float force, float duration)
        {
            if (_isKnockedback)
                return;

            StartCoroutine(KnockbackCoroutine(direction, force, duration));
        }

        private IEnumerator TekkaiSkillRoutine(float duration)
        {
            ReflectionOn();

            yield return new WaitForSeconds(duration);

            ReflectionOff();

            _tekkaiCoroutine = null;
        }

        private IEnumerator SummonSkillRoutine(float duration)
        {
            _cumulativeDamage = 0;

            SummonOn();

            yield return new WaitForSeconds(duration);

            SummonOff();

            _summonCoroutine = null;
        }

        private IEnumerator StunCoroutine(float duration)
        {
            _isStunned = true;

            var vfx = VFXManager.Instance.GetVFX(EffectType.Stun, gameObject.transform.position, gameObject.transform.rotation);

            context.movement.MoveStop();

            yield return new WaitForSeconds(duration);

            _isStunned = false;
        }

        private IEnumerator KnockbackCoroutine(Vector3 direction, float force, float duration)
        {
            _isKnockedback = true;

            context.movement.MoveStop();

            float timer = 0f;

            while (timer < duration)
            {
                transform.position += direction * force * Time.deltaTime;

                timer += Time.deltaTime;

                yield return null;
            }

            _isKnockedback = false;
        }

        private IEnumerator SlowCoroutine(float slowAmount, float duration)
        {
            GameObject slowVFX = VFXManager.Instance.GetVFX(EffectType.Slow, transform.position, transform.rotation, transform);

            float multiplier = 1f - slowAmount;

            context.runtimeMonsterData.moveSpeed = _originalMoveSpeed * multiplier;

            context.movement.SetMoveSpeed(context.runtimeMonsterData.moveSpeed);

            yield return new WaitForSeconds(duration);

            context.runtimeMonsterData.moveSpeed = _originalMoveSpeed;

            context.movement.SetMoveSpeed(_originalMoveSpeed);

            VFXManager.Instance.ReleaseVFX(slowVFX);
        }
        public float GetDefense()
        {
            return 1f;
        }
    }
}