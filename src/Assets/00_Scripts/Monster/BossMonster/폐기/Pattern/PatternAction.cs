using UnityEngine;

namespace HBDinosaur_ER_Project.Monster
{
    public abstract class PatternAction : ScriptableObject
    {
        public abstract void Execute(RuntimePatternData data);
    }
}