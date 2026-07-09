namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public static class UnityObjectExtension
    {
        public static T OrNull<T>(this T obj) where T : UnityEngine.Object
        {
            return obj == null ? null : obj;
        }
    }
}