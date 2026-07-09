using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public interface IDataInjectable<T>
    {
        void InjectData(T storage);
    }
}