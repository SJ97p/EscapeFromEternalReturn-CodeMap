using System;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public static class InputEventBus
    {
        public static event Action OnStop;
        public static void RaiseStop()
        {
            OnStop?.Invoke();
        }

        public static event Action<Vector2, InputType> OnMousePressed;
        public static void RaiseMousePressed(Vector2 mousePos, InputType type)
        {
            OnMousePressed?.Invoke(mousePos, type);
        }
        public static event Action<Vector2, InputType> OnMouseReleased;
        public static void RaiseMouseReleased(Vector2 mousePos, InputType type)
        {
            OnMouseReleased?.Invoke(mousePos, type);
        }
        public static event Action<Vector2> OnMouseMovement;
        public static void RaiseMouseMove(Vector2 mousePos)
        {
            OnMouseMovement?.Invoke(mousePos);
        }
        public static event Action<int, Vector2> OnSkillPressed;
        public static void RaiseSkillPressed(int skillNum, Vector2 mousePos)
        {
            OnSkillPressed?.Invoke(skillNum, mousePos);
        }
        public static event Action<int, Vector2> OnSkillReleased;
        public static void RaiseSkillRelease(int skillNum, Vector2 mousePos)
        {
            OnSkillReleased?.Invoke(skillNum, mousePos);
        }

        public static event Action OnArrived;
        public static void RaiseArrived()
        {
            OnArrived?.Invoke();
        }
    }
}