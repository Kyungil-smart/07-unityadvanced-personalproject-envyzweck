using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public float activeRadius = 30f;
    public Vector2Int gridSize = new Vector2Int(50, 50);

    private Dictionary<Vector2Int, GameObject> chunkGrid = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        foreach (Transform chunk in transform) 
        {
            if (chunk.name.StartsWith("Chunk"))
            {
                Vector2Int gridPos = WorldToGrid(chunk.position);

                if (!chunkGrid.ContainsKey(gridPos))
                {
                    chunkGrid.Add(gridPos, chunk.gameObject);
                    Debug.Log($"등록됨: {chunk.name} at {gridPos}");
                }
                else
                {
                    Debug.LogWarning($"중복된 그리드 좌표 발견: {chunk.name}이 {gridPos}에 있음.");
                }
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int playerGrid = WorldToGrid(player.position);

        foreach (var kvp in chunkGrid)
        {
            GameObject chunk = kvp.Value;
            float dist = Vector2.Distance(new Vector2(player.position.x, player.position.y), 
                                          new Vector2(chunk.transform.position.x, chunk.transform.position.y));
            
            bool shouldBeActive = dist < activeRadius;
            
            if (chunk.activeSelf != shouldBeActive)
            {
                chunk.SetActive(shouldBeActive);
                Debug.Log($"{chunk.name} 활성화: {shouldBeActive} (거리: {dist})");
            }
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gx = Mathf.FloorToInt(worldPos.x / gridSize.x);
        int gy = Mathf.FloorToInt(worldPos.y / gridSize.y);
        return new Vector2Int(gx, gy);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        for (int i = -10; i <= 10; i++)
        {
            Gizmos.DrawLine(new Vector3(i * 10, -100, 0), new Vector3(i * 10, 100, 0));
            Gizmos.DrawLine(new Vector3(-100, i * 10, 0), new Vector3(100, i * 10, 0));
        }
        
        Gizmos.color = Color.yellow;
        for (int i = -5; i <= 5; i++)
        {
            float pos = i * gridSize.x;
            Gizmos.DrawLine(new Vector3(pos, -100, 0), new Vector3(pos, 100, 0));
            Gizmos.DrawLine(new Vector3(-100, pos, 0), new Vector3(100, pos, 0));
        }
    }
}