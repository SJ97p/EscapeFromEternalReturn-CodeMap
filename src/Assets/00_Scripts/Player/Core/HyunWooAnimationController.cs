using HBDinosaur_ER_Project.Common;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class HyunWooAnimationController : MonoBehaviour
    {
        private Animator animator;
        private HashSet<string> parameters = new();
        private bool isFullCharged;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            foreach (var parameter in animator.parameters)
            {
                parameters.Add(parameter.name);
            }
            animator.SetFloat("moveSpeed", 1);
        }

        private void OnEnable()
        {
            AnimationEventBus.OnMove += HandleMove;
            AnimationEventBus.OnSkill += HandleSkill;
            AnimationEventBus.OnSkillFinish += HandleSkillFinish;
            AnimationEventBus.OnAttack += HandleAttack;
            AnimationEventBus.OnDead += HandleDead;
            AnimationEventBus.OnTeleportStart += TeleportStart;
            AnimationEventBus.OnTeleportEnd += TeleportEnd;
        }
        private void OnDisable()
        {
            AnimationEventBus.OnMove -= HandleMove;
            AnimationEventBus.OnSkill -= HandleSkill;
            AnimationEventBus.OnSkillFinish -= HandleSkillFinish;
            AnimationEventBus.OnAttack -= HandleAttack;
            AnimationEventBus.OnDead -= HandleDead;
            AnimationEventBus.OnTeleportStart -= TeleportStart;
            AnimationEventBus.OnTeleportEnd -= TeleportEnd;
        }

        private void TeleportStart()
        {
            animator.SetTrigger("tTeleportAction");
        }

        private void TeleportEnd()
        {
            animator.SetTrigger("tTeleportArrive");
        }

        private void HandleDead()
        {
            animator.SetBool("dead", true);
        }

        private void HandleAttack(float attackSpeed)
        {
            Debug.Log(attackSpeed);
            animator.SetFloat("attackSpeed", attackSpeed);
            animator.SetTrigger("tAttack01");
        }

        private void HandleMove(float speed)
        {
            SetFloat("moveVelocity", speed);
        }

        private void HandleSkill(int skillId)
        {
            //SetTrigger($tSkill0{skillId + 1});

            // 임시용
            switch (skillId)
            {
                case 0: PlayQAnimation(); break;
                case 1: PlayWAnimation(); break;
                case 2: PlayEAnimation(); break;
                case 3: PlayRAnimation(); break;
                default: Fallback(); break;
            }
        }

        #region 임시용
        private void HandleSkillFinish(int skillId)
        {
            switch (skillId)
            {
                case 0: PlayQAnimation(); break;
                case 1: PlayWAnimation(); break;
                case 2: FinishEAnimation(); break;
                case 3:
                    if (isFullCharged)
                    {
                        FinishRAniation_2();
                    }
                    else
                    {
                        FinishRAnimation();
                    }
                    break;
                default: Fallback(); break;
            }
        }



        private void PlayQAnimation()
        {
            SetTrigger("tSkill01");
        }

        private void PlayWAnimation()
        {
            Fallback();
        }

        private void PlayEAnimation()
        {
            SetBool("bSkill03", true);
        }

        private void FinishEAnimation()
        {
            SetBool("bSkill03", false);
        }
        private void PlayRAnimation()
        {
            SetBool("bSkill04", true);
            SetTrigger("tSkill04");
        }

        private void FinishRAnimation()
        {
            SetTrigger("tSkill04_2");
            SetBool("bSkill04", false);
            SetBool("bSkill04_02", false);
        }

        private void FinishRAniation_2()
        {
            SetBool("bSkill04_02", true);
            SetTrigger("tSkill04_2");
            isFullCharged = false;
        }

        public void FullCharged()
        {
            isFullCharged = true;
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Skill_04_FullCharged, transform.position);
        }
        #endregion

        private void SetTrigger(string name)
        {
            if (parameters.Contains(name))
            {
                animator.SetTrigger(name);
            }
            else
            {
                Fallback();
            }
        }

        private void SetFloat(string name, float value)
        {
            if (parameters.Contains(name))
            {
                animator.SetFloat(name, value);
            }
            else
            {
                Fallback();
            }
        }

        private void SetBool(string name, bool value)
        {
            if (parameters.Contains(name))
            {
                animator.SetBool(name, value);
            }
            else
            {
                Fallback();
            }
        }

        private void Fallback()
        {

        }

        public void AnimationEvent_AttackHit()
        {
            AnimationEventBus.RaiseAttackHit();
        }

        private void PlayFootStep()
        {
            SFXManager.Instance.PlayFootstep(Sound.FootstepType.Asphalt, transform.position);
        }
    }
}
