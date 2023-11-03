using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates;
    public Tile tile;

    public bool IsEmpty()
    {
        return tile == null;
    }

    public bool IsOccupied()
    {
        return tile != null;
    }
}
