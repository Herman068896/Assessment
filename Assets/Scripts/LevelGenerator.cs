using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;
    public int[,] levelMap;
    public float cellSize = 1f;
    public int width = 19;
    public int height = 21;

    void Awake()
    {
        Instance = this;
        levelMap = new int[width, height];
        GenerateSimpleMap();
    }

    void GenerateSimpleMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    levelMap[x, y] = 1;
                else
                    levelMap[x, y] = 2;
            }
        }
        levelMap[width / 2, height / 2] = 3;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gx = Mathf.RoundToInt(worldPos.x / cellSize);
        int gy = Mathf.RoundToInt(worldPos.y / cellSize);
        return new Vector2Int(gx, gy);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0f);
    }

    public bool IsWalkable(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width || gridPos.y >= height)
            return false;
        int tile = levelMap[gridPos.x, gridPos.y];
        if (tile == 1) return false;
        return true;
    }

    public bool HasPelletAt(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width || gridPos.y >= height)
            return false;
        return levelMap[gridPos.x, gridPos.y] == 2;
    }

    public void EatPellet(Vector2Int gridPos)
    {
        if (HasPelletAt(gridPos))
            levelMap[gridPos.x, gridPos.y] = 0;
    }
}
