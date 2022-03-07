using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameMode
{
    GridWorld, Sokoban
}

public class GridManager : MonoBehaviour
{

    [SerializeField] public int width, height;

    [SerializeField] private Tile tile;

    [SerializeField] private Transform cam;

    [SerializeField] private Transform parent;

    public List<GameObject> listCaisse;

    public GameMode mode = GameMode.GridWorld;

    private Dictionary<Vector2, Tile> tilesDict;

    public List<List<State>> States;

    private void Start()
    {
        GenerateGrid();
        listCaisse = new List<GameObject>();
    }

    /// <summary>
    /// Génère une grille carrée de taille width * height
    /// </summary>
    void GenerateGrid()
    {
        tilesDict = new Dictionary<Vector2, Tile>();
        cam.position = new Vector3(width/2 - 0.5f, height/2 -0.5f, -10);
        States = new List<List<State>>(width);
        for (int x = 0; x < width; x++)
        {
            States.Add(new List<State>());
            for(int y = 0; y < height; y++)
            {
                var spamedTile = Instantiate(tile, new Vector3(x, y), Quaternion.identity);
                spamedTile.transform.SetParent(parent);
                var isOffset = (x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0);
                spamedTile.Init(isOffset);
                spamedTile.name = $"Tile ({x})({y})";

                tilesDict[new Vector2(x, y)] = spamedTile;

                State state = new State();

                state.action = (Action)Random.Range(0, 4);
                state.pos = new Vector2(x, y);
                States[x].Add(state);
            }
        }
    }
    /// <summary>
    /// Récupère la Tile correspondant à la position
    /// </summary>
    /// <param name="pos">Position à l'intérieure d'une Tile</param>
    /// <returns></returns>
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (tilesDict.TryGetValue(pos, out var tile))
            return tile;

        return null;
    }

    /// <summary>
    /// Initialise les etats a leur valeur de base en fonction de la grille actuelle
    /// </summary>
    /// <param name="States"></param>
    public void InitGrid()
    {
        for (int x = 0; x < width; x++)
        {

            for (int y = 0; y < height; y++)
            {
                States[x][y].reward = 0f;
                States[x][y].value = 0f;
                States[x][y].action = (Action)Random.Range(0, 4);
                if (GetTileAtPosition(new Vector2(x, y)).rend.color == Color.green)
                {
                    States[x][y].reward = 1;
                    States[x][y].action = Action.Win;
                }
                else if (GetTileAtPosition(new Vector2(x, y)).rend.color == Color.red)
                {
                    States[x][y].reward = -1f;
                    States[x][y].action = Action.None;
                }

            }
        }
    }

    public void ChangeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = GetTileAtPosition(new Vector2(x, y));
                if (States[x][y].action == Action.Left)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "← \n" + States[x][y].value.ToString("N3");
                else if (States[x][y].action == Action.Right)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "→ \n" + States[x][y].value.ToString("N3");
                else if (States[x][y].action == Action.Top)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "↑ \n" + States[x][y].value.ToString("N3");
                else if (States[x][y].action == Action.Down)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "↓ \n" + States[x][y].value.ToString("N3");
                else
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = States[x][y].value.ToString("N3");
            }
        }
    }
}
