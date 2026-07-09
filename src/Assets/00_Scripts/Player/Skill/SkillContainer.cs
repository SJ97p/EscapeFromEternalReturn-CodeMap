using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Player.Skill
{
    public enum ActionExecutionType
    {
        Sequential,
        Parallel
    }

    public class SkillContainer : SkillBase
    {
        private List<SkillAction> actions = new();
        private int currentActionIndex = 0;
        private int finishedActionCount = 0;

        protected ActionExecutionType executionType = ActionExecutionType.Sequential;

        public SkillContainer AddAction(SkillAction action)
        {
            actions.Add(action);
            return this;
        }

        public override void ExecuteSameSkill()
        {
            if (executionType == ActionExecutionType.Sequential && currentActionIndex < actions.Count)
            {
                actions[currentActionIndex].Execute(Context);
            }
        }

        protected override void OnCast()
        {
            currentActionIndex = 0;
            finishedActionCount = 0;

            foreach (var action in actions)
            {
                action.ResetAction();
            }

            if (executionType == ActionExecutionType.Sequential)
            {
                ExecuteNextAction();
            }
            else
            {
                ExecuteActions();
            }
        }

        private void ExecuteActions()
        {
            foreach (SkillAction action in actions)
            {
                action.OnSkillFinished += OnActionsFinished;
                action.Execute(Context);
            }
        }

        private void ExecuteNextAction()
        {
            if (IsCancelled || currentActionIndex >= actions.Count)
            {
                Finish();
                return;
            }

            SkillAction action = actions[currentActionIndex];
            action.OnSkillFinished += OnActionFinished;
            action.Execute(Context);
        }

        private void OnActionFinished()
        {
            SkillAction action = actions[currentActionIndex];
            action.OnSkillFinished -= OnActionFinished;

            currentActionIndex++;
            ExecuteNextAction();
        }

        private void OnActionsFinished()
        {
            finishedActionCount++;

            if (IsCancelled || finishedActionCount >= actions.Count)
            {
                foreach (var action in actions)
                {
                    action.OnSkillFinished -= OnActionsFinished;
                }
                Finish();
                return;
            }
        }
    }
}