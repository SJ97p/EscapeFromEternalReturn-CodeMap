using HBDinosaur_ER_Project.ItemData;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.Crafting.UI
{
    public class CraftSelectView : MonoBehaviour
    {
        [Header("Item Info")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image gradeBackgroundImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Stats")]
        [SerializeField] private Transform statRoot;
        [SerializeField] private TMP_Text statTextPrefab;

        [Header("Actions")]
        [SerializeField] private Button craftButton;

        private ItemGradeColorPalette gradeColorPalette;
        private Action onCraftClicked;

        private void Awake()
        {
            if (craftButton != null)
                craftButton.onClick.AddListener(OnClick_Craft);
        }

        public void Initialize(Action onCraftClicked)
        {
            this.onCraftClicked = onCraftClicked;
        }

        public void SetData(CraftTreeNode node, ItemDataStruct? data)
        {
            gradeColorPalette = ItemDatabase.Instance != null
    ? ItemDatabase.Instance.colorPalette
    : null;

            if (node == null)
            {
                Clear();
                return;
            }

            if (!data.HasValue)
            {
                ShowUnknown(node.ItemId);
                return;
            }

            ItemDataStruct itemData = data.Value;

            if (nameText != null)
                nameText.text = itemData.itemName;

            if (descriptionText != null)
                descriptionText.text = itemData.description;

            if (iconImage != null)
            {
                iconImage.sprite = itemData.Icon;
                iconImage.enabled = itemData.Icon != null;
            }

            if (gradeBackgroundImage != null)
            {
                gradeBackgroundImage.color = gradeColorPalette != null
                    ? gradeColorPalette.GetColor(itemData.Grade)
                    : Color.white;
            }

            SetStats(itemData);
        }
        private void SetStats(ItemDataStruct itemData)
        {
            ClearStats();

            AddStatText("공격력", itemData.EquipmentData.Attack);
            AddStatText("방어력", itemData.EquipmentData.Defense);
            AddStatText("체력", itemData.EquipmentData.Hp);
            AddStatText("이동속도", itemData.EquipmentData.MoveSpeed);
        }
        private void AddStatText(string label, float value)
        {
            if (Mathf.Approximately(value, 0f))
                return;

            TMP_Text statText = Instantiate(statTextPrefab, statRoot);
            statText.text = $"{label} +{value}";
        }

        private void ClearStats()
        {
            if (statRoot == null)
                return;

            for (int i = statRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(statRoot.GetChild(i).gameObject);
            }
        }
        public void Clear()
        {
            if (nameText != null)
                nameText.text = string.Empty;

            if (descriptionText != null)
                descriptionText.text = string.Empty;


            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (gradeBackgroundImage != null)
                gradeBackgroundImage.color = Color.white;
        }

        private void ShowUnknown(int itemId)
        {
            if (nameText != null)
                nameText.text = $"Item {itemId}";

            if (descriptionText != null)
                descriptionText.text = "No item data.";

            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (gradeBackgroundImage != null)
                gradeBackgroundImage.color = Color.white;
        }

        private void OnClick_Craft()
        {
            onCraftClicked?.Invoke();
        }
    }
}
