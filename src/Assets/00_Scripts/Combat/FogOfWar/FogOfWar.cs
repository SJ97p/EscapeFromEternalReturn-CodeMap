using System.Collections.Generic;
using UnityEngine;


namespace FogWar
{
    public enum FogState
    {
        Visible,
        Invisible,
        Hidden,
    }

    public class FogOfWar : MonoBehaviour
    {
        [System.Serializable]
        public class VisionSource
        {
            public GameObject Revealer;
            public float VisionRadius;
            public bool UpdateOnlyOnMove;

            [System.NonSerialized] public Vector3 LastPosition;
            [System.NonSerialized] public bool IsInitialized;
            [System.NonSerialized] public List<Vector2Int> CachedVisibleCells = new List<Vector2Int>();
        }
        [SerializeField] private List<VisionSource> visionSources;
        [SerializeField] private float fogFadeSpeed = 5f;
        [SerializeField] private Transform mapCenter;

        [System.Serializable]
        public class FogAlpha
        {
            [Range(0, 1)] public float visible = 0f;
            [Range(0, 1)] public float invisible = 0.5f;
            [Range(0, 1)] public float hidden = 1.0f;
        }
        [SerializeField] private FogAlpha fogAlpha = new FogAlpha();

        [SerializeField] private Color fogColor = new Color32(5, 15, 25, 255);
        [SerializeField] private float visionUpdateInterval = 0.1f;
        private float lastVisionUpdateTime = -999f;

        [SerializeField] private float fogPlaneHeight = 0.5f;
        [SerializeField] private Material fogMaterial;
        [SerializeField] private LayerMask hideLayer;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask groundLayer;

        [SerializeField][Range(1, 512)] private int levelDimensionX = 11;
        [SerializeField][Range(1, 512)] private int levelDimensionY = 11;
        [SerializeField] private float unitScale = 1;
        [SerializeField] private bool isAOS = true;

        private Texture2D fogPlaneTexture;
        private Texture2D fogPlaneTextureBuffer;
        private GameObject fogPlane;
        private FogCell[,] cells;
        private Color[] fogPixels;

        private List<Vector2Int> lastVisibleCells = new List<Vector2Int>();
        private List<Vector2Int> currentVisibleCells = new List<Vector2Int>();
        private HashSet<Vector2Int> currentVisibleSet = new HashSet<Vector2Int>();
        private List<Vector2Int> fadingCells = new List<Vector2Int>();
        private bool[,] isFading;
        private Collider[] overlapBuffer = new Collider[512];

        public void AddVisionSource(VisionSource vision)
        {
            visionSources.Add(vision);
        }

        public void RemoveVisionSource(int index)
        {
            if (index >= visionSources.Count || index < 0) return;
            visionSources.RemoveAt(index);
        }

        private void Start()
        {
            if (visionSources.Count == 0)
            {
                AddPlayer();
            }
            InitializeFog();
            InitializeCells();
        }

        private void AddPlayer()
        {
            VisionSource toAdd = new();
            toAdd.VisionRadius = 7f;
            toAdd.Revealer = GameObject.FindWithTag("Player");
            visionSources.Add(toAdd);
        }

        private void InitializeFog()
        {
            fogPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            fogPlane.name = "Fog Plane";
            fogPlane.transform.position = new Vector3(mapCenter.position.x, mapCenter.position.y + fogPlaneHeight, mapCenter.position.z);
            fogPlane.transform.localScale = new Vector3((levelDimensionX * unitScale) / 10f, 1f, (levelDimensionY * unitScale) / 10f);
            fogPlane.transform.rotation = mapCenter.rotation;

            fogPlane.GetComponent<MeshRenderer>().material = fogMaterial;

            fogPlaneTexture = new Texture2D(levelDimensionX, levelDimensionY);
            fogPlaneTextureBuffer = new Texture2D(levelDimensionX, levelDimensionY);

            fogPlaneTexture.filterMode = FilterMode.Bilinear;

            fogPlaneTexture.wrapMode = TextureWrapMode.Clamp;

            float initialAlpha = isAOS ? fogAlpha.invisible : fogAlpha.hidden;
            fogPixels = new Color[levelDimensionX * levelDimensionY];
            for (int i = 0; i < fogPixels.Length; i++)
            {
                fogPixels[i] = new Color(fogColor.r, fogColor.g, fogColor.b, initialAlpha);
            }
            fogPlaneTexture.SetPixels(fogPixels);
            fogPlaneTexture.Apply();

            fogPlane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", fogPlaneTexture);

        }

        private void Update()
        {
            UpdateFog();
        }

