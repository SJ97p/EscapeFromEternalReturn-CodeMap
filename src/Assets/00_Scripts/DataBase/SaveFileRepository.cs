using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SQLite;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class SaveFileRepository
    {
        private readonly SQLiteConnection db;
        public SaveFileRepository(SQLiteConnection db)
        {
            this.db = db;
        }
    }
}

