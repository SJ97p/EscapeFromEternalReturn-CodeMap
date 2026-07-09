namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public static class StringExtension
    {
        public static int ComputeFNV1aHash(this string s)
        {
            uint hash = 2166136261;
            foreach (char c in s)
            {
                hash = (hash ^ c) * 16777619;
            }
            return unchecked((int)hash);
        }
    }
}