        private void UpdateFog()
        {
            if (cells == null || fogPixels == null) return;

            // 시야 탐색
            if (Time.time - lastVisionUpdateTime >= visionUpdateInterval)
            {
                lastVisionUpdateTime = Time.time;
                RecalculateVision();
            }

            // 화면 전환 및 텍스처 업데이트
            UpdateFadingAndTexture();
        }

        private void RecalculateVision()
        {
            currentVisibleCells.Clear();
            currentVisibleSet.Clear();

            foreach (var source in visionSources)
            {
                if (source.Revealer == null) continue;

                Vector3 sourcePos = source.Revealer.transform.position;

                // UpdateOnlyOnMove 활성화 시, 제자리에서 움직이지 않았다면 기존 캐시 시야를 사용하여 불필요한 연산 제거
                bool needsRecalculate = !source.IsInitialized ||
                                        (sourcePos - source.LastPosition).sqrMagnitude > 0.001f;

                if (source.UpdateOnlyOnMove && !needsRecalculate)
                {
                    for (int i = 0; i < source.CachedVisibleCells.Count; i++)
                    {
                        Vector2Int pos = source.CachedVisibleCells[i];
                        if (currentVisibleSet.Add(pos))
                        {
                            currentVisibleCells.Add(pos);
                        }
                    }
                    continue;
                }

                // 시야 재계산
                source.CachedVisibleCells.Clear();
                source.LastPosition = sourcePos;
                source.IsInitialized = true;

                Vector2Int sourceCell = WorldToCell(sourcePos);
                int cellRadius = Mathf.CeilToInt(source.VisionRadius / unitScale) + 2;

                int minX = Mathf.Clamp(sourceCell.x - cellRadius, 0, levelDimensionX - 1);
                int maxX = Mathf.Clamp(sourceCell.x + cellRadius, 0, levelDimensionX - 1);
                int minY = Mathf.Clamp(sourceCell.y - cellRadius, 0, levelDimensionY - 1);
                int maxY = Mathf.Clamp(sourceCell.y + cellRadius, 0, levelDimensionY - 1);

                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        FogCell cell = cells[x, y];
                        float distance = Vector3.Distance(
                            new Vector3(sourcePos.x, 0, sourcePos.z),
                            new Vector3(cell.WorldPosition.x, 0, cell.WorldPosition.z)
                        );

                        if (distance <= source.VisionRadius + (unitScale * 0.5f))
                        {
                            Vector3 rayStart = sourcePos;
                            rayStart.y = sourcePos.y + 1f;
                            Vector3 rayEnd = cell.WorldPosition;
                            rayEnd.y = cell.WorldPosition.y + 1f;

                            if (!Physics.Linecast(rayStart, rayEnd, obstacleLayer))
                            {
                                Vector2Int pos = new Vector2Int(x, y);
                                source.CachedVisibleCells.Add(pos);
                                if (currentVisibleSet.Add(pos)) // 중복 추가 방지
                                {
                                    currentVisibleCells.Add(pos);
                                }
                            }
                        }
                    }
                }
            }

            // 이전 시야와 비교하여 상태 변화가 있는 셀만 상태 변경

            // 시야에서 벗어난 셀 리셋
            for (int i = 0; i < lastVisibleCells.Count; i++)
            {
                Vector2Int pos = lastVisibleCells[i];
                if (!currentVisibleSet.Contains(pos))
                {
                    FogCell cell = cells[pos.x, pos.y];
                    cell.TargetAlpha = fogAlpha.invisible;
                    cell.IsVisible = false;
                    cell.State = FogState.Invisible;

                    if (!isFading[pos.x, pos.y])
                    {
                        isFading[pos.x, pos.y] = true;
                        fadingCells.Add(pos);
                    }
                }
            }

            // 새로 시야에 들어온 셀 설정
            for (int i = 0; i < currentVisibleCells.Count; i++)
            {
                Vector2Int pos = currentVisibleCells[i];
                FogCell cell = cells[pos.x, pos.y];

                cell.TargetAlpha = fogAlpha.visible;
                cell.IsVisible = true;
                cell.State = FogState.Visible;

                if (!isFading[pos.x, pos.y] && !Mathf.Approximately(cell.CurrentAlpha, cell.TargetAlpha))
                {
                    isFading[pos.x, pos.y] = true;
                    fadingCells.Add(pos);
                }
            }

