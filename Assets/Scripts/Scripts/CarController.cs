using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float decisionDistance = 0.1f;

    [Header("References")]
    public Tilemap roadTilemap;

    private Vector2 currentDirection = Vector2.right;
    private Vector3Int lastTilePosition;
    private bool isOnRoad = true;

    void Start()
    {
        lastTilePosition = roadTilemap.WorldToCell(transform.position);
        SnapToGrid();
    }

    void Update()
    {
        if (!isOnRoad) return;

        Vector3Int currentTilePos = roadTilemap.WorldToCell(transform.position);
        if (currentTilePos != lastTilePosition || ShouldMakeDecision())
        {
            CheckRoadTile(currentTilePos);
            lastTilePosition = currentTilePos;
        }

        MoveCar();
        RotateCar();
    }

    void MoveCar()
    {
        transform.position += (Vector3)currentDirection * speed * Time.deltaTime;
    }

    void RotateCar()
    {
        float targetAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    bool ShouldMakeDecision()
    {
        Vector3 cellCenter = roadTilemap.GetCellCenterWorld(lastTilePosition);
        return Vector3.Distance(transform.position, cellCenter) < decisionDistance;
    }

    void CheckRoadTile(Vector3Int tilePos)
    {
        RoadTile tile = roadTilemap.GetTile<RoadTile>(tilePos);

        if (tile == null)
        {
            isOnRoad = false;
            speed = 0f;
            return;
        }

        // Conversia la Vector2Int pentru verificare
        Vector2Int currentDirInt = new Vector2Int(
            Mathf.RoundToInt(currentDirection.x),
            Mathf.RoundToInt(currentDirection.y));

        if (!tile.AllowsDirection(currentDirInt))
        {
            ChooseNewDirection(tile);
            SnapToGrid();
        }
    }

    void ChooseNewDirection(RoadTile tile)
    {
        List<Vector2> possibleDirections = new List<Vector2>();

        if (tile.allowNorth) possibleDirections.Add(Vector2.up);
        if (tile.allowSouth) possibleDirections.Add(Vector2.down);
        if (tile.allowEast) possibleDirections.Add(Vector2.right);
        if (tile.allowWest) possibleDirections.Add(Vector2.left);

        // Convertim direcția curentă la Vector2 pentru comparație
        Vector2 oppositeDirection = -currentDirection;
        possibleDirections.RemoveAll(dir => Vector2.Dot(dir, oppositeDirection) > 0.9f);

        if (possibleDirections.Count > 0)
        {
            currentDirection = possibleDirections[Random.Range(0, possibleDirections.Count)].normalized;
        }
        else
        {
            isOnRoad = false;
            speed = 0f;
        }
    }

    void SnapToGrid()
    {
        Vector3Int cellPos = roadTilemap.WorldToCell(transform.position);
        Vector3 cellCenter = roadTilemap.GetCellCenterWorld(cellPos);
        transform.position = new Vector3(cellCenter.x, cellCenter.y, transform.position.z);
    }

    // Adaugă acest cod temporar în CarController pentru debug
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3Int cellPos = roadTilemap.WorldToCell(transform.position);
        RoadTile tile = roadTilemap.GetTile<RoadTile>(cellPos);

        Gizmos.color = tile != null ? Color.green : Color.red;
        Gizmos.DrawWireCube(roadTilemap.GetCellCenterWorld(cellPos), Vector3.one * 0.9f);
    }
}