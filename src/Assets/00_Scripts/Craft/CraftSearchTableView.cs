using System;
using HBDinosaur_ER_Project.ItemData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftSearchITableView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Button button;

        private int itemId;
        private Action<int> onClicked;

        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(Click);
        }

        public void SetData(int itemId, ItemDataStruct? data, ItemGradeColorPalette gradeColorPalette, Action<int> onClicked)
        {
            this.itemId = itemId;
            this.onClicked = onClicked;

            if (data.HasValue)
            {
                ItemDataStruct itemData = data.Value;

                nameText.text = itemData.itemName;
                iconImage.sprite = itemData.Icon;
                iconImage.enabled = itemData.Icon != null;

                if (backgroundImage != null && gradeColorPalette != null)
                    backgroundImage.color = gradeColorPalette.GetColor(itemData.Grade);
            }
            else
            {
                nameText.text = $"{-1}";
                iconImage.sprite = null;
                iconImage.enabled = false;

                if (backgroundImage != null)
                    backgroundImage.color = Color.white;
            }
        }

        private void Click()
        {
            onClicked?.Invoke(itemId);
        }
    }
}

