using HBDinosaur_ER_Project;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public interface IWriteRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
