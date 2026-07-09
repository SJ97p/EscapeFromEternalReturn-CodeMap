using System;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [Serializable]
    public class BossBlackBoard
    {
        Dictionary<string, BossBlackBoardKey> KeyRegistry = new();
        Dictionary<BossBlackBoardKey, object> entries = new();
        
        public void Debug()
        {
            foreach(var entry in entries)
            {
                var entryType = entry.Value.GetType();

                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BossBlackBoardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    var value = valueProperty.GetValue(entry.Value);
                    UnityEngine.Debug.Log($"Key : {entry.Key}, Value : {value}");
                }
            }
        }

        public T GetValue<T>(BossBlackBoardKey key)
        {
            if (entries.TryGetValue(key, out var entry) && entry is BossBlackBoardEntry<T> castedEntry)
            {
                return castedEntry.value;
            }

            throw new Exception($"Key {key} not found or wrong type");
        }

        public bool TryGetValue<T>(BossBlackBoardKey key, out T value)
        {
            if(entries.TryGetValue(key, out var entry) && entry is BossBlackBoardEntry<T> castedEntry)
            {
                value = castedEntry.value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BossBlackBoardKey key, T value)
        {
            entries[key] = new BossBlackBoardEntry<T>(key, value);
        }

        public BossBlackBoardKey GetorRegisterKey(string keyname)
        {
            Preconditions.CheckNotNull(keyname);

            if(!KeyRegistry.TryGetValue(keyname, out BossBlackBoardKey key))
            {
                key = new BossBlackBoardKey(keyname);
                KeyRegistry[keyname] = key;
            }

            return key;
        }

        public bool ContainsKey(BossBlackBoardKey key) => entries.ContainsKey(key);

        public void Remove(BossBlackBoardKey key) => entries.Remove(key);
    }
}