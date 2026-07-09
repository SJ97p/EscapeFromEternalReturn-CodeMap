using System;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.UI
{
    [CreateAssetMenu(fileName = "UISceneProfile", menuName = "HBDinosaur/UI/Scene Profile")]
    public class UISceneProfile : ScriptableObject
    {
        [SerializeField] private string sceneName;
        [SerializeField] private List<UIPanelState> panelStates = new();

        public string SceneName => sceneName;
        public IReadOnlyList<UIPanelState> PanelStates => panelStates;
    }

    [Serializable]
    public struct UIPanelState
    {
        public UIPanelId PanelId;
        public bool Active;
    }
}