            // 스왑을 통한 GC Alloc 최적화
            List<Vector2Int> temp = lastVisibleCells;
            lastVisibleCells = currentVisibleCells;
            currentVisibleCells = temp;
        }

        private void UpdateFadingAndTexture()
        {
            // Fadeing 셀들의 알파값 갱신 및 텍스처 데이터 버퍼 수정
            bool textureDirty = false;
            for (int i = fadingCells.Count - 1; i >= 0; i--)
            {
                Vector2Int pos = fadingCells[i];
                FogCell cell = cells[pos.x, pos.y];

                cell.CurrentAlpha = Mathf.MoveTowards(cell.CurrentAlpha, cell.TargetAlpha, fogFadeSpeed * Time.deltaTime);
                fogPixels[pos.y * levelDimensionX + pos.x] = new Color(fogColor.r, fogColor.g, fogColor.b, cell.CurrentAlpha);
                textureDirty = true;

                if (Mathf.Approximately(cell.CurrentAlpha, cell.TargetAlpha))
                {
                    cell.CurrentAlpha = cell.TargetAlpha;
                    isFading[pos.x, pos.y] = false;
                    fadingCells.RemoveAt(i);
                }
            }

            // 실제로 변화가 있을 때만 데이터 전송
            if (textureDirty)
            {
                fogPlaneTexture.SetPixels(fogPixels);
                fogPlaneTexture.Apply();
            }

            UpdateHideLayerObjects();
        }

        private void UpdateHideLayerObjects()
        {
            if (hideLayer == 0) return;

            Vector3 boxHalfExtents = new Vector3((levelDimensionX * unitScale) / 2f, 10f, (levelDimensionY * unitScale) / 2f);

            int count = Physics.OverlapBoxNonAlloc(mapCenter.position, boxHalfExtents, overlapBuffer, mapCenter.rotation, hideLayer);

            for (int i = 0; i < count; i++)
            {
                Collider hit = overlapBuffer[i];
                if (hit == null) continue;

                if (hit.TryGetComponent<FogHideTarget>(out var hideTarget))
                {
                    hideTarget.SetVisible(IsPositionVisible(hit.transform.position));
                }
            }

            System.Array.Clear(overlapBuffer, 0, count);
        }
        private bool IsPositionVisible(Vector3 worldPos, int range = 1)
        {
            Vector2Int center = WorldToCell(worldPos);

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    int checkX = center.x + x;
                    int checkY = center.y + y;

                    if (checkX < 0 || checkX >= levelDimensionX ||
                        checkY < 0 || checkY >= levelDimensionY)
                        continue;

                    if (cells[checkX, checkY].IsVisible)
                        return true;
                }
            }

            return false;
        }

        private void InitializeCells()
        {
            cells = new FogCell[levelDimensionX, levelDimensionY];
            isFading = new bool[levelDimensionX, levelDimensionY];

            lastVisibleCells.Clear();
            currentVisibleCells.Clear();
            currentVisibleSet.Clear();
            fadingCells.Clear();

            for (int x = 0; x < levelDimensionX; x++)
            {
                for (int y = 0; y < levelDimensionY; y++)
                {
                    cells[x, y] = new FogCell();

                    float localX = -(levelDimensionX * unitScale) / 2f + (x * unitScale) + (unitScale / 2f);
                    float localZ = -(levelDimensionY * unitScale) / 2f + (y * unitScale) + (unitScale / 2f);

                    Vector3 localPos = new Vector3(localX, 0, localZ);
                    Vector3 worldPos = mapCenter.position + (mapCenter.rotation * localPos);
                    worldPos.y = mapCenter.position.y;

                    if (groundLayer != 0)
                    {
                        Vector3 rayStart = new Vector3(worldPos.x, mapCenter.position.y + 50f, worldPos.z);
                        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, groundLayer))
                        {
                            worldPos.y = hit.point.y;
                        }
                    }

                    cells[x, y].WorldPosition = worldPos;

                    cells[x, y].State = isAOS ? FogState.Invisible : FogState.Hidden;
                    cells[x, y].IsVisible = false;
                    cells[x, y].CurrentAlpha = isAOS ? fogAlpha.invisible : fogAlpha.hidden;
                    cells[x, y].TargetAlpha = isAOS ? fogAlpha.invisible : fogAlpha.hidden;
                }
            }
        }

        private Vector2Int WorldToCell(Vector3 worldPos)
        {
            Vector3 offset = worldPos - mapCenter.position;
            Vector3 localPos = Quaternion.Inverse(mapCenter.rotation) * offset;

            int x = Mathf.FloorToInt((localPos.x + (levelDimensionX * unitScale) / 2f) / unitScale);
            int y = Mathf.FloorToInt((localPos.z + (levelDimensionY * unitScale) / 2f) / unitScale);

            x = Mathf.Clamp(x, 0, levelDimensionX - 1);
            y = Mathf.Clamp(y, 0, levelDimensionY - 1);

            return new Vector2Int(x, y);
        }
    }
}