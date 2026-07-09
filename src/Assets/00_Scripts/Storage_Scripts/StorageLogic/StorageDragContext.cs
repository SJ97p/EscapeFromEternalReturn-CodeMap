using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class DragContext : MonoBehaviour
    {
        public static bool IsDragging { get; private set; }
        public static int FromX { get; private set; }
        public static int FromY { get; private set; }
        public static Sprite DragIcon { get; private set; }

        public static void Begin(int fromX, int fromY, Sprite icon)
        {
            IsDragging = true;
            FromX = fromX;
            FromY = fromY;
            DragIcon = icon;
        }

        public static void Clear()
        {
            IsDragging = false;
            FromX = -1;
            FromY = -1;
            DragIcon = null;
        }
    }

}
