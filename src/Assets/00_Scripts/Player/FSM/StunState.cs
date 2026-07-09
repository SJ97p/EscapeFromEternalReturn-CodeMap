using HBDinosaur_ER_Project.Common;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class StunState : IPlayerState
    {
        private PlayerFSM fsm;
        private float timer;
        private float stunDuration;
        private GameObject stunVFX;
        public bool IsOccupying => true;

        public int Priority => 100;
        public StunState(PlayerFSM fsm)
        {
            this.fsm = fsm;
        }
        public void Enter()
        {
            timer = 0f;
            Vector3 pos = fsm.transform.position + new Vector3(0, 1.5f, 0);
            Quaternion rot = fsm.transform.rotation * Quaternion.Euler(-90, 0, 0);
            stunVFX = VFXManager.Instance.GetVFX(Effects.EffectType.Stun, pos, rot);
            fsm.Movement.StopMove();
        }

        public void SetDuration(float duration)
        {
            stunDuration = duration;
        }

        public void Exit()
        {
            VFXManager.Instance.ReleaseVFX(stunVFX);
            stunVFX = null;
            Debug.Log("스턴 종료");
        }

        public void Update()
        {
            timer += Time.deltaTime;
            if (timer >= stunDuration)
            {
                fsm.ChangeState(fsm.Idle);
            }
        }
    }
}