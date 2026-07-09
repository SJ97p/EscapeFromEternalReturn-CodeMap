using SQLite;

namespace HBDinosaur_ER_Project.Database
{
    public class StorageItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int SaveId { get; set; }

        public int StorageType { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

    }
}
