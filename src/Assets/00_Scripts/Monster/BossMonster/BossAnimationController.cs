using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossAnimationController : MonoBehaviour
    {
        Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        #region Animation
        public void SetAttackAnimSpeed(float index)
        {
            anim.SetFloat("attackSpeed", index);
        }

        public void SetMoveAnimSpeed(float index)
        {
            anim.SetFloat("moveSpeed", index);
        }

        public void SetWalkAnim(float index)
        {
            anim.SetFloat("moveVelocity", index);
        }

        public void StartDeadAnim()
        {
            anim.SetBool("dead", true);
        }

        public void EndDeadAnim()
        {
            anim.SetBool("dying", true);
            anim.SetBool("dead", false);
        }

        public void StartPatrolAnim()
        {
            anim.SetBool("bAppear", true);
            anim.SetTrigger("tAppear");
        }

        public void EndPatrolAnim()
        {
            anim.SetBool("bAppear", false);
        }

        public void NomalAttack1Anim()
        {
            anim.SetTrigger("tAttack01");
        }

        public void NomalAttack1Anim2()
        {
            anim.CrossFadeInFixedTime("Attack01", 0.05f);
        }

        public void NomalAttack2Anim()
        {
            anim.SetTrigger("tAttack02"); 
        }

        public void NomalAttack2Anim2()
        {
            anim.CrossFadeInFixedTime("Attack02", 0.05f);
        }

        public void SkillAttack1Anim()
        {
            anim.SetTrigger("tSkill01");
        }

        public void SkillAttack2Anim()
        {
            anim.SetTrigger("tSkill02");
        }

        public void SkillAttack3Anim()
        {
            anim.SetTrigger("tSkill04");
            anim.SetBool("bSkill04", true);
        }

        public void EndSkillAttack3Anim()
        {
            anim.SetBool("bSkill04", false);
        }

        public void StartMoveSkillAnim()
        {
            anim.SetBool("bSkill03", true);
            anim.SetTrigger("tSkill03");
        }

        public void EndMoveSkillAnim()
        {
            anim.SetBool("bSkill03", false);
        }

        public void EndBattleAnim()
        {
            anim.SetTrigger("tEndBattle");
        }

        public void StartDanceAnim()
        {
            anim.SetBool("bDance", true);
        }

        public void EndDanceAnim()
        {
            anim.SetBool("bDance", false);
        }
        #endregion
    }
}