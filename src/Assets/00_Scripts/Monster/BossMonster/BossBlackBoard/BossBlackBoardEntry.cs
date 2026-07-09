using System;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [Serializable]
    public class BossBlackBoardEntry<T>
    {
        public BossBlackBoardKey key { get; }
        public T value { get; }
        public Type valueTpye { get; }

        public BossBlackBoardEntry(BossBlackBoardKey key, T value)
        {
            this.key = key;
            this.value = value;
            valueTpye = typeof(T);
        }

        public override bool Equals(object obj) => obj is BossBlackBoardEntry<T> other && other.key == key;
        public override int GetHashCode() => key.GetHashCode();
    }
}