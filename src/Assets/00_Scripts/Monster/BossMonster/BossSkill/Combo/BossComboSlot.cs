using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [System.Serializable]
    public class BossComboSlot
    {
        [Header("Combo")]
        public BossCombatAction action;

        [Header("ComboName")]
        public string name;

        [Header("Cooldown")]
        public float cooldown = 5f;

        [SerializeField]
        private float currentCooldown;

        [Header("Priority")]
        public int priority = 0;

        [Header("Random Weight")]
        public int weight = 100;

        [Header("Option")]
        public bool oneShot;

        [HideInInspector]
        public bool usedOnce;

        public void Tick()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }

        public bool IsReady()
        {
            if (action == null)
                return false;

            if (oneShot == true && usedOnce == true)
                return false;

            return currentCooldown <= 0;
        }

        public void Use()
        {
            currentCooldown = cooldown;

            if (oneShot == true)
                usedOnce = true;
        }

        public void ResetCooldown()
        {
            currentCooldown = 0;
        }

        public float GetRemainCooldown()
        {
            return currentCooldown;
        }
    }
}