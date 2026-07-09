using SingletonPattern_Scripts;
using System;

public class MonsterMutationManager : Singleton<MonsterMutationManager>
{
    public static bool IsMutationActive;

    public static Action<bool> OnMutationChanged;

    public static void SetMutation(bool active)
    {
        IsMutationActive = active;

        OnMutationChanged?.Invoke(active);
    }
}
