using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = System.Numerics.Vector2;

public class AStarManager : MonoBehaviour
{
    [SerializeField] Tilemap walkableTilemap;
    [SerializeField] Transform idie;
    [SerializeField] Transform highlight;

    //Note: In C#, variables without an access modifier are private by default
    Vector3Int[,] walkableArea;
    Astar astar;
    BoundsInt bounds;

    List<Spot> idiePath = new List<Spot>();
    private Vector3Int GridPositionOfMouse3D
    {
        get
        {
            return walkableTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private Vector2Int GridPositionOfMouse2D => (Vector2Int)GridPositionOfMouse3D;

    private Vector2Int GridPositionOfIdie
    {
        get
        {
            return (Vector2Int)walkableTilemap.WorldToCell(idie.position);
        }
    }

    private void Start()
    {
        //Trims any empty cells from the edges of the tilemap
        walkableTilemap.CompressBounds();
        bounds = walkableTilemap.cellBounds;

        CreateGrid();
        astar = new Astar(walkableArea, bounds.size.x, bounds.size.y);
    }

    private void CreateGrid()
    {
        walkableArea = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (walkableTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    walkableArea[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    walkableArea[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }

    private void Update()
    {
        highlight.position = walkableTilemap.GetCellCenterWorld(GridPositionOfMouse3D);

        //Fill in your logic here
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            idiePath = astar.CreatePath(walkableArea, GridPositionOfIdie, GridPositionOfMouse2D);
            if (idiePath == null)
            {
                return;
            }
            StartCoroutine(IdieMover(idiePath));
        }
    }

    IEnumerator IdieMover(List<Spot> list)
    {
        for(int x = 0; x < list.Count; x++)
        {
            idie.transform.position = new Vector3Int(list[x].X, list[x].Y);
            yield return new WaitForSeconds(.25f);
        }
    }
}
