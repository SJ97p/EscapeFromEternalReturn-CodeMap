using HBDinosaur_ER_Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.UI
{
    public class UIPanelButton : MonoBehaviour
    {
        private enum UIActionType
        {
            Open,
            Close,
            Toggle
        }

        [SerializeField] private Button button;
        [SerializeField] private UIPanelId targetPanelId;
        [SerializeField] private UIActionType actionType;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            switch (actionType)
            {
                case UIActionType.Open:
                    NewUIManager.Instance.Open(targetPanelId);
                    break;

                case UIActionType.Close:
                    NewUIManager.Instance.Close(targetPanelId);
                    break;

                case UIActionType.Toggle:
                    NewUIManager.Instance.Toggle(targetPanelId);
                    break;
            }
        }
    }
}