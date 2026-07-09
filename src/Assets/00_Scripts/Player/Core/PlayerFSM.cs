using HBDinosaur_ER_Project.Player.Skill;
using System;
using UnityEngine;
namespace HBDinosaur_ER_Project.Player
{
    public enum PlayerFSMState
    {
        IDLE,
        ATTACK,
        SKILL,
        MOVE,
        INTERACT,
        STUN,
        KNOCKBACK,
        DIE,
    }


    public class PlayerFSM : MonoBehaviour
    {
        public IPlayerState CurrentState { get; private set; }

        public event Action<IPlayerState> OnStateChanged;
        public IdleState Idle { get; private set; }
        public AttackState Attack { get; private set; }
        public SkillState Skill { get; private set; }
        public MoveState Move { get; private set; }
        public InteractState Interact { get; private set; }
        public StunState Stun { get; private set; }
        public KnockbackState Knockback { get; private set; }
        public DeadState Dead { get; private set; }

        public PlayerMove Movement { get; private set; }
        public PlayerStat Stat { get; private set; }
        public SkillManager SkillManager { get; private set; }
        public SkillCaster SkillCaster { get; private set; }

        private void Awake()
        {
            Movement = GetComponent<PlayerMove>();
            Stat = GetComponent<PlayerStat>();
            SkillManager = GetComponent<SkillManager>();
            SkillCaster = GetComponent<SkillCaster>();

            Idle = new IdleState(this);
            Attack = new AttackState(this);
            Skill = new SkillState(this);
            Move = new MoveState(this);
            Interact = new InteractState(this);
            Stun = new StunState(this);
            Knockback = new KnockbackState(this);
            Dead = new DeadState(this);

            ChangeState(Idle);
        }

        public void StateConverter(PlayerFSMState state)
        {
            IPlayerState iState = null;
            switch (state)
            {
                case PlayerFSMState.IDLE: iState = Idle; break;
                case PlayerFSMState.ATTACK: iState = Attack; break;
                case PlayerFSMState.MOVE: iState = Move; break;
                case PlayerFSMState.INTERACT: iState = Interact; break;
                case PlayerFSMState.SKILL: iState = Skill; break;
                case PlayerFSMState.STUN: iState = Stun; break;
                case PlayerFSMState.KNOCKBACK: iState = Knockback; break;
                case PlayerFSMState.DIE: iState = Dead; break;
                default: iState = Idle; break;
            }
            if (iState == null) return;
            if (CurrentState != null && CurrentState.IsOccupying)
            {
                if (iState.Priority < CurrentState.Priority)
                {
                    Debug.Log($"{CurrentState}가 {iState}보다 우선순위가 높습니다.");
                    return;
                }
            }

            ChangeState(iState);
        }

        public void ChangeState(IPlayerState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(newState);
        }

        private void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }
    }

}