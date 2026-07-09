using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.SceneSystem;
using HBDinosaur_ER_Project.UI;
using UnityEngine;

namespace HBDinosaur_ER_Project.HyperloopSystem
{
    public class Hyperloop : MonoBehaviour
    {
        [SerializeField] private Region region;
        public Region Region => region;
        public void Interact()
        {
            if (InGameSceneController.Instance.IsEscapable(region))
            {
                NewUIManager.Instance?.Open(UIPanelId.LobbyMenu);
            }

            NewUIManager.Instance?.Open(UIPanelId.HyperloopUI);
        }
    }
}