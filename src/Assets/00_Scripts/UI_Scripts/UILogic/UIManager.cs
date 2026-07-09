using System.Collections.Generic;
using HBDinosaur_ER_Project.InventoryView;
using HBDinosaur_ER_Project.StorageSystem;
using SingletonPattern_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HBDinosaur_ER_Project.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private List<UIPanel> panels = new();

        private Dictionary<UIPanelId, UIPanel> panelMap;
        private readonly HashSet<UIPanel> openedPanels = new();

        public bool IsWorldInputBlocked => openedPanels.Count > 0;


        //리팩토링 대상
        [SerializeField] private List<UISceneProfile> sceneProfiles = new();

        [Header("Target Inventory")]
        [SerializeField] private UIPanel targetInventoryPanel;
        [SerializeField] private TargetInventoryHud targetInventoryHud;

        public Storage TargetInventory { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            BuildPanelMap();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplyProfile(scene.name);
        }

        private void BuildPanelMap()
        {
            panelMap = new Dictionary<UIPanelId, UIPanel>();

            for (int i = 0; i < panels.Count; i++)
            {
                UIPanel panel = panels[i];
                if (panel == null)
                    continue;

                panelMap[panel.PanelId] = panel;
            }
        }
        private void ApplyProfile(string sceneName)
        {
            UISceneProfile profile = FindProfile(sceneName);

            if (profile == null)
            {
                Debug.LogWarning($"[UIManager] No UI profile for scene: {sceneName}");
                return;
            }

            foreach (UIPanelState state in profile.PanelStates)
            {
                if (!panelMap.TryGetValue(state.PanelId, out UIPanel panel) || panel == null)
                    continue;

                if (state.Active)
                    panel.Open();
                else
                    panel.Close();
            }
        }
        private UISceneProfile FindProfile(string sceneName)
        {
            for (int i = 0; i < sceneProfiles.Count; i++)
            {
                if (sceneProfiles[i] != null && sceneProfiles[i].SceneName == sceneName)
                    return sceneProfiles[i];
            }

            return null;
        }

        public void RegisterOpened(UIPanel panel)
        {
            if (panel != null)
                openedPanels.Add(panel);
        }

        public void RegisterClosed(UIPanel panel)
        {
            if (panel != null)
                openedPanels.Remove(panel);
        }

        public void OpenStorage(Storage storage)
        {
            if (storage == null)
                return;

            TargetInventory = storage;

            if (targetInventoryPanel != null)
                targetInventoryPanel.Open();

            if (targetInventoryHud != null)
                targetInventoryHud.Bind(storage);
        }

        public void CloseTargetInventory()
        {
            TargetInventory = null;

            if (targetInventoryHud != null)
                targetInventoryHud.Unbind();

            if (targetInventoryPanel != null)
                targetInventoryPanel.Close();
        }
    }
}
