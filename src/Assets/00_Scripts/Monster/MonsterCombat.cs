using HBDinosaur_ER_Project.Player;
using HBDinosaur_ER_Project.Common;
using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster
{
    public class MonsterCombat : MonoBehaviour, IDamageable, IStunnable, IKnockbackable, ISlowable
    {
        private MonsterController controller;

        private float originalMoveSpeed;
        private float slowMultiplier = 1f;
        private bool isStunned = false;
        private bool isKnockedback = false;
        private bool isSlowed = false;

        private void Awake()
        {
            controller = GetComponent<MonsterController>();
        }

        private void Start()
        {
            originalMoveSpeed = controller.runtimeData.moveSpeed;
        }


        public void TakeDamage(DamageContext con)
        {
            int finalDamage = con.Damage - (int)controller.runtimeData.defense;
            controller.blackboard.currentHp -= finalDamage;

            if (controller.blackboard.currentHp <= 0)
            {
                controller.blackboard.currentHp = 0;
                controller.HPCanvas.gameObject.SetActive(false);
                controller.DieAnim();
            }
            if (controller.blackboard.isfight == false || controller.blackboard.target == null)
            {
                controller.blackboard.isfight = true;
                controller.blackboard.target = con.Attacker;
            }

            controller.HitSound();
            controller.MonsterHpUI.UpdateHpBar(controller.blackboard.currentHp);
        }

        public void AttackCalulation()
        {
            DamageContext context = new DamageContext
            {
                Damage = (int)controller.runtimeData.damage,
                Attacker = this.transform
            };

            if (controller.blackboard.target.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(context);
            }

            Debug.Log($"{this.name} Attacking {controller.blackboard.target.name} for {context.Damage} damage.");
        }

        public void ApplyStun(float duration)
        {
            StartCoroutine(StunCoroutine(duration));
        }

        public void ApplyKnockback(Vector3 direction, float force, float duration)
        {
            StartCoroutine(KnockbackCoroutine(direction, force, duration));
        }

        public void ApplySlow(float slowAmount, float duration)
        {
            StartCoroutine(SlowCoroutine(slowAmount, duration));
        }

        private IEnumerator StunCoroutine(float duration)
        {
            if (isStunned == true)
                yield break;

            isStunned = true;

            MonsterStateMachine currentState = new MonsterStateMachine();

            currentState.fsm = controller.stateMachine.fsm;

            controller.abnormalStatus = true;

            controller.stateMachine.ChangeStateFSM(controller.StunFSM);

            Vector3 pos = controller.gameObject.transform.position + new Vector3(0, 2, 0);

            GameObject vfx = VFXManager.Instance.GetVFX(Effects.EffectType.Stun, pos, controller.transform.rotation);

            controller.StopMove();

            yield return new WaitForSeconds(duration);

            VFXManager.Instance.ReleaseVFX(vfx);

            controller.stateMachine.ChangeStateFSM(currentState.fsm);

            controller.abnormalStatus = false;

            isStunned = false;
        }

        private IEnumerator KnockbackCoroutine(Vector3 dir, float power, float duration)
        {
            if (isKnockedback == true)
                yield break;

            isKnockedback = true;

            controller.abnormalStatus = true;

            MonsterStateMachine currentState = new MonsterStateMachine();

            currentState.fsm = controller.stateMachine.fsm;

            controller.StopMove();

            float timer = 0f;

            while (timer < duration)
            {
                transform.position += dir * power * Time.deltaTime;

                timer += Time.deltaTime;

                yield return null;
            } 

            controller.stateMachine.ChangeStateFSM(currentState.fsm);

            isKnockedback = false;

            controller.abnormalStatus = false;
        }

        private IEnumerator SlowCoroutine(float slowAmount, float duration)
        {
            if (isSlowed == true)
                yield break;

            isSlowed = true;

            originalMoveSpeed = controller.runtimeData.moveSpeed;

            slowMultiplier = 1f;

            slowMultiplier *= (1 - slowAmount);

            controller.runtimeData.moveSpeed = originalMoveSpeed * slowMultiplier;

            Debug.Log(controller.runtimeData.moveSpeed);

            controller.Agent.speed = controller.runtimeData.moveSpeed;

            Vector3 pos = controller.gameObject.transform.position + new Vector3(0, -1, 0);

            GameObject vfx = VFXManager.Instance.GetVFX(Effects.EffectType.Slow, pos, controller.transform.rotation);

            yield return new WaitForSeconds(duration);

            VFXManager.Instance.ReleaseVFX(vfx);

            controller.runtimeData.moveSpeed = originalMoveSpeed;

            controller.Agent.speed = originalMoveSpeed;

            isSlowed = false;
        }

        public float GetDefense()
        {
            return controller.runtimeData.defense;
        }
    }
}