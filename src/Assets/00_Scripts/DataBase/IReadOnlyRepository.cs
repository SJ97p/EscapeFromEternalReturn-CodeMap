using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Database
{
    public interface IReadOnlyRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
    }
}
