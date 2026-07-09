using System;

namespace HBDinosaur_ER_Project.Player.Skill
{
    public abstract class SkillBase
    {
        protected SkillContext Context { get; private set; }
        public SkillContext GetContext() => Context;
        public SkillRawData Data { get; private set; }

        public SkillId SkillId { get; set; }
        public virtual bool IsOccupying => true;
        public bool IsCancelled { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action OnFinished;

        public virtual void ExecuteSameSkill() { }

        public void Initialize(SkillContext context, SkillRawData data)
        {
            Context = context;
            Data = data;
            OnInitialize();
        }

        public void Cast()
        {
            //if (IsRunning) return;

            IsRunning = true;
            IsCancelled = false;

            OnCast();
        }

        public void Cancel()
        {
            if (!IsRunning) return;

            IsCancelled = true;
            OnCancel();
            Finish();
        }

        public void Finish()
        {
            if (!IsRunning) return;

            IsRunning = false;
            OnFinish();
            OnFinished?.Invoke();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnCast() { }
        protected virtual void OnCancel() { }
        protected virtual void OnFinish() { }
    }
}