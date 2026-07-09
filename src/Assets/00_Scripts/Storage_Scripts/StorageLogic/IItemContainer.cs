using HBDinosaur_ER_Project.StorageSystem;

namespace HBDinosaur_ER_Project.UI.DragDrop
{
    public interface IItemContainer
    {
        ItemContainerType ContainerType { get; }

        int Width { get; }  // 추가
        int Height { get; } // 추가

        bool IsValidSlot(int x, int y);
        bool IsEmpty(int x, int y);

        int GetItemId(int x, int y);
        int GetAmount(int x, int y);

        bool SetSlot(int x, int y, int itemId, int amount);
        bool ClearSlot(int x, int y);

        bool CanDrop(UIDragPayload payload, int toX, int toY);
        bool HandleDrop(UIDragPayload payload, int toX, int toY);
        bool HandleClick(int x, int y);

        void RefreshUI();
    }
}