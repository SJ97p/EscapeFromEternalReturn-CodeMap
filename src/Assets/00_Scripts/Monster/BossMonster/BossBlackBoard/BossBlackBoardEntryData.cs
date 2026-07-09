using System;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [Serializable]
    public class BossBlackBoardEntryData : ISerializationCallbackReceiver
    {
        public string keyName;
        public AnyValue.valueTpye valueType;
        public AnyValue value;

        public void SetValueOnBlackboard(BossBlackBoard blackboard)
        {
            var key = blackboard.GetorRegisterKey(keyName);
            setValueDispatchTable[value.type](blackboard, key, value);
        }

        static Dictionary<AnyValue.valueTpye, Action<BossBlackBoard, BossBlackBoardKey, AnyValue>> setValueDispatchTable = new()
        {
            { AnyValue.valueTpye.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue) },
            { AnyValue.valueTpye.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key, anyValue) },
            { AnyValue.valueTpye.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue) },
            { AnyValue.valueTpye.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key, anyValue) },
            { AnyValue.valueTpye.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key, anyValue) }
        };

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => value.type = valueType;
    }
}