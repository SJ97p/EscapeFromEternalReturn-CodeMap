namespace HBDinosaur_ER_Project.Player
{
    public interface IPlayerState
    {
        void Enter();
        void Update();
        void Exit();
        bool IsOccupying { get; }
        int Priority { get; }
    }
}