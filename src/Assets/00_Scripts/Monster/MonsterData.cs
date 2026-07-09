using HBDinosaur_ER_Project.Monster;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    [SerializeField] public string monsterName;
    [SerializeField] public GameObject _prefab;
    [SerializeField] public MonsterType _changeMonster;

    [Header("Base Stats")]
    [SerializeField] public float maxHp;
    [SerializeField] public float currentHp;
    [SerializeField] public float regenHp;
    [SerializeField] public float damage;
    [SerializeField] public float defense;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float attackRange;
    [SerializeField] public float chaseRange;
    [SerializeField] public float leashRange;

    [Header("AI")]
    [SerializeField] public float regenTime;
    [SerializeField] public float nextActionTime;
    [SerializeField] public float teleportTime;
    [SerializeField] public bool firstStrike;
    [SerializeField] public bool isPatrol;
    [SerializeField] public bool isTeleport;
    [SerializeField] public MonsterType monsterType;
    [SerializeField] public MonsterSubType monsterSubType;

    [Header("Drop")]
    [SerializeField] public float dropExperience;
    [SerializeField] public GameObject[] dropItem;
}
