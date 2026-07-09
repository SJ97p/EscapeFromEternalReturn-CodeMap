namespace HBDinosaur_ER_Project.Monster.BossMonster
{ 
    public interface IStrategy
    {
        BossMonsterNode.Status Process();
        void Reset()
        {

        }
    }
}