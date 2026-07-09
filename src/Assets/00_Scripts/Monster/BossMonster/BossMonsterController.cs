using HBDinosaur_ER_Project.Core;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossMonsterController : MonoBehaviour
    {
        [SerializeField] private BossBlackBoardData _blackBoardData;
        [SerializeField] private MonsterData _originData;
        [SerializeField] private Canvas _hpCanvas;
        [SerializeField] private GameObject _containerPrefab;
        [SerializeField] private GameObject _firePosition;
        private BossBlackBoard _blackBoard = new BossBlackBoard();
        private BossContext _context = new BossContext();
        private BossMonsterSkillManager _skillManager;
        private BossComboManager _comboManager;
        private BossComboExecutor _comboExecutor;
        private BossMonsterHpUI _monsterHpUI;
        private MonsterSenser _senser;

        private BehaviorTree _BossMonsterBT;

        private BossBlackBoardKey _startBattlePos;
        private BossBlackBoardKey _inBattle;
        private BossBlackBoardKey _actionTimer;
        private BossBlackBoardKey _teleportTimer;
        private BossBlackBoardKey _monsterState;

        public Canvas Hpcanavas { get {return _hpCanvas; } }
        public BossContext Context { get { return _context; } }
        public BossMonsterHpUI MonsterHpUI { get { return _monsterHpUI; } }


        private void Awake()
        {
            GetComponents();
            ContextSetting();
            _blackBoardData.SetValuesOnBlackBoard(_blackBoard);
            BlackBoardSetting();
            _monsterHpUI.UpdateHpBar(_context.runtimeMonsterData.currentHp);
        }
        private void Start()
        {
            _skillManager.Init(_context);
            _comboExecutor.Init(_skillManager, _context);
            _comboManager.Init(_context, _comboExecutor);
            BossMonsterBTSetting();
        }

        private void Update()
        {
            if (Context.dead == true)
                return;

            if (_senser.Target != null)
                _context.target = _senser.Target.transform;
            else _context.target = null;

            NextActionTimers();

            _BossMonsterBT.Process();
        }

        private void GetComponents()
        {
            _senser = gameObject.GetComponentInChildren<MonsterSenser>();
            _monsterHpUI = gameObject.GetComponentInChildren<BossMonsterHpUI>();
            _skillManager = gameObject.GetComponent<BossMonsterSkillManager>();
            _comboManager = gameObject.GetComponent<BossComboManager>();
            _comboExecutor = gameObject.GetComponent<BossComboExecutor>();
        }

        private void ContextSetting()
        {
            _context.owner = gameObject;
            _context.combo = GetComponent<BossComboManager>();
            _context.movement = GetComponent<BossMovementController>();
            _context.animation = GetComponent<BossAnimationController>();
            _context.projectilePool = GetComponentInChildren<BossProjectilePool>();
            _context.runtimeMonsterData = Instantiate(_originData);
            _context.runtimeMonsterData.currentHp = _context.runtimeMonsterData.maxHp;
            _context.movement.Init(_context.runtimeMonsterData);
            _context.firePosition = _firePosition.transform;
            _context.phase = 1;
        }

        private void BlackBoardSetting()
        {
            _startBattlePos = _blackBoard.GetorRegisterKey("StartBattlePos");
            _inBattle = _blackBoard.GetorRegisterKey("InBattle");
            _actionTimer = _blackBoard.GetorRegisterKey("ActionTimer");
            _teleportTimer = _blackBoard.GetorRegisterKey("TeleportTimer");
            _monsterState = _blackBoard.GetorRegisterKey("MonsterState");
        }

        private void BossMonsterBTSetting()
        {
            _BossMonsterBT = new BehaviorTree("BossMonsterBT");
            PrioritySelector prioritySelector = new PrioritySelector("PrioritySelector");
            Selector combatSelector = new Selector("CombatSelector", 4);
            Sequence phase1 = new Sequence("Phase1Sequence");
            PrioritySelector phase1Combat = new PrioritySelector("Phase1CombatPrioritySelector");
            Sequence phase2 = new Sequence("Phase2Squence");
            PrioritySelector phase2Combat = new PrioritySelector("Phase2CombatPrioritySelector");
            Sequence phase3 = new Sequence("Phase3Sequence");
            PrioritySelector phase3Combat = new PrioritySelector("Phase3CombatPrioritySelector");

            phase1Combat.AddChild(new BossMonsterLeaf("NomalAttack1", new UseSkillStrategy(_skillManager, BossSkillType.NomalAttack1), 1));
            phase1Combat.AddChild(new BossMonsterLeaf("NomalAttack2", new UseSkillStrategy(_skillManager, BossSkillType.NomalAttack2), 1));
            phase1Combat.AddChild(new BossMonsterLeaf("ACombo", new UseComboStrategy(_comboManager, _comboManager.SelectSlot("ACombo")), 5));
            phase1Combat.AddChild(new BossMonsterLeaf("ACombo", new UseComboStrategy(_comboManager, _comboManager.SelectSlot("HCombo")), 5));
            phase1Combat.AddChild(new BossMonsterLeaf("ACombo", new UseComboStrategy(_comboManager, _comboManager.SelectSlot("DCombo")), 5));

            phase2Combat.AddChild(new BossMonsterLeaf("NomalAttack1", new UseSkillStrategy(_skillManager, BossSkillType.NomalAttack1), 1));
            phase2Combat.AddChild(new BossMonsterLeaf("NomalAttack2", new UseSkillStrategy(_skillManager, BossSkillType.NomalAttack2), 1));
            phase2Combat.AddChild(new BossMonsterLeaf("Earthquake", new UseSkillStrategy(_skillManager, BossSkillType.Earthquake), 3));
            phase2Combat.AddChild(new BossMonsterLeaf("ACombo", new UseComboStrategy(_comboManager, _comboManager.SelectSlot("BCombo")), 5));
            phase2Combat.AddChild(new BossMonsterLeaf("ACombo", new UseComboStrategy(_comboManager, _comboManager.SelectSlot("CCombo")), 5));
            phase2Combat.AddChild(new BossMonsterLeaf("ACombo", new UseComboStrategy(_comboManager, _comboManager.SelectSlot("ECombo")), 5));

            phase3Combat.AddChild(new BossMonsterLeaf("NomalAttack1", new UseSkillStrategy(_skillManager, BossSkillType.NomalAttack1), 1));
            phase3Combat.AddChild(new BossMonsterLeaf("Tekkai", new UseSkillStrategy(_skillManager, BossSkillType.Tekkai), 3));
            phase3Combat.AddChild(new BossMonsterLeaf("Summon", new UseSkillStrategy(_skillManager, BossSkillType.Summon), 3));
            phase3Combat.AddChild(new BossMonsterLeaf("ComboSelect", new UseComboStrategy(_comboManager, _comboManager.GetHighestPrioritySlot()), 5));

            phase3.AddChild(new BossMonsterLeaf("CheckAttackRange", new CheckAttackRange(Context), 5));
            phase3.AddChild(new BossMonsterLeaf("CheckPhase1", new CheckIsPhase3(_context), 5));
            phase3.AddChild(phase3Combat);

            phase2.AddChild(new BossMonsterLeaf("CheckAttackRange", new CheckAttackRange(Context), 5));
            phase2.AddChild(new BossMonsterLeaf("CheckPhase1", new CheckIsPhase2(_context), 5));
            phase2.AddChild(phase2Combat);

            phase1.AddChild(new BossMonsterLeaf("CheckAttackRange", new CheckAttackRange(Context), 5));
            phase1.AddChild(new BossMonsterLeaf("CheckPhase1", new CheckIsPhase1(_context), 5));
            phase1.AddChild(phase1Combat);

            combatSelector.AddChild(phase3);
            combatSelector.AddChild(phase2);
            combatSelector.AddChild(phase1);

            prioritySelector.AddChild(new BossMonsterLeaf("ReturnState", new ReturnStrategy(_blackBoard, _context, _inBattle, _startBattlePos, _monsterState), 6));
            prioritySelector.AddChild(combatSelector);
            prioritySelector.AddChild(new BossMonsterLeaf("ChaseState", new ChaseStrategy(_blackBoard, _context, _startBattlePos, _inBattle, _monsterState), 3));
            prioritySelector.AddChild(new BossMonsterLeaf("PatrolState", new PatrolStrategy(_blackBoard, _context, _actionTimer, _monsterState), 2));
            prioritySelector.AddChild(new BossMonsterLeaf("IdleState", new IdleStrategy(_blackBoard, _context, _inBattle, _actionTimer, _monsterState), 1));

            _BossMonsterBT.AddChild(prioritySelector);
        }

        private void NextActionTimers()
        {
            string state = _blackBoard.GetValue<string>(_monsterState);

            if (state == "Idle")
            {
                float timer = _blackBoard.GetValue<float>(_actionTimer);
                timer += Time.deltaTime;
                _blackBoard.SetValue(_actionTimer, timer);
            }
        }

        public void DeadContainer()
        {
            PoolManager.Instance.Get(_containerPrefab, transform.position, transform.rotation);
            PoolManager.Instance.Release(gameObject);
        }
    }
}