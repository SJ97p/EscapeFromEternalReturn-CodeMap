using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.HyperloopSystem
{
    public class HyperloopOutline : MonoBehaviour, IClickable
    {
        private Outline outline;
        private void Awake()
        {
            outline = GetComponent<Outline>();
        }
        public void Clicked(InputType inputType)
        {
            if (TryGetComponent(out Hyperloop hyperloop))
            {
                hyperloop.Interact();
            }

        }

        public void Hovered()
        {
            outline.enabled = true;
        }

        public void UnHoverd()
        {
            outline.enabled = false;
        }
    }
}