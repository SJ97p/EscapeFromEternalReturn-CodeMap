using System.Collections.Generic;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.ItemData;

namespace HBDinosaur_ER_Project.Database
{
    public interface IRepository<T> : IReadOnlyRepository<T>, IWriteRepository<T>
    {
    }
}

