using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [System.Serializable]
    public class BossMonsterSkillSlot
    {
        public BossMonsterSkill skill;
        public GameObject instantiatedHitbox;
        public MonsterSkillHitBox hitboxComponent;

        public Vector3 hitBoxScale;

        public float cooldown;
        public float currentTime;

        public bool isRunning;

        public bool usedOnce;

        public bool initialized = false;

        public void Init(BossContext con)
        {
            if (initialized)
                return;

            if (instantiatedHitbox == null)
            {
                if (skill.hitboxPrefab == null)
                {
                    initialized = true;
                    return;
                }

                instantiatedHitbox = Object.Instantiate(skill.hitboxPrefab);

                instantiatedHitbox.transform.localScale = hitBoxScale;

                instantiatedHitbox.SetActive(false);

                hitboxComponent = instantiatedHitbox.GetComponentInChildren<MonsterSkillHitBox>(true);
            }

            initialized = true;
        }

        public bool IsReady()
        {
            return currentTime >= cooldown;
        }

        public void UpdateTime()
        {
            currentTime += Time.deltaTime;
        }

        public void Reset()
        {
            currentTime = 0;
            if (skill.IsOneShot() == false)
                usedOnce = false;
        }

    }
}