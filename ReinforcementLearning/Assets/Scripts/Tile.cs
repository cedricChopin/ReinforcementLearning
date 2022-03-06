using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] public SpriteRenderer rend;
    [SerializeField] private GameObject caisse;
    private GridManager grid;

    public bool gotCaisse = false;


    private void Start()
    {
        grid = transform.parent.GetComponentInChildren<GridManager>();
    }

    private Color previousColor;
    public void Init(bool isOffset)
    {
        rend.color = (isOffset) ? baseColor : offsetColor;
    }

    private void OnMouseDown()
    {
        //Cycle : Case normale - Case rouge (Obstacle) - Case Verte (Victoire)
        if (rend.color == baseColor)
        {
            previousColor = baseColor;
            rend.color = Color.red;
        }

        else if(rend.color == offsetColor)
        {
            previousColor = offsetColor;
            rend.color = Color.red;
        }
        else if(rend.color == Color.red)
        {
            rend.color = Color.green;
        }
        else
        {
            rend.color = previousColor;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && grid.mode == GameMode.Sokoban)
        {
            if (!gotCaisse)
            {
                gotCaisse = true;
                GameObject go = Instantiate(caisse, transform.position, Quaternion.identity);
                grid.listCaisse.Add(go);
            }
            else
            {
                foreach(GameObject g in grid.listCaisse)
                {
                    if(g.transform.position == transform.position)
                    {
                        Destroy(g);
                        grid.listCaisse.Remove(g);
                        break;
                    }
                }
                gotCaisse = false;
            }
        }
    }
}
