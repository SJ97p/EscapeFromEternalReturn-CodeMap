using HBDinosaur_ER_Project.Player.Skill;
using HBDinosaur_ER_Project.itemData;
using UnityEngine;
using System.Collections;
using HBDinosaur_ER_Project.Common;
using System;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerStat : MonoBehaviour
    {
        public static PlayerStat Instance { get; private set; }

        [SerializeField] private PlayerStats stats;
        private BuffManager buffManager;

        private float baseDefense;
        private float bonusDefense;
        private float maxHp;
        private float hp;
        private float attackDamage;
        private float attackSpeed;
        private float attackRange;
        private float _attackMultiplier;
        private float _skillDamage;
        private float _regenHp;
        private float moveSpeed;
        private bool isCCImmune;
        private float maxZoneTimer = 30;
        private float zoneTimer;


        #region 프로퍼티
        public float ZoneTimer
        {
            get => zoneTimer;
            set
            {
                zoneTimer = value;
            }
        }
        public float SkillDamage
        {
            get => _skillDamage;
            set
            {
                _skillDamage = value;
            }
        }

        public float AttackMultiplier
        {
            get => _attackMultiplier;
            set
            {
                _attackMultiplier = Mathf.Max(value, 1);
            }
        }

        public bool IsCCImmune
        {
            get => isCCImmune;
            set
            {
                isCCImmune = value;
            }
        }
        public float HP
        {
            get => hp;
            set
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                StateEventBus.RaiseHpChanged(hp);
            }
        }
        public float MaxHP
        {
            get => maxHp;
            set
            {
                maxHp = value;
                StateEventBus.RaiseMaxHpChanged(maxHp);
            }
        }
        public float AttackDamage
        {
            get => attackDamage;
            set
            {
                attackDamage = value;
                StateEventBus.RaiseAttackDamageChanged(attackDamage);
            }
        }

        public float AttackSpeed
        {
            get => attackSpeed;
            set
            {
                attackSpeed = value;
                StateEventBus.RaiseAttackSpeedChanged(attackSpeed);
            }
        }
        public float AttackRange
        {
            get => attackRange;
            set
            {
                attackRange = value;
            }
        }
        public float MoveSpeed
        {
            get => moveSpeed;
            set
            {
                moveSpeed = value;
                StateEventBus.RaiseMoveSpeedChanged(moveSpeed);
                var playerMove = GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.UpdateSpeed();
                }
            }
        }
        public float Defense => baseDefense + bonusDefense;
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 플레이어 자체가 중복이면 gameObject 제거
                return;
            }

            Instance = this;

            Initialize();
            buffManager = GetComponent<BuffManager>();
        }

        private void Start()
        {
            StartCoroutine(RegenHP());
        }

        private IEnumerator RegenHP()
        {
            while (HP > 0)
            {
                HP += _regenHp;
                yield return new WaitForSeconds(1f);
            }
        }

        private void Initialize()
        {
            MaxHP = stats.MaxHP;
            HP = maxHp;
            baseDefense = stats.Defense;
            StateEventBus.RaiseDefenseChanged(Defense);   // 초기값 UI 반영
            AttackDamage = stats.AttackDamage;
            AttackSpeed = stats.AttackSpeed;
            attackRange = stats.AttackRange;
            _attackMultiplier = stats.AttackMultiplier;
            _skillDamage = stats.SkillDamage;
            _regenHp = stats.RegenerationHP;
            MoveSpeed = stats.MoveSpeed;
            ZoneTimer = maxZoneTimer;
        }


        public void AddDefense(float value)
        {
            bonusDefense += value;
            StateEventBus.RaiseDefenseChanged(Defense);
        }

        public void RemoveDefense(float value)
        {
            bonusDefense -= value;
            StateEventBus.RaiseDefenseChanged(Defense);
        }

        public void ResetTimer()
        {
            ZoneTimer = maxZoneTimer;
        }

        public void ApplyEquipmentStats(EquipmentItemData equipData)
        {
            SFXManager.Instance.PlaySFX(Sound.SFXType.Equipmentinstall_epic);
            AttackDamage += equipData.Attack;

            AddDefense(equipData.Defense);
            MoveSpeed += equipData.MoveSpeed;
            MaxHP += equipData.Hp;
            HP += equipData.Hp;
        }

        public void RemoveEquipmentStats(EquipmentItemData equipData)
        {
            AttackDamage -= equipData.Attack;
            RemoveDefense(equipData.Defense);
            MoveSpeed -= equipData.MoveSpeed;
            MaxHP -= equipData.Hp;
            HP = hp;
        }

        public SkillContext BuildContext(int skillNum, Vector3 targetDirection)
        {
            return new SkillContext
            {
                Caster = transform,
                Direction = targetDirection - transform.position,
                LookPosition = targetDirection,
                BuffManager = buffManager,
                TargetLayer = LayerMask.GetMask("Monster"),
                SkillNum = skillNum,
                Power = AttackDamage,
                SkillAmplification = SkillDamage,
                AttackMultiplier = AttackMultiplier,
            };
        }
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}