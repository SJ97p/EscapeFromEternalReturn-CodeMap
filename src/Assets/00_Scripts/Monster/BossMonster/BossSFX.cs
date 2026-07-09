using HBDinosaur_ER_Project.Common;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossSFX : MonoBehaviour
    {
        public void Wickline_Attack1Sound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Attack1, gameObject.transform.position);
        }

        public void Wickline_Attack2Sound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Attack2, gameObject.transform.position);
        }

        public void Wickline_DieSound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Die, gameObject.transform.position);
        }

        public void Wickline_HitSound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Hit, gameObject.transform.position);
        }

        public void Wickline_Skill1ReadySound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Skill1Ready, gameObject.transform.position);
        }

        public void Wickline_Skill1HitSound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Skill1Hit, gameObject.transform.position);
        }

        public void Wickline_Skill2ReadySound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Skill2Ready, gameObject.transform.position);
        }

        public void Wickline_Skill2ShotSound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Skill2Shot, gameObject.transform.position);
        }

        public void Wickline_Skill3ReadySound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Skill3Ready, gameObject.transform.position);
        }

        public void Wickline_Skill3HitSound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_Skill3Hit, gameObject.transform.position);
        }
        public void Wickline_MoveSkillSound()
        {
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.Wickline_MoveSkill, gameObject.transform.position);
        }
    }
}