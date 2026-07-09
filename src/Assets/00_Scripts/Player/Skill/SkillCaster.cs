using HBDinosaur_ER_Project.Common;
using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player.Skill
{
    public class SkillCaster : MonoBehaviour
    {
        public SkillBase CurrentSkill => currentSkill;
        private SkillBase currentSkill;
        private Coroutine runningCoroutine;

        private PlayerMove playermove;

        // 임시
        private SkillContext context;
        //
        public bool IsCasting { get; private set; }

        private void Awake()
        {
            playermove = GetComponent<PlayerMove>();
        }

        public void Cast(SkillBase skill, SkillContext context)
        {
            if (skill.IsOccupying && IsCasting)
            {
                return;
            }
            var data = SkillRegistry.GetRawData(skill.SkillId);
            skill.Initialize(context, data);

            if (skill.IsOccupying)
            {
                // 임시
                this.context = context;
                //
                currentSkill = skill;
                runningCoroutine = StartCoroutine(CastRoutine());
            }
            else
            {
                StartCoroutine(NonOccupyingCastRoutine(skill, context));
            }
        }

        public void Cancel()
        {
            if (!IsCasting) return;
            currentSkill?.Cancel();
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
                runningCoroutine = null;
            }
            EndCast();
        }

        public void HandleReCast()
        {
            if (IsCasting && currentSkill != null)
            {
                currentSkill.ExecuteSameSkill();
            }
        }

        private IEnumerator CastRoutine()
        {
            IsCasting = true;

            SetMovement(false);
            // 임시
            AnimationEventBus.RaiseSkill(context.SkillNum);
            //
            bool finished = false;
            currentSkill.OnFinished += () => finished = true;
            currentSkill.Cast();

            while (!finished)
            {
                yield return null;
            }
            EndCast();
        }

        private IEnumerator NonOccupyingCastRoutine(SkillBase skill, SkillContext context)
        {
            bool finished = false;
            skill.OnFinished += () => finished = true;
            skill.Cast();

            while (!finished)
            {
                yield return null;
            }
        }

        private void EndCast()
        {
            IsCasting = false;
            // 임시
            AnimationEventBus.RaiseSkillFinish(context.SkillNum);
            //
            SetMovement(true);
            currentSkill = null;
            runningCoroutine = null;
        }

        private void SetMovement(bool movable)
        {
            if (playermove != null)
            {
                playermove.SetMovable(movable);

                if (!movable)
                    playermove.StopMove();
            }
        }
    }
}