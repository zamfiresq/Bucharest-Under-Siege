using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public Tilemap roadTilemap;

    [Header("Debug")]
    public bool showDebug = true;

    private Vector2 currentDirection = Vector2.right;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SnapToGrid();
    }

    void Update()
    {
        MoveCar();
        CheckCurrentTile();
    }

    void MoveCar()
    {
        rb.velocity = currentDirection * speed;
        RotateCar();
    }

    void RotateCar()
    {
        float targetAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(0, 0, targetAngle),
            rotationSpeed * Time.deltaTime);
    }

    void CheckCurrentTile()
    {
        Vector3Int cellPos = roadTilemap.WorldToCell(transform.position);
        RoadTile currentTile = roadTilemap.GetTile<RoadTile>(cellPos);

        if (currentTile == null || !IsDirectionAllowed(currentTile))
        {
            ChangeDirection(currentTile);
            SnapToGrid();
        }

        Debug.Log($"Current Tile: {roadTilemap.GetTile<RoadTile>(cellPos).name} | Direction: {currentDirection}");
    }

    bool IsDirectionAllowed(RoadTile tile)
    {
        return (currentDirection == Vector2.up && tile.allowNorth) ||
               (currentDirection == Vector2.down && tile.allowSouth) ||
               (currentDirection == Vector2.right && tile.allowEast) ||
               (currentDirection == Vector2.left && tile.allowWest);
    }

    void ChangeDirection(RoadTile tile)
    {
        List<Vector2> possibleDirections = new List<Vector2>();

        if (tile.allowNorth) possibleDirections.Add(Vector2.up);
        if (tile.allowSouth) possibleDirections.Add(Vector2.down);
        if (tile.allowEast) possibleDirections.Add(Vector2.right);
        if (tile.allowWest) possibleDirections.Add(Vector2.left);

        // Remove opposite direction
        possibleDirections.RemoveAll(dir => dir == -currentDirection);

        if (possibleDirections.Count > 0)
        {
            currentDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SnapToGrid()
    {
        Vector3Int cellPos = roadTilemap.WorldToCell(transform.position);
        transform.position = roadTilemap.GetCellCenterWorld(cellPos);
    }

    void OnDrawGizmos()
    {
        if (!showDebug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentDirection * 0.5f);

        if (roadTilemap != null)
        {
            Vector3Int cell = roadTilemap.WorldToCell(transform.position);
            Gizmos.DrawWireCube(roadTilemap.GetCellCenterWorld(cell), Vector3.one * 0.9f);
        }
    }
}