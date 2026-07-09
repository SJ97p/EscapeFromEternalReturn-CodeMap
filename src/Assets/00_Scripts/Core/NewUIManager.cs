using System.Collections.Generic;
using HBDinosaur_ER_Project.StorageSystem;
using HBDinosaur_ER_Project.UI;
using SingletonPattern_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HBDinosaur_ER_Project.Core
{
    public class NewUIManager : Singleton<NewUIManager>
    {
        private Dictionary<UIPanelId, UIPanel> panelMap = new();
        private readonly HashSet<UIPanel> openedPanels = new();

        protected override void Awake()
        {
            base.Awake();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            RebuildPanelMap();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RebuildPanelMap();
        }

        private void RebuildPanelMap()
        {
            panelMap.Clear();

            UIPanel[] panels = Resources.FindObjectsOfTypeAll<UIPanel>();

            foreach (UIPanel panel in panels)
            {
                if (panel == null)
                    continue;

                if (!panel.gameObject.scene.IsValid())
                    continue;

                UIPanelId id = panel.PanelId;

                if (panelMap.ContainsKey(id))
                {
                    Debug.LogWarning($"[UIManager] Duplicate UIPanelId detected : {id}");
                    continue;
                }

                panelMap.Add(id, panel);

                Debug.Log($"[UIManager] Registered Panel : {id}");
            }
        }

        public void Open(UIPanelId id)
        {
            if (panelMap.TryGetValue(id, out UIPanel panel))
            {
                panel.Open();
            }
        }

        public void Close(UIPanelId id)
        {
            if (panelMap.TryGetValue(id, out UIPanel panel))
            {
                panel.Close();
            }
        }

        public void Toggle(UIPanelId id)
        {
            if (panelMap.TryGetValue(id, out UIPanel panel))
            {
                panel.Toggle();
            }
        }

        public T Get<T>(UIPanelId id) where T : UIPanel
        {
            if (panelMap.TryGetValue(id, out UIPanel panel))
            {
                return panel as T;
            }

            return null;
        }

        public bool IsOpened(UIPanelId id)
        {
            if (panelMap.TryGetValue(id, out UIPanel panel))
            {
                return panel.IsOpen;
            }

            return false;
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
        public void Open<TData>(UIPanelId id, TData data)
        {
            if (panelMap.TryGetValue(id, out UIPanel panel))
            {
                // 1. 패널을 먼저 엽니다 (GameObject가 Active 상태가 됨)
                panel.Open();

                // 2. 그 다음 데이터를 주입합니다 (이제 코루틴이 정상 작동함)
                if (panel is IDataInjectable<TData> injectable)
                {
                    injectable.InjectData(data);
                }
            }
        }
        public bool IsWorldInputBlocked => openedPanels.Count > 0;
    }
}