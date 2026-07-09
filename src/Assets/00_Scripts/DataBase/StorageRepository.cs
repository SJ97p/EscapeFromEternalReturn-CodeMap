using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SQLite;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class StorageRepository : IRepository<StorageData>
    {
        private readonly DBLoader _dbLoader;
        private readonly string _dbKey = "playerstoragedb"; // 연결할 DB 키값

        // ❌기존: SQLiteConnection을 받던 것을 ⭕변경: DBLoader를 받도록 수정
        public StorageRepository(DBLoader dbLoader)
        {
            _dbLoader = dbLoader;
        }

        public IEnumerable<StorageData> GetAll()
        {
            var db = _dbLoader.GetConnection(_dbKey);
            if (db == null) return new List<StorageData>();

            var rows = db.Table<StorageItem>().ToList();

            List<StorageData> storage = new();
            foreach (var row in rows)
            {
                storage.Add(MapFromRow(row));
            }

            return storage;
        }

        public StorageData GetById(int id)
        {
            var db = _dbLoader.GetConnection(_dbKey);
            if (db == null) return null;

            var row = db.Table<StorageItem>().Where(x => x.ItemId == id).FirstOrDefault();
            return row == null ? null : MapFromRow(row);
        }

        public StorageData MapFromRow(StorageItem row)
        {
            if (row == null)
                return null;

            return new StorageData(
                row.StorageType,
                row.SaveId,
                row.ItemId,
                row.Quantity,
                row.X,
                row.Y);
        }

        public void Update(StorageData data) { }

        public void Delete(int data) { }

        public void DeleteAllBySaveId(int saveId)
        {
            var db = _dbLoader.GetConnection(_dbKey);
            if (db == null) return;

            var rows = db.Table<StorageItem>().Where(x => x.SaveId == saveId).ToList();
            foreach (var row in rows)
            {
                db.Delete(row);
            }
        }

        public void Add(StorageData data)
        {
            var db = _dbLoader.GetConnection(_dbKey);
            if (db == null) return;

            var row = new StorageItem
            {
                StorageType = (int)data.StorageType,
                SaveId = data.SaveId,
                ItemId = data.ItemId,
                Quantity = data.Quantity,
                X = data.X,
                Y = data.Y
            };
            db.Insert(row);
        }

        // ★ [추가] 특정 세이브 슬롯(SaveId)의 데이터만 가져오는 메서드
        public IEnumerable<StorageData> GetBySaveId(int saveId)
        {
            var db = _dbLoader.GetConnection(_dbKey);
            if (db == null) return new List<StorageData>();

            var rows = db.Table<StorageItem>().Where(x => x.SaveId == saveId).ToList();
            List<StorageData> storage = new();
            foreach (var row in rows)
            {
                storage.Add(MapFromRow(row));
            }
            return storage;
        }
    }
}