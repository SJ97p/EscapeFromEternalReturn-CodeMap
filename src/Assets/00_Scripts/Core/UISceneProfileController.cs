using System.Collections.Generic;
using HBDinosaur_ER_Project.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HBDinosaur_ER_Project.UI
{
    public class UISceneProfileController : MonoBehaviour
    {
        [SerializeField] private List<UIPanel> panels = new();

        private Dictionary<UIPanelId, UIPanel> panelMap;
        private readonly HashSet<UIPanel> openedPanels = new();

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            //BuildUIPanelMap();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
        }
    }
}
