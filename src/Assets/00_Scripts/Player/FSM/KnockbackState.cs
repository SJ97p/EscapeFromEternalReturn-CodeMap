using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class KnockbackState : IPlayerState
    {
        private PlayerFSM _fsm;
        private float _speed;
        private float _distance;
        private float _moved;
        private Vector3 _direction;
        public bool IsOccupying => true;
        public int Priority => 100;

        public KnockbackState(PlayerFSM fsm)
        {
            _fsm = fsm;
        }

        public void SetData(Vector3 direction, float knockbackSpeed, float knockbackDistacne)
        {
            _direction = direction;
            _direction.y = 0f;
            _direction.Normalize();
            _speed = knockbackSpeed;
            _distance = knockbackDistacne;
        }

        public void Enter()
        {
            _moved = 0f;
            _fsm.Movement.StopMove();
        }

        public void Exit()
        {

        }

        public void Update()
        {
            float dt = Time.deltaTime;
            if (_moved > _distance)
            {
                Debug.Log("넉백 종료");
                _fsm.ChangeState(_fsm.Idle);
            }
            float step = _speed * dt;

            _moved += step;

            _fsm.gameObject.transform.position += _direction * step;
        }
    }
}