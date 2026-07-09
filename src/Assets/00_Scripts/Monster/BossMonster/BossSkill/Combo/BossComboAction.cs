using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public abstract class BossCombatAction : ScriptableObject
    {

    }

    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/Action/BossComboAction")]
    [System.Serializable]
    public class BossComboAction : BossCombatAction
    {
        public List<BossCombatAction> actions;
    }
}