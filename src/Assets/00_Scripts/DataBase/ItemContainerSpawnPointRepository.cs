using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SQLite;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class ItemContainerSpawnPointRepository : IReadOnlyRepository<ItemContainerSpawnPointData>
    {
        private readonly SQLiteConnection db;

        public ItemContainerSpawnPointRepository(SQLiteConnection db)
        {
            this.db = db;
        }

        public IEnumerable<ItemContainerSpawnPointData> GetAll()
        {
            var rows = db.Table<ItemContainerSpawnDataDB>().ToList();

            List<ItemContainerSpawnPointData> items = new();
            foreach (var row in rows)
            {
                items.Add(MapFromRow(row));
            }

            return items;
        }

        public ItemContainerSpawnPointData GetById(int id)
        {
            var row = db.Table<ItemContainerSpawnDataDB>().Where(x => x.id == id).FirstOrDefault();
            return row == null ? null : MapFromRow(row);
        }

        public ItemContainerSpawnPointData MapFromRow(ItemContainerSpawnDataDB row)
        {
            if (row == null)
                return null;

            return new ItemContainerSpawnPointData(
                row.id,
                row.container_type,
                row.region_id,
                row.pos_x, row.pos_y, row.pos_z,
                row.rot_x, row.rot_y, row.rot_z,
                row.scale_x, row.scale_y, row.scale_z);
        }
    }
}

