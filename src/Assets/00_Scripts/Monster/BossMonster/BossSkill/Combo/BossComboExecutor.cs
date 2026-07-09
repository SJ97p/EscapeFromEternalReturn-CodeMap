using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossComboExecutor : MonoBehaviour
    {
        private BossMonsterSkillManager _skillManager;
        private BossContext _context;

        private Coroutine _currentRoutine;

        public bool IsRunning { get; private set; }

        public void Init(BossMonsterSkillManager manager, BossContext con)
        {
            _skillManager = manager;
            _context = con;
        }

        public void StartCombo(BossCombatAction action)
        {
            if (IsRunning)
                return;

            IsRunning = true;

            _currentRoutine = StartCoroutine(RunCombo(action));
        }

        public void StopCombo()
        {
            if (_currentRoutine != null)
            {
                StopCoroutine(_currentRoutine);
                _currentRoutine = null;
            }

            IsRunning = false;
        }

        private IEnumerator RunCombo(BossCombatAction action)
        {
            yield return ExecuteAction(action);

            IsRunning = false;

            _currentRoutine = null;
        }

        private IEnumerator ExecuteAction(BossCombatAction action)
        {
            if (action is BossMonsterSkill skill)
            {
                BossMonsterSkillSlot slot = _skillManager.TryUseSkill(skill.skillType);

                if (slot == null)
                    yield break;

                while (slot.skill.IsFinished(slot) == false)
                    yield return null;
            }

            else if (action is BossComboAction combo)
            {
                foreach (var childAction in combo.actions)
                {
                    yield return StartCoroutine(ExecuteAction(childAction));
                }
            }
        }
    }
}