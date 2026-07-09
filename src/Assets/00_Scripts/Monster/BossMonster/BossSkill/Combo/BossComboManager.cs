using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossComboManager : MonoBehaviour
    {
        [Header("Combo Slots")]
        [SerializeField] private List<BossComboSlot> comboSlots = new();

        private BossComboExecutor comboExecutor;
        private BossContext context;

        private BossComboSlot currentSlot;

        public void Init(BossContext con, BossComboExecutor executor)
        {
            context = con;
            comboExecutor = executor;
        }

        private void Update()
        {
            Tick();
        }

        private void Tick()
        {
            foreach (var slot in comboSlots)
            {
                if (slot == null)
                    continue;

                slot.Tick();
            }
        }

        public bool TryUseCombo(BossComboSlot slot)
        {
            if (slot == null)
                return false;

            if (IsComboRunning())
                return false;

            if (slot.action == null)
                return false;


            if (!slot.IsReady())
                return false;

            currentSlot = slot;

            comboExecutor.StartCombo(slot.action);

            slot.Use();

            return true;
        }

        public bool IsComboRunning()
        {
            return comboExecutor.IsRunning;
        }

        public BossComboSlot GetCurrentSlot()
        {
            return currentSlot;
        }

        public BossComboSlot SelectSlot(string name)
        {
            foreach(var slot in comboSlots)
            {
                if (slot.name == name)
                    return slot;

                continue;
            }

            return null;
        }

        public List<BossComboSlot> GetAllSlots()
        {
            return comboSlots;
        }

        public List<BossComboSlot> GetReadySlots()
        {
            List<BossComboSlot> readySlots = new();

            foreach (var slot in comboSlots)
            {
                if (slot == null)
                    continue;

                if (!slot.IsReady())
                    continue;

                readySlots.Add(slot);
            }

            return readySlots;
        }

        public BossComboSlot GetHighestPrioritySlot()
        {
            BossComboSlot bestSlot = null;

            foreach (var slot in comboSlots)
            {
                if (slot == null)
                    continue;

                if (!slot.IsReady())
                    continue;

                if (slot.action == null)
                    continue;


                if (bestSlot == null)
                {
                    bestSlot = slot;
                    continue;
                }

                if (slot.priority > bestSlot.priority)
                {
                    bestSlot = slot;
                }
            }

            return bestSlot;
        }

        public void ResetAllCooldown()
        {
            foreach (var slot in comboSlots)
            {
                if (slot == null)
                    continue;

                slot.ResetCooldown();
            }
        }
    }
}
