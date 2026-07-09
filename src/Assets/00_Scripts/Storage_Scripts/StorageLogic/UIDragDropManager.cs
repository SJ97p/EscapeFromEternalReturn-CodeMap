using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI.DragDrop;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.UI
{
    public class UIDragDropManager : MonoBehaviour
    {
        [SerializeField] private DragFollowIcon dragFollowIcon;

        public bool IsDragging { get; private set; }
        public UIDragPayload CurrentPayload { get; private set; }
        public IItemContainer SourceContainer { get; private set; }

        public void BeginDrag(IItemContainer sourceContainer, UIDragPayload payload, Sprite icon, Vector2 screenPosition)
        {
            IsDragging = true;
            SourceContainer = sourceContainer;
            CurrentPayload = payload;

            if (dragFollowIcon != null && icon != null)
                dragFollowIcon.Show(icon, screenPosition);
        }

        public void UpdateDrag(Vector2 screenPosition)
        {
            if (!IsDragging) return;
            if (dragFollowIcon != null)
                dragFollowIcon.Move(screenPosition);
        }

        public void EndDrag()
        {
            IsDragging = false;
            SourceContainer = null;
            CurrentPayload = default;

            if (dragFollowIcon != null)
                dragFollowIcon.Hide();
        }
    }
}

