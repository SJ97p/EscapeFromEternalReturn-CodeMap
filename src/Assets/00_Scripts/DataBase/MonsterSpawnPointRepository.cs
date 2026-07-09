using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SQLite;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class MonsterSpawnPointRepository : IReadOnlyRepository<MonsterSpawnPointData>
    {
        private readonly SQLiteConnection db;

        public MonsterSpawnPointRepository(SQLiteConnection db)
        {
            this.db = db;
        }

        public IEnumerable<MonsterSpawnPointData> GetAll()
        {
            var rows = db.Table<MonsterSpawnDataDB>().ToList();

            List<MonsterSpawnPointData> items = new();
            foreach (var row in rows)
            {
                items.Add(MapFromRow(row));
            }

            return items;
        }

        public MonsterSpawnPointData GetById(int id)
        {
            var row = db.Table<MonsterSpawnDataDB>().Where(x => x.id == id).FirstOrDefault();
            return row == null ? null : MapFromRow(row);
        }

        public MonsterSpawnPointData MapFromRow(MonsterSpawnDataDB row)
        {
            if (row == null)
                return null;

            return new MonsterSpawnPointData(
                row.id, 
                row.monster_type,
                row.region_id,
                row.pos_x, row.pos_y, row.pos_z, 
                row.rot_x, row.rot_y, row.rot_z, 
                row.scale_x, row.scale_y, row.scale_z);
        }
    }
}

