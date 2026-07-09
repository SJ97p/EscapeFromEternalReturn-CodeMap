using SQLite;

namespace HBDinosaur_ER_Project.Database
{
    public class MonsterSpawnDataDB
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }        
        public int monster_type { get; set; }
        public int region_id { get; set; }
        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }
        public float rot_x { get; set; }
        public float rot_y { get; set; }
        public float rot_z { get; set; }
        public float scale_x { get; set; }
        public float scale_y { get; set; }
        public float scale_z { get; set; }

    }
}
