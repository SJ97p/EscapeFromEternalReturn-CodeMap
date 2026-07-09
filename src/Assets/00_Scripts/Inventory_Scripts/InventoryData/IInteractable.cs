using UnityEngine;
using System.Collections;

namespace HBDinosaur_ER_Project.InventoryData
{
    public interface IInteractable
    {
        // A 001. 나는 상호작용할 수 있는가? 를 InteractContext를 받아 판단
        bool CanInteract(InteractContext context);
        // A 002. 상호작용하면 무슨 일이 일어나는가? 를 InteractContext를 받아 판단
        void Interact(InteractContext context);
    }
}