using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }

    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size / height;

    void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    void Start()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].coordinates = new Vector2Int(j, i);
            }
        }
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        List<TileCell> emptyCells = new List<TileCell>();
        foreach (var cell in cells)
        {
            if (cell.IsEmpty())
            {
                emptyCells.Add(cell);
            }
        }

        if (emptyCells.Count > 1)
        {
            return emptyCells[Random.Range(0, emptyCells.Count)];
        }
        else if (emptyCells.Count == 1)
        {
            return emptyCells[0];
        }

        return null;
    }
}