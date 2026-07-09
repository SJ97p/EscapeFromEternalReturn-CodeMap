using UnityEngine;

namespace FogWar
{
    public class FogCell
    {
        public FogState State;

        public Vector3 WorldPosition;

        public float CurrentAlpha;
        public float TargetAlpha;

        public bool IsVisible;
    }
}