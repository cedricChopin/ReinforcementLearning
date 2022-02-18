using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] public int width, height;

    [SerializeField] private Tile tile;

    [SerializeField] private Transform camera;

    [SerializeField] private Transform parent;

    private Dictionary<Vector2, Tile> tilesDict;

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        tilesDict = new Dictionary<Vector2, Tile>();
        camera.position = new Vector3(width/2 - 0.5f, height/2 -0.5f, -10);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var spamedTile = Instantiate(tile, new Vector3(x, y), Quaternion.identity);
                spamedTile.transform.SetParent(parent);
                var isOffset = (x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0);
                spamedTile.Init(isOffset);
                spamedTile.name = $"Tile ({x})({y})";

                tilesDict[new Vector2(x, y)] = spamedTile;
            }
        }
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (tilesDict.TryGetValue(pos, out var tile))
            return tile;

        return null;
    }
}
