using HBDinosaur_ER_Project.StorageSystem;

namespace HBDinosaur_ER_Project.UI.DragDrop
{
    public interface IQuickMoveService
    {
        bool TryQuickMove(ItemContainerType sourceType, int x, int y);
    }
}