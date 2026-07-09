using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster
{
    public class PatternData : ScriptableObject
    {
        public PatternID id;
        public List<PatternGroup> groups;

        public float basedamage;
        public float basecooldown;
        public float baserange;

        public List<PhaseModifiers> paseModefifiers;
    }
}