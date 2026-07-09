using SQLite;

namespace HBDinosaur_ER_Project.Database
{
    public class ItemDB
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string Grade { get; set; }
        public bool IsExtractable { get; set; }
        public bool IsSellable { get; set; }
        public bool isBaseFurniture { get; set; }
        public bool IsFavorite { get; set; }
        public float hp { get; set; }
        public float atkPhys { get; set; }
        public float atkMagic { get; set; }
        public float defPhys { get; set; }
        public float defMagic { get; set; }
        public float moveSpeed { get; set; }
        public float critRate { get; set; }
        public float critDmg { get; set; }
        public float hpRegen { get; set; }
        public int specialEffectID { get; set; }

    }
}
