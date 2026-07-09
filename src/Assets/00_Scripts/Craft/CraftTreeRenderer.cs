using System;
using HBDinosaur_ER_Project.ItemData;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting.UI
{
    public class CraftTreeRenderer : MonoBehaviour
    {
        [Header("Containers")]
        [SerializeField] private RectTransform rootContainer;
        [SerializeField] private RectTransform lineContainer;

        [Header("Prefabs")]
        [SerializeField] private CraftTreeNodeView nodePrefab;
        [SerializeField] private RectTransform linePrefab;

        [Header("Tree Layout")]
        [SerializeField] private float rootWidth = 1400f;
        [SerializeField] private float topPadding = 80f;
        [SerializeField] private float startVerticalSpacing = 220f;
        [SerializeField] private float verticalSpacingFalloff = 0.8f;
        [SerializeField] private float minVerticalSpacing = 90f;

        [Header("Branch Line")]
        [SerializeField] private float lineThickness = 4f;
        [SerializeField] private float branchOffsetFromParent = 35f;
        [SerializeField] private float childConnectOffset = 25f;

        private Action<CraftTreeNode> onNodeClicked;

        public void Render(CraftTreeNode rootNode, CraftingService craftingService, Action<CraftTreeNode> onNodeClicked = null)
        {
            this.onNodeClicked = onNodeClicked;

            Clear();

            if (rootNode == null)
                return;

            float leftBound = -rootWidth * 0.5f;
            float rightBound = rootWidth * 0.5f;

            CreateNodeRecursive(rootNode, craftingService, leftBound, rightBound, -topPadding, 0);
        }

        private void Clear()
        {
            if (rootContainer != null)
            {
                for (int i = rootContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(rootContainer.GetChild(i).gameObject);
                }
            }

            if (lineContainer != null)
            {
                for (int i = lineContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(lineContainer.GetChild(i).gameObject);
                }
            }
        }

        private CraftTreeNodeView CreateNodeRecursive(
            CraftTreeNode node,
            CraftingService craftingService,
            float leftBound,
            float rightBound,
            float y,
            int depth)
        {
            if (node == null)
                return null;

            float centerX = (leftBound + rightBound) * 0.5f;

            CraftTreeNodeView nodeView = Instantiate(nodePrefab, rootContainer);
            ItemDataStruct? data = ItemDatabase.Instance.GetItemByID(node.ItemId);
            var palatte = ItemDatabase.Instance.colorPalette;
            nodeView.SetData(node, data, palatte, onNodeClicked);
            nodeView.RectTransform.anchoredPosition = new Vector2(centerX, y);

            if (craftingService != null)
            {
                // 서비스의 GetTotalItemCount를 호출하여 총 보유량을 가져옵니다.
                int ownedCount = craftingService.GetTotalItemCount(node.ItemId);
                int requiredCount = node.NeedAmount;

                nodeView.SetCountText(ownedCount, requiredCount);
            }

            int leftLeafCount = GetLeafCount(node.Left);
            int rightLeafCount = GetLeafCount(node.Right);
            int totalLeafCount = leftLeafCount + rightLeafCount;

            if (totalLeafCount == 0)
                return nodeView;

            float nextY = y - GetVerticalSpacing(depth);
            float leftWidthRatio = totalLeafCount > 0 ? (float)leftLeafCount / totalLeafCount : 0.5f;
            float splitX = Mathf.Lerp(leftBound, rightBound, leftWidthRatio);

            CraftTreeNodeView leftView = null;
            CraftTreeNodeView rightView = null;

            if (node.Left != null)
            {
                leftView = CreateNodeRecursive(
                    node.Left,
                    craftingService,
                    leftBound,
                    splitX,
                    nextY,
                    depth + 1);
            }

            if (node.Right != null)
            {
                rightView = CreateNodeRecursive(
                    node.Right,
                    craftingService,
                    splitX,
                    rightBound,
                    nextY,
                    depth + 1);
            }

            CreateBranch(nodeView.RectTransform, leftView ? leftView.RectTransform : null, rightView ? rightView.RectTransform : null);

            return nodeView;
        }

        private void CreateBranch(RectTransform parent, RectTransform leftChild, RectTransform rightChild)
        {
            if (parent == null)
                return;

            if (leftChild == null && rightChild == null)
                return;

            if (leftChild != null && rightChild != null)
            {
                CreateDoubleChildBranch(parent, leftChild, rightChild);
                return;
            }

            RectTransform singleChild = leftChild != null ? leftChild : rightChild;
            CreateSingleChildBranch(parent, singleChild);
        }

        private void CreateDoubleChildBranch(RectTransform parent, RectTransform leftChild, RectTransform rightChild)
        {
            Vector2 parentBottom = GetBottomCenter(parent);

            Vector2 leftAttach = GetRightCenter(leftChild);   // 왼쪽 자식 오른쪽 면
            Vector2 rightAttach = GetLeftCenter(rightChild);  // 오른쪽 자식 왼쪽 면

            // 자식들이 놓인 높이 기준으로 bus 생성
            float branchY = leftAttach.y;

            // 부모에서 아래로 내려오는 수직선
            CreateVerticalLine(
                parentBottom,
                new Vector2(parentBottom.x, branchY));

            // 왼쪽 자식 -> 부모 stem x 까지 수평선
            CreateHorizontalLine(
                leftAttach,
                new Vector2(parentBottom.x, branchY));

            // 부모 stem x -> 오른쪽 자식 까지 수평선
            CreateHorizontalLine(
                new Vector2(parentBottom.x, branchY),
                rightAttach);
        }

        private void CreateSingleChildBranch(RectTransform parent, RectTransform child)
        {
            if (child == null)
                return;

            Vector2 parentBottom = GetBottomCenter(parent);
            Vector2 childTop = GetTopCenter(child);

            CreateVerticalLine(
                parentBottom,
                new Vector2(parentBottom.x, childTop.y + childConnectOffset));

            CreateHorizontalLine(
                new Vector2(parentBottom.x, childTop.y + childConnectOffset),
                new Vector2(childTop.x, childTop.y + childConnectOffset));

            CreateVerticalLine(
                new Vector2(childTop.x, childTop.y + childConnectOffset),
                childTop);
        }

        private void CreateHorizontalLine(Vector2 start, Vector2 end)
        {
            RectTransform line = Instantiate(linePrefab, lineContainer);

            float width = Mathf.Abs(end.x - start.x);
            float centerX = (start.x + end.x) * 0.5f;
            float centerY = start.y;

            line.anchoredPosition = new Vector2(centerX, centerY);
            line.sizeDelta = new Vector2(width, lineThickness);
            line.localRotation = Quaternion.identity;
            line.localScale = Vector3.one;
        }

        private void CreateVerticalLine(Vector2 start, Vector2 end)
        {
            RectTransform line = Instantiate(linePrefab, lineContainer);

            float height = Mathf.Abs(end.y - start.y);
            float centerX = start.x;
            float centerY = (start.y + end.y) * 0.5f;

            line.anchoredPosition = new Vector2(centerX, centerY);
            line.sizeDelta = new Vector2(lineThickness, height);
            line.localRotation = Quaternion.identity;
            line.localScale = Vector3.one;
        }

        private Vector2 GetTopCenter(RectTransform rect)
        {
            Vector2 pos = rect.anchoredPosition;
            float topY = pos.y - (rect.rect.height * (1f - rect.pivot.y));
            return new Vector2(pos.x, topY);
        }

        private Vector2 GetBottomCenter(RectTransform rect)
        {
            Vector2 pos = rect.anchoredPosition;
            float bottomY = pos.y - (rect.rect.height * rect.pivot.y);
            return new Vector2(pos.x, bottomY);
        }

        private int GetLeafCount(CraftTreeNode node)
        {
            if (node == null)
                return 0;

            if (node.Left == null && node.Right == null)
                return 1;

            return GetLeafCount(node.Left) + GetLeafCount(node.Right);
        }

        private float GetVerticalSpacing(int depth)
        {
            float spacing = startVerticalSpacing * Mathf.Pow(verticalSpacingFalloff, depth);
            return Mathf.Max(minVerticalSpacing, spacing);
        }
        private Vector2 GetLeftCenter(RectTransform rect)
        {
            Vector2 pos = rect.anchoredPosition;
            float x = pos.x - (rect.rect.width * rect.pivot.x);
            return new Vector2(x, pos.y);
        }

        private Vector2 GetRightCenter(RectTransform rect)
        {
            Vector2 pos = rect.anchoredPosition;
            float x = pos.x + (rect.rect.width * (1f - rect.pivot.x));
            return new Vector2(x, pos.y);
        }
    }
}