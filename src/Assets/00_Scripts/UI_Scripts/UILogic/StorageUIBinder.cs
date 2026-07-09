using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.StorageSystem;
using UnityEngine;
using System.Collections;

namespace HBDinosaur_ER_Project.UI
{
    public class StorageUIBinder : MonoBehaviour
    {
        [SerializeField] private EquipmentHud equipHud;
        [SerializeField] private PlayerInventoryHud inventoryHud;
        [SerializeField] private StoragePanelUI storagePanelUI;
    }
}
