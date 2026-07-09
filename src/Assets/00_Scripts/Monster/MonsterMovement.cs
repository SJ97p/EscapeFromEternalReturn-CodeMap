using HBDinosaur_ER_Project.Database;
using UnityEngine;
using UnityEngine.AI;

namespace HBDinosaur_ER_Project.Monster
{
    public class MonsterMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Region _region;

        public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }
        private MonsterData _data;

        private void Awake()
        {
            if (_agent == null)
            {
                _agent = gameObject.AddComponent<NavMeshAgent>();
            }
            _agent = GetComponent<NavMeshAgent>();
        }
        private void Start()
        {
            AgentSetting();
        }
        public void Initialize(MonsterData data)
        {
            _data = data;
        }
                public void InitializeSpawnData(MonsterSpawnPointData spawnPoint)
        {
            if (_agent == null) return;

            // NavMeshAgent 비활성화 - 강제 이동 시 발생하는 충돌 방지
            _agent.enabled = false;

            if (NavMesh.SamplePosition(spawnPoint.Position, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                gameObject.transform.rotation = spawnPoint.Rotation;
            }
            else
            {
                transform.position = spawnPoint.Position;
            }

            // 위치 이동 후 다시 활성화
            _agent.enabled = true;
            _region = spawnPoint.Region;
        }
        private void AgentSetting()
        {
            _agent.acceleration = 10000f;
            _agent.speed = _data.moveSpeed;
            _agent.stoppingDistance = 2f;
        }
        public bool IsArrived()
        {
            if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh || !gameObject.activeInHierarchy)
            {
                return false;
            }
            if (_agent.pathPending)
            {
                return false;
            }
            return _agent.remainingDistance < _agent.stoppingDistance;
        }

        public void MoveTo(Vector3 pos)
        {
            _agent.isStopped = false;
            transform.LookAt(pos);
            _agent.SetDestination(pos);
        }

        public void SetMoveSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void StopMove()
        {
            _agent.isStopped = true;
        }
    }
}
