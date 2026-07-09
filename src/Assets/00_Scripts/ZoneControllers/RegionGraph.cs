using System;
using System.Collections.Generic;
using UnityEngine;
using HBDinosaur_ER_Project.Database;

namespace HBDinosaur_ER_Project.Database
{
    [CreateAssetMenu(fileName = "RegionGraphSO", menuName = "ZoneSystem/Region Graph")]
    public class RegionGraphSO : ScriptableObject
    {
        public List<RegionNodeData> nodes = new();
    }

    [Serializable]
    public class RegionNodeData
    {
        public Region region;
        public List<Region> adjacentRegions = new();
    }
}