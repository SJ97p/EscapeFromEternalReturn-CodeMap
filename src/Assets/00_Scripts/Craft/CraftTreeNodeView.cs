using System;
using HBDinosaur_ER_Project.Crafting;
using HBDinosaur_ER_Project.ItemData;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.Crafting.UI
{
    public class CraftTreeNodeView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text itemIdText;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text countText;

        private CraftTreeNode node;
        private Action<CraftTreeNode> onClicked;

        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();

            if (button != null)
                button.onClick.AddListener(OnClick_TryCraft);
        }

        public void SetData(CraftTreeNode node,ItemDataStruct? data,ItemGradeColorPalette gradeColorPalette, Action<CraftTreeNode> onClicked)
        {
            this.node = node;
            this.onClicked = onClicked;

            if (data.HasValue)
            {
                ItemDataStruct itemData = data.Value;

                itemIdText.text = itemData.itemName;
                iconImage.sprite = itemData.Icon;
                iconImage.enabled = itemData.Icon != null;

                if (backgroundImage != null && gradeColorPalette != null)
                    backgroundImage.color = gradeColorPalette.GetColor(itemData.Grade);
            }
            else
            {
                itemIdText.text = $"Item {node.ItemId}";
                iconImage.sprite = null;
                iconImage.enabled = false;

                if (backgroundImage != null)
                    backgroundImage.color = Color.white;
            }
        }
        public void SetCountText(int ownedCount, int requiredCount)
        {
            if (countText == null) return;

            // 재료가 충분하면 흰색(또는 초록색), 부족하면 빨간색
            string colorHex = (ownedCount >= requiredCount) ? "#FFFFFF" : "#FF5555";

            countText.text = $"<color={colorHex}>{ownedCount}</color>/{requiredCount}";
        }

        public void OnClick_TryCraft()
        {
            if (node == null)
                return;

            onClicked?.Invoke(node);
        }
    }
}
