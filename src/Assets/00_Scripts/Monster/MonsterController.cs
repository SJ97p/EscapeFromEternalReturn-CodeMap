using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.Sound;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace HBDinosaur_ER_Project.Monster
{
    public class MonsterController : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private SFXType _attackSFX;
        [SerializeField] private SFXType _hitSFX;
        [SerializeField] private SFXType _wakeUpSFX;
        [SerializeField] private SFXType _dieSFX;

        private NavMeshAgent agent;
        [Header("MonsterInfo")]
        [SerializeField] private MonsterData originData;
        [SerializeField] private GameObject _containerPrefab;
        [SerializeField] private MonsterSenser senser;
        [SerializeField] private Canvas hpCanvas;
        [SerializeField] private MonsterType changeMonsterType;

        private Collider collider;
        private MonsterHpUI monsterHpUI;
        private MonsterSkillManager skillManager;
        private MonsterContext context;
        private Animator anim;
        private IIdleFSM idleFSM;
        private IAggroFSM aggroFSM;
        private IReturnFSM returnFSM;
        private IDeadFSM deadFSM;
        private StunFSM stunFSM;
        private float dis = 0;
        private float targetRange = 0;
        private float regenTimer;
        private Region _region;

        public MonsterStateMachine stateMachine;
        public MonsterBlackboard blackboard;
        public MonsterData runtimeData;
        public bool abnormalStatus;

        public StunFSM StunFSM { get { return stunFSM; } }
        public IIdleFSM IdleFSM { get { return idleFSM; } }
        public IAggroFSM AggroFSM { get { return aggroFSM; } }
        public NavMeshAgent Agent { get { return agent; } }
        public Canvas HPCanvas { get { return hpCanvas; } }
        public MonsterHpUI MonsterHpUI { get { return monsterHpUI; } }
        public float PatrolTimer { get { return PatrolTimer; } }

        public event Action<MonsterController> OnDead;

        private void Awake()
        {
            context = new MonsterContext();
            blackboard = new MonsterBlackboard();
            stateMachine = new MonsterStateMachine();
            collider = GetComponent<Collider>();
            monsterHpUI = GetComponentInChildren<MonsterHpUI>();
            anim = GetComponent<Animator>();

            if (gameObject.GetComponentInChildren<MonsterSkillHitBox>())
                context.skillHitBox = gameObject.GetComponentInChildren<MonsterSkillHitBox>();

            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }

            runtimeData = Instantiate(originData);

            context.blackboard = blackboard;
            context.owner = gameObject;
            context.runtimeData = runtimeData;

            skillManager = gameObject.GetComponent<MonsterSkillManager>();
            skillManager?.Init(this, context);

            if (gameObject.GetComponentInChildren<MonsterSenser>())
                senser = gameObject.GetComponentInChildren<MonsterSenser>();

            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                agent = GetComponent<NavMeshAgent>();
            }

            idleFSM = new IIdleFSM(blackboard, this);
            aggroFSM = new IAggroFSM(blackboard, this, skillManager);
            returnFSM = new IReturnFSM(blackboard, this);
            deadFSM = new IDeadFSM(this);
            stunFSM = new StunFSM(this);

            agent.enabled = false;
            agent.speed = runtimeData.moveSpeed;
            blackboard.currentHp = runtimeData.maxHp;
            blackboard.attackRange = runtimeData.attackRange;
            blackboard.maxChaseRange = runtimeData.chaseRange;
            blackboard.originPos = transform.position;
        }

        private void Start()
        {

        }

        private void OnEnable()
        {
            TimeManager.Instance.OnStateChanged += ChangeMonsterEvent;
            ResetMonster();
        }

        private void OnDisable()
        {
            TimeManager.Instance.OnStateChanged -= ChangeMonsterEvent;
        }

        private void Update()
        {
            if (abnormalStatus == true || agent.enabled == false)
                return;

            RegenHP();

            if (blackboard.isfight == true)
                blackboard.battleTime += Time.deltaTime;
            else if (blackboard.isfight == false)
                blackboard.battleTime = 0;

            if (stateMachine.fsm == idleFSM)
                blackboard.patrolTime += Time.deltaTime;
            else
                blackboard.patrolTime = 0;


            if (senser.Target != null)
            {
                blackboard.target = senser.Target.transform;
                context.target = senser.Target;
            }
            else
            {
                blackboard.target = null;
                context.target = null;
                blackboard.isfight = false;
            }

            if (blackboard.target != null)
            {
                dis = Vector3.Distance(blackboard.originPos, transform.position);
                targetRange = Vector3.Distance(blackboard.target.position, transform.position);
            }


            if (agent.remainingDistance > agent.stoppingDistance && stateMachine.fsm == returnFSM)
            {
                stateMachine.Update();
                return;
            }


            if (runtimeData.firstStrike == true && targetRange < blackboard.maxChaseRange && blackboard.isfight == false && stateMachine.fsm != returnFSM)
            {
                blackboard.originPos = transform.position;
                blackboard.isfight = true;
            }


            if (blackboard.currentHp <= 0)
            {
                stateMachine.ChangeStateFSM(deadFSM);
            }
            else if (runtimeData.leashRange < dis ||
                blackboard.target == null && Vector3.Distance(transform.position, blackboard.originPos) > 0.5f)
            {
                stateMachine.ChangeStateFSM(returnFSM);
            }
            else if (blackboard.target != null)
            {
                stateMachine.ChangeStateFSM(aggroFSM);
            }
            else if (!blackboard.isfight)
            {
                StopMove();
                stateMachine.ChangeStateFSM(idleFSM);
            }


            stateMachine.Update();
        }

        private void ChangeMonsterEvent(int o, InGameState state)
        {
            Vector3 pos = gameObject.transform.position;
            Quaternion rot = gameObject.transform.rotation;
            Vector3 Scale = gameObject.transform.localScale;

            var spawnPoint = new MonsterSpawnPointData(changeMonsterType, _region, pos, rot, Scale);

            if (state == InGameState.NIGHT)
                return;

            var monsterData = GameDataStore.Instance.MonsterDatabase.monsters
            .FirstOrDefault(x => x.Type == changeMonsterType);

            Debug.Log(changeMonsterType);
            var monster = PoolManager.Instance.Get(monsterData.Prefab).GetComponent<MonsterController>();
            monster.InitializeData(spawnPoint);
            OnDead?.Invoke(this);
            PoolManager.Instance.Release(gameObject);
            return;
        }

        public void RegenHP()
        {
            if (blackboard.currentHp >= runtimeData.maxHp) return;

            if (blackboard.isfight == true) return;

            Debug.Log("Regen");

            regenTimer += Time.deltaTime;

            if (regenTimer >= 1f)
            {
                regenTimer = 0f;

                blackboard.currentHp += runtimeData.regenHp;

                blackboard.currentHp = Mathf.Clamp(blackboard.currentHp, 0, runtimeData.maxHp);

                monsterHpUI.UpdateHpBar(blackboard.currentHp);
            }
        }

        private void ResetMonster()
        {
            StopAllCoroutines();
            if (!agent.isOnNavMesh) return;

            abnormalStatus = false;

            blackboard.target = null;
            blackboard.isfight = false;
            blackboard.isDead = false;
            blackboard.isAttacking = false;
            blackboard.isUsingSkill = false;
            blackboard.isReturn = false;

            blackboard.currentHp = runtimeData.maxHp;

            blackboard.originPos = transform.position;

            regenTimer = 0;

            collider.isTrigger = false;

            agent.enabled = false;
            agent.enabled = true;

            if (!agent.isOnNavMesh) return;
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            agent.speed = runtimeData.moveSpeed;

            anim.Rebind();
            anim.Update(0f);

            monsterHpUI.UpdateMaxHp(runtimeData.maxHp);
            monsterHpUI.UpdateHpBar(blackboard.currentHp);

            stateMachine.ChangeStateFSM(idleFSM);

            StartCoroutine(AppearedAnim());
        }

        public void InCombat()
        {
            anim.SetBool("bInCombat", true);
        }

        public void Attack()
        {
            if (blackboard.target != null && blackboard.isAttacking == false)
            {
                blackboard.isAttacking = true;
                blackboard.isAttackFinished = false;
                transform.LookAt(blackboard.target);

                anim.SetTrigger("tAttack01");
            }
        }

        public void EnterBattle(Transform target)
        {
            blackboard.target = target;
            blackboard.isfight = true;

            MovetoTarget();
        }

        public void AttackAnimationEnd()
        {
            blackboard.isAttacking = false;
            blackboard.isAttackFinished = true;
        }

        public void Die()
        {
            if (blackboard.isDead) return;

            OnDead?.Invoke(this);
            collider.isTrigger = true;
            agent.enabled = false;
            blackboard.currentHp = 0;
            blackboard.isDead = true;
        }

        public void DieEvent()
        {
            PoolManager.Instance.Get(_containerPrefab, transform.position, transform.rotation);
            PoolManager.Instance.Release(gameObject);
        }

        public void MovetoTarget()
        {
            if (blackboard.target == null)
            {
                agent.isStopped = true;
                return;
            }
            anim.SetBool("bBeware", false);

            float distance = Vector3.Distance(transform.position, blackboard.target.position);

            agent.stoppingDistance = runtimeData.attackRange;

            if (distance > runtimeData.attackRange)
            {
                Vector3 dir = agent.destination - transform.position;
                dir.y = 0;

                if (dir != Vector3.zero)
                {
                    Quaternion rot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 30 * Time.deltaTime);
                }
                anim.SetFloat("moveVelocity", 1);
                agent.isStopped = false;
                agent.SetDestination(blackboard.target.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }

        public void MovetoPatrolPos()
        {
            if (blackboard.hasPatrolPos == false)
            {
                agent.isStopped = true;
                return;
            }

            if (agent.destination != blackboard.patrolPos)
            {
                Vector3 dir = agent.destination - transform.position;
                dir.y = 0;
                float dis = Vector3.Distance(agent.destination, transform.position);

                if (dir != Vector3.zero)
                {
                    Quaternion rot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 30 * Time.deltaTime);
                }
                anim.SetFloat("moveVelocity", 1);
                agent.isStopped = false;
                agent.SetDestination(blackboard.patrolPos);
            }
        }

        public void MovetoOriginPos()
        {
            if (blackboard.isDead == false)
            {
                Vector3 dir = agent.destination - transform.position;
                dir.y = 0;

                if (dir != Vector3.zero)
                {
                    Quaternion rot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 30 * Time.deltaTime);
                }
                anim.SetFloat("moveVelocity", 1);
                agent.isStopped = false;
                agent.SetDestination(blackboard.originPos);
                agent.stoppingDistance = 0.1f;
            }
        }

        public void StopMove()
        {
            if (!agent.isOnNavMesh) return;
            anim.SetFloat("moveVelocity", 0);
            agent.isStopped = true;
        }

        public void EndBattleAnim()
        {
            blackboard.isReturn = true;
            anim.SetBool("bEndBattle", false);
        }

        public void EndBewareAnim()
        {
            anim.SetBool("bInCombat", false);
        }

        public void EndAppearedAnim()
        {
            stateMachine.ChangeStateFSM(idleFSM);
            anim.SetBool("bAppear", false);
        }

        public void BoundaryAnim()
        {
            if (anim.GetBool("bBeware") == true)
                return;

            anim.SetBool("bBeware", true);
            anim.SetTrigger("tBeware");
        }

        public void EndBoundaryAnim()
        {
            anim.SetBool("bBeware", false);
        }

        public void DieAnim()
        {
            anim.SetBool("dead", true);
        }

        public void EndDieAnim()
        {
            anim.SetBool("dead", false);
            anim.SetBool("dying", true);
        }

        public void SkillAnim()
        {
            blackboard.isUsingSkill = true;
            anim.SetBool("bSkill01", true);
            anim.SetTrigger("tSkill01");
        }

        public void EndSkillAnim()
        {
            blackboard.isUsingSkill = false;
            anim.SetBool("bSkill01", false);
        }

        public IEnumerator AppearedAnim()
        {
            yield return null;
            anim.SetBool("bAppear", true);
            anim.SetTrigger("tAppear");
        }

        public IEnumerator ReturnAnim()
        {
            StopMove();
            blackboard.isReturn = false;
            anim.SetBool("bEndBattle", true);
            yield return new WaitForSeconds(2);
            anim.SetTrigger("tEndBattle");
        }

        public IEnumerator IdleAnim()
        {
            yield return null;
            anim.SetBool("bInCombat", true);
            yield return new WaitForSeconds(3);
            if (stateMachine.fsm != idleFSM)
                yield break;
            anim.SetTrigger("tEndBeware");
        }

        public void InitializeData(MonsterSpawnPointData spawnPoint)
        {
            if (agent == null) return;

            // NavMeshAgent ?????? - ???? ??? ?? ?????? ?�� ????
            agent.enabled = false;

            if (NavMesh.SamplePosition(spawnPoint.Position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                gameObject.transform.rotation = spawnPoint.Rotation;
            }
            else
            {
                Debug.LogWarning($"NavMesh ???? ????: {spawnPoint.Position}");
                transform.position = spawnPoint.Position;
            }

            // ???? ????? originPos?? ??????? ???? ?? ???? ???? ????
            blackboard.originPos = transform.position;

            // ??? ??? ?? ??? ????
            agent.enabled = true;
            _region = spawnPoint.Region;
        }

        public void AttackSound()
        {
            SFXManager.Instance.PlaySFX3D(_attackSFX, gameObject.transform.position);
        }

        public void HitSound()
        {
            SFXManager.Instance.PlaySFX3D(_hitSFX, gameObject.transform.position);
        }

        public void DieSound()
        {
            SFXManager.Instance.PlaySFX3D(_dieSFX, gameObject.transform.position);
        }

        public void WakeUpSound()
        {
            SFXManager.Instance.PlaySFX3D(_wakeUpSFX, gameObject.transform.position);
        }
    }
}
