using UnityEngine;

namespace HBDinosaur_ER_Project.UI
{
    public class StorageButton : MonoBehaviour
    {
        [SerializeField] private UIPanel panel;


        public void OnClicked_Storage()
        {
            if (panel.gameObject.activeSelf)
            {
                panel.Close();
            }
            else
            {
                panel.Open();
            }
        }
    }

}
