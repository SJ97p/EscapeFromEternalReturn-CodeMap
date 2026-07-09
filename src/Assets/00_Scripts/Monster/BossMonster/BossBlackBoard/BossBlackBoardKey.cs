using System;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [Serializable]
    public readonly struct BossBlackBoardKey : IEquatable<BossBlackBoardKey>
    {
        readonly string name;
        readonly int hashedKey;

        public BossBlackBoardKey(string n)
        {
            name = n;
            hashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BossBlackBoardKey other) => hashedKey == other.hashedKey;

        public override bool Equals(object obj) => obj is BossBlackBoardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;
        public override string ToString() => name;

        public static bool operator ==(BossBlackBoardKey lhs, BossBlackBoardKey rhs) => lhs.hashedKey == rhs.hashedKey;
        public static bool operator !=(BossBlackBoardKey lhs, BossBlackBoardKey rhs) => !(lhs == rhs);
    }
}