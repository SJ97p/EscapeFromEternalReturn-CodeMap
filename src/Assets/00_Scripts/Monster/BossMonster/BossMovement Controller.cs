using UnityEngine;
using UnityEngine.AI;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossMovementController : MonoBehaviour
    {
        public NavMeshAgent Agent { get { return agent; } }

        private MonsterData runtimeData;
        private NavMeshAgent agent;
        private void Awake()
        {
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
            }
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            AgentSetting();
        }

        #region Setting Method

        public void Init(MonsterData data)
        {
            runtimeData = data;
        }

        private void AgentSetting()
        {
            agent.acceleration = 10000f;
            agent.speed = runtimeData.moveSpeed;
            agent.stoppingDistance = 2f;
        }
        #endregion

        #region Run Method

        public Vector3 SetRandomPoint()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 randomPos = transform.position + new Vector3(Random.Range(-15, 15), 0f, Random.Range(-15, 15));

                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPos, out hit, 10f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }

        public bool IsArrived(Vector3 pos)
        {
            float dis = Vector3.Distance(transform.position, pos);

            if (dis < agent.stoppingDistance)
            {
                return true;
            }

            return false;
        }

        public void Moveto(Vector3 pos)
        {
            agent.isStopped = false;
            gameObject.transform.LookAt(pos);
            agent.SetDestination(pos);
        }

        public void TelePort(Transform target, TeleportType type)
        {
            Vector3 pos = target.position;

            if (type == TeleportType.FrontOfTarget)
            {
                pos += target.forward * 2f;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(pos, out hit, 5f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    return;
                }
            }
            else if (type == TeleportType.BackOfTarget)
            {
                pos -= target.forward * 2f;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(pos, out hit, 5f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    return;
                }
            }
            else if (type == TeleportType.None)
            {
                pos = transform.position;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(pos, out hit, 5f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    return;
                }
            }
            else if (type == TeleportType.RandomAroundTarget)
            {
                bool success = false;

                for (int i = 0; i < 5; i++)
                {
                    NavMeshHit hit;

                    Vector3 teleportpos = pos + new Vector3(Random.Range(-5, 5), 0f, Random.Range(-5, 5));

                    if (NavMesh.SamplePosition(teleportpos, out hit, 5f, NavMesh.AllAreas))
                    {
                        transform.position = hit.position;
                        success = true;
                        break;
                    }
                }

                if (success == false)
                    Debug.Log("Teleport Fail");
            }
        }

        public void LookAtTarget()
        {
            Vector3 dir = agent.destination - transform.position;
            dir.y = 0;
            float dis = Vector3.Distance(agent.destination, transform.position);

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 30 * Time.deltaTime);
            }
        }

        public void SetMoveSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void MoveStop()
        {
            agent.isStopped = true;
        }

        public void MoveStart()
        {
            agent.isStopped = false;
        }

        #endregion
    }

    public enum TeleportType
    {
        None,
        RandomAroundTarget,
        FrontOfTarget,
        BackOfTarget
    }
}