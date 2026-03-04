using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;          // 플레이어 참조
    public float activeRadius = 30f;  // 플레이어 주변 활성화 거리
    public Vector2Int gridSize = new Vector2Int(50, 50); // Chunk 크기 (타일 단위)

    // 그리드 인덱스로 Chunk를 관리
    private Dictionary<Vector2Int, GameObject> chunkGrid = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // 씬 내 모든 Chunk 등록
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Chunk"))
            {
                Vector2Int gridPos = WorldToGrid(child.position);
                if (!chunkGrid.ContainsKey(gridPos))
                    chunkGrid.Add(gridPos, child.gameObject);
                
                Debug.Log($"등록됨: {child.name} at {gridPos}");

            }
        }
    }

    void Update()
    {
        // 플레이어가 속한 그리드 좌표
        Vector2Int playerGrid = WorldToGrid(player.position);

        // 플레이어 주변 셀만 검사
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int checkGrid = new Vector2Int(playerGrid.x + x, playerGrid.y + y);

                if (chunkGrid.TryGetValue(checkGrid, out GameObject chunk))
                {
                    float dist = Vector2.Distance(player.position, chunk.transform.position);
                    bool shouldBeActive = dist < activeRadius;

                    if (chunk.activeSelf != shouldBeActive)
                        Debug.Log($"{chunk.name} 활성화 상태 변경: {shouldBeActive}");
                        Debug.Log($"{chunk.name} dist={dist}, active={shouldBeActive}");

                        chunk.SetActive(shouldBeActive);
                }
            }
        }
    }

    // 월드 좌표 → 그리드 좌표 변환
    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gx = Mathf.FloorToInt(worldPos.x / gridSize.x);
        int gy = Mathf.FloorToInt(worldPos.y / gridSize.y);
        return new Vector2Int(gx, gy);
    }
}