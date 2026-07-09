using UnityEngine;


namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/SummonSkill")]
    public class Summon : BossMonsterSkill
    {
        [SerializeField] private float _TimeToSummon = 5f;
        [SerializeField] private GameObject _summonedMonsterPrefab;
        private int _requiredDamage = 0;
        private float _summonTimer = 0f;
        private bool _isSummoned = false;
        
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
            if (_isSummoned == false && skillSlot.isRunning == false)
                return true;

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                _isSummoned = false;
                skillSlot.isRunning = false;
            }

            if (_isSummoned == true)
            {
                int cumulativeDamage = con.combat.GetCumulativeDamage();

                if (_summonTimer > Time.time && cumulativeDamage >= _requiredDamage)
                {
                    con.combat.CancelSummon();
                    con.animation.EndSkillAttack3Anim();
                    _isSummoned = false;
                    skillSlot.isRunning = false;
                    skillSlot.Reset();
                }
                else if (_summonTimer <= Time.time && cumulativeDamage < _requiredDamage)
                {
                    Debug.Log("Summoning Success.");
                    Instantiate(_summonedMonsterPrefab, con.owner.transform.position + new Vector3(2f, 0, 0), Quaternion.identity);
                    con.animation.EndSkillAttack3Anim();
                    _isSummoned = false;
                    skillSlot.isRunning = false;
                    skillSlot.Reset();
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            _isSummoned = false;

            con.animation.SkillAttack3Anim();

            _requiredDamage = SetRequiredDamage(con.phase);

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            con.combat.Summon(_TimeToSummon);
            _summonTimer = Time.time + _TimeToSummon;
            _isSummoned = true;
        }

        private int SetRequiredDamage(int phase)
        {
            switch (phase)
            {
                case 1:
                    return 10;
                case 2:
                    return 20;
                case 3:
                    return 30;
                default:
                    return 10;
            }
        }
    }
}