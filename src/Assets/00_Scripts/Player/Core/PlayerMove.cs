using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Database;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerMove : MonoBehaviour
    {
        private NavMeshAgent agent;
        private PlayerStat playerStat;
        private PlayerFSM playerFSM;
        private float speedMultiplier = 1f;
        private bool movable = true;

        [SerializeField] private float hyperloopCastingTime = 2.5f;
        private Coroutine hyperloopCoroutine;

        public bool Movable
        {
            get => movable;
            set
            {
                movable = value;
            }
        }

        private float BaseSpeed
        {
            get
            {
                if (playerStat == null)
                    playerStat = GetComponent<PlayerStat>();
                return playerStat != null ? playerStat.MoveSpeed : (agent != null ? agent.speed : 0f);
            }
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            playerStat = GetComponent<PlayerStat>();
            playerFSM = GetComponent<PlayerFSM>();
        }

        private void Start()
        {
            UpdateSpeed();
        }

        private void Update()
        {
            if (IsArrived())
            {
                StopMove();
                InputEventBus.RaiseArrived();
            }
        }

        private bool IsArrived()
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh || !gameObject.activeInHierarchy)
                return true;

            if (agent.pathPending)
                return false;

            return agent.remainingDistance < agent.stoppingDistance;
        }

        public void MoveToTargetWithDistance(Vector3 targetPos, float interactableDistance)
        {
            Vector3 playerPos = transform.position;
            Vector3 dir = (targetPos - playerPos).normalized;

            Vector3 destination = targetPos - dir * interactableDistance;
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                SetMovePath(hit.position);
            }
        }

        public void SetMovePath(Vector3 pos)
        {
            if (!movable) return;

            NavMeshPath path = new();
            if (NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, path))
            {
                MoveTo(pos, speedMultiplier);
            }
        }

        public void StopMove()
        {
            if (agent == null) return;
            agent.ResetPath();
            agent.isStopped = true;
            AnimationEventBus.RaiseMove(0f);
        }

        public void SetMovable(bool movable)
        {
            Movable = movable;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = multiplier;
            UpdateSpeed();
        }

        public void ResetSpeed()
        {
            speedMultiplier = 1f;
            UpdateSpeed();
        }

        public void UpdateSpeed()
        {
            if (agent != null)
            {
                agent.speed = BaseSpeed * speedMultiplier;
            }
        }

        public void Teleport(Vector3 pos, Region targetRegion)
        {
            StopMove();
            if (agent != null && agent.enabled)
            {
                agent.Warp(pos);
            }
            else
            {
                transform.position = pos;
            }
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.hyperloop_arrive, transform.position);
            PlayerRegionTracker tracker = GetComponent<PlayerRegionTracker>();
            if (tracker != null)
            {
                tracker.SetRegion(targetRegion);
            }
        }

        public void StartHyperloopCasting(Vector3 destination, Region targetRegion)
        {
            CancelHyperloopCasting();

            StopMove();
            VoiceManager.Instance.PlayVoice(Sound.VoiceSituation.UseHyperloop);
            SFXManager.Instance.PlaySFX3D(Sound.SFXType.hyperloop_Operate, transform.position);
            if (playerFSM != null)
            {
                playerFSM.OnStateChanged += HandleFSMStateChanged;
                playerFSM.StateConverter(PlayerFSMState.IDLE);
            }

            hyperloopCoroutine = StartCoroutine(HyperloopCastingRoutine(destination, targetRegion));

        }

        public void CancelHyperloopCasting()
        {
            if (hyperloopCoroutine != null)
            {
                StopCoroutine(hyperloopCoroutine);
                hyperloopCoroutine = null;

                if (playerFSM != null)
                {
                    playerFSM.OnStateChanged -= HandleFSMStateChanged;
                }
                SFXManager.Instance.StopSFX(Sound.SFXType.hyperloop_Operate);
                Debug.Log("[Hyperloop] 텔레포트 캐스팅이 취소되었습니다.");
            }
        }

        private IEnumerator HyperloopCastingRoutine(Vector3 destination, Region targetRegion)
        {
            Debug.Log($"[Hyperloop] {hyperloopCastingTime}초 후 텔레포트합니다...");
            yield return new WaitForSeconds(hyperloopCastingTime);

            if (playerFSM != null)
            {
                playerFSM.OnStateChanged -= HandleFSMStateChanged;
            }
            hyperloopCoroutine = null;

            Teleport(destination, targetRegion);
            AnimationEventBus.RaiseTelportEnd();
            Debug.Log("[Hyperloop] 텔레포트 완료!");
        }

        private void HandleFSMStateChanged(IPlayerState newState)
        {
            if (newState is MoveState ||
                newState is AttackState ||
                newState is SkillState ||
                newState is StunState ||
                newState is KnockbackState ||
                newState is DeadState)
            {
                Debug.Log($"[Hyperloop] 플레이어 행동({newState.GetType().Name})으로 인해 텔레포트 캐스팅이 취소되었습니다.");
                CancelHyperloopCasting();
            }
        }

        private void MoveTo(Vector3 pos, float speed = 1f)
        {
            transform.LookAt(pos);
            agent.isStopped = false;
            agent.SetDestination(pos);
            AnimationEventBus.RaiseMove(speed);
        }
    }
}