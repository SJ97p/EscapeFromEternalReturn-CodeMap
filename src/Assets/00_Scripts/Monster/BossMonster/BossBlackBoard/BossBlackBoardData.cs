using UnityEngine;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(fileName = "BossBlackBoardData", menuName = "MonsterBlackBoard/BossBlackBoardData")]
    public class BossBlackBoardData : ScriptableObject
    {
        public List<BossBlackBoardEntryData> entries = new();

        public void SetValuesOnBlackBoard(BossBlackBoard blackboard)
        {
            foreach (var entry in entries)
            {
                entry.SetValueOnBlackboard(blackboard);
            }
        }
    }
}