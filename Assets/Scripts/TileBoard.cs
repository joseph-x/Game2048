using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting = false;

    private int difficultMode = 0;

    void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>();
        waiting = false;
    }

    void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Move(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }

        }
    }

    public void ClearBoard()
    {
        foreach (var c in grid.cells)
        {
            c.tile = null;
        }

        foreach (var t in tiles)
        {
            Destroy(t.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        TileCell tc = grid.GetRandomEmptyCell();

        if (tc != null)
        {
            Tile tile = Instantiate(tilePrefab, grid.transform);
            TileState state;

            if (tiles.Count > (grid.size / 2)) { difficultMode = 1; }

            switch (difficultMode)
            {
                case 0:
                    {
                        state = tileStates[0];

                        break;
                    }
                case 1:
                    {
                        bool randomBool = UnityEngine.Random.value > 0.5f;

                        if (randomBool)
                        {
                            state = tileStates[0];
                        }
                        else
                        {
                            state = tileStates[1];
                        }

                        break;
                    }
                default:
                    {
                        state = tileStates[0];
                        break;
                    }
            }

            tile.SetState(state);
            tile.LinkCell(tc);
            tiles.Add(tile);
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var t in tiles)
        {
            TileCell up = grid.GetAdjacentCell(t.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(t.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(t.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(t.cell, Vector2Int.right);

            if (up != null && CanMerge(t, up.tile))
            {
                return false;
            }

            if (down != null && CanMerge(t, down.tile))
            {
                return false;
            }

            if (right != null && CanMerge(t, right.tile))
            {
                return false;
            }

            if (left != null && CanMerge(t, left.tile))
            {
                return false;
            }
        }

        return true;
    }

    private void Move(Vector2Int direction, int startX,int incrementX, int startY, int incrementY)
    {
        bool change = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell tileCell = grid.GetCell(x, y);

                if (tileCell.IsOccupied())
                {
                    change |= MoveTile(tileCell.tile, direction);
                }
            }
        }

        if (change)
        {
            StartCoroutine(WaitforChanges());
        }
    }

    private IEnumerator WaitforChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.3f);

        Next();
        waiting = false;
    }

    private void Next()
    {
        foreach (var t in tiles)
        {
            t.locked = false;
        }

        CreateTile();

        bool randomBool = UnityEngine.Random.value > 0.5f;
        if(randomBool)
        { CreateTile(); }
        

        if(CheckForGameOver())
        {
            gameManager.GameOver();
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.IsOccupied())
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }


    private bool CanMerge(Tile a, Tile b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);


        // int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int index = IndexOf(b.state) + 1;
        TileState newState = tileStates[index];

        b.SetState(newState);

        gameManager.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        int index = 0;

        for(int i = 0; i < tileStates.Length; i++)
        {
            if (tileStates[i].Equals(state))
            {
                index = i;
                return index;
            }
        }

        return index;
    }


}
