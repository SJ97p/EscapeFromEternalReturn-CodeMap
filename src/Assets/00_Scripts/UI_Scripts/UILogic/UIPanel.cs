using HBDinosaur_ER_Project.Core;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.UI
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private UIPanelId panelId;
        //[SerializeField] private bool blocksWorldInput = true;

        //public bool BlocksWorldInput => blocksWorldInput;
        public UIPanelId PanelId => panelId;
        public bool IsOpen => gameObject.activeSelf;

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public void Toggle()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }

        private void OnEnable()
        {
            if (Singleton<NewUIManager>.IsQuitting)
                return;

            NewUIManager.Instance?.RegisterOpened(this);
        }

        private void OnDisable()
        {
            if (Singleton<NewUIManager>.IsQuitting)
                return;

            NewUIManager.Instance?.RegisterClosed(this);
        }
        public virtual void RefreshAll()
        {
            // 기본 동작이 필요 없다면 비워두거나 기본 로그 작성
        }
    }
}
