using System.Collections.Generic;
using UnityEngine;
using HBDinosaur_ER_Project.ItemData;

namespace HBDinosaur_ER_Project.Database
{
    [System.Serializable]
    public class UserSaveData
    {
        public int currentDay;
        public int currentTime;

        public Dictionary<EquipmentSlotType, int> equipments;

        public List<StorageData> storageSlots = new List<StorageData>();
    }
}
