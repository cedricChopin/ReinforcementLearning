using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Dictionary<GameObject, Vector2> listCaisse;
    public List<Vector2> listWin;

    public GameMode mode = GameMode.GridWorld;

    private Dictionary<Vector2, Tile> tilesDict;

    public List<List<State>> States;

    private void Start()
    {
        GenerateGrid();
        listCaisse = new Dictionary<GameObject, Vector2>();
        listWin = new List<Vector2>();
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
        {
            return tile;
        }
        else return null;
    }

    /// <summary>
    /// Initialise les etats a leur valeur de base en fonction de la grille actuelle
    /// </summary>
    /// <param name="States"></param>
    public void InitGrid()
    {
        listWin.Clear();
        for (int x = 0; x < width; x++)
        {

            for (int y = 0; y < height; y++)
            {
                States[x][y].reward = new List<float> { -1f, -1f, -1f, -1f };
                States[x][y].value = new List<float> { 0f, 0f, 0f, 0f };
                if (GetTileAtPosition(new Vector2(x, y)).rend.color == Color.green)
                {
                    changeStateAround_Win(States[x][y]);
                    listWin.Add(States[x][y].pos);
                }
                changeStateAround_Vide(States[x][y]);

            }
        }
    }

    private void changeStateAround_Win(State state)
    {
        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        state.isWin = true;
        if (y + 1 < height)
            States[x][y + 1].reward[(int)Action.Down] = 1000f;
        if (y - 1 >= 0)
            States[x][y - 1].reward[(int)Action.Top] = 1000f;
        if (x + 1 < width)
            States[x + 1][y].reward[(int)Action.Left] = 1000f;
        if (x - 1 >= 0)
            States[x - 1][y].reward[(int)Action.Right] = 1000f;
    }

    private void changeStateAround_Obstacle(State state)
    {
        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        if (y + 1 >= height || isObstacle(States[x][y + 1]))
            States[x][y + 1].reward[(int)Action.Down] = -2f;
        if (y - 1 < 0 || isObstacle(States[x][y - 1]))
            States[x][y - 1].reward[(int)Action.Top] = -2f;
        if (x + 1 >= width || isObstacle(States[x + 1][y]))
            States[x + 1][y].reward[(int)Action.Left] = -2f;
        if (x - 1 < 0 || isObstacle(States[x - 1][y]))
            States[x - 1][y].reward[(int)Action.Right] = -2f;
    }

    private void changeStateAround_Vide(State state)
    {
        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        if (y + 1 >= height || isObstacle(States[x][y + 1]))
            States[x][y].reward[(int)Action.Top] = -2f;
        if (y - 1 < 0 || isObstacle(States[x][y - 1]))
            States[x][y].reward[(int)Action.Down] = -2f;
        if (x + 1 >= width || isObstacle(States[x + 1][y]))
            States[x][y].reward[(int)Action.Right] = -2f;
        if (x - 1 < 0 || isObstacle(States[x - 1][y]))
            States[x][y].reward[(int)Action.Left] = -2f;
    }

    public void ChangeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = GetTileAtPosition(new Vector2(x, y));
                tile.GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, -4.418206f, 0, -4.346054f);
                tile.GetComponentInChildren<TextMeshProUGUI>().text = "";
                tile.GetComponentInChildren<TextMeshProUGUI>().text += "↑ " + States[x][y].value[(int)Action.Top].ToString("N2") + "\n";
                tile.GetComponentInChildren<TextMeshProUGUI>().text += "↓ " + States[x][y].value[(int)Action.Down].ToString("N2") + "\n";
                tile.GetComponentInChildren<TextMeshProUGUI>().text += "← " + States[x][y].value[(int)Action.Left].ToString("N2") + "\n";
                tile.GetComponentInChildren<TextMeshProUGUI>().text += "→ " + States[x][y].value[(int)Action.Right].ToString("N2");
            }
        }
    }
    public bool isObstacle(State state)
    {
        if (GetTileAtPosition(new Vector2(state.pos.x, state.pos.y)) == null)
            return true;
        return GetTileAtPosition(new Vector2(state.pos.x, state.pos.y)).rend.color == Color.red;
    }

    public void MoveCaisse(Vector2 pos, Vector2 newPos)
    {
        var caisse = listCaisse.FirstOrDefault(x=>x.Value == pos).Key;
        listCaisse[caisse] = newPos;
        caisse.transform.position = newPos;
        GetTileAtPosition(pos).gotCaisse = false;
        GetTileAtPosition(newPos).gotCaisse = true;
        States[(int)pos.x][(int)pos.y].hasCaisse = false;
        States[(int)newPos.x][(int)newPos.y].hasCaisse = true;
    }

    public float SimulateMoveCaisse(Vector2 pos, Vector2 newPos, ref List<List<State>> lststate, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        var caisse = lstCaisse.FirstOrDefault(x => x.Value == pos).Key;
        lstCaisse[caisse] = newPos;
        lststate[(int)pos.x][(int)pos.y].hasCaisse = false;
        lststate[(int)newPos.x][(int)newPos.y].hasCaisse = true;
        if (lststate[(int)newPos.x][(int)newPos.y].isWin)
        {
            return 1000f;
        }
        else
        {
            return -1f;
        }
    }
}
