using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban : MonoBehaviour
{
    [SerializeField] private GridManager grid;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) GoLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) GoRight();
        if (Input.GetKeyDown(KeyCode.UpArrow)) GoTop();
        if (Input.GetKeyDown(KeyCode.DownArrow)) GoDown();
    }

    public void GoLeft()
    {
        if (transform.position.x - 1 >= 0)
        {
            Vector2 newPos = new Vector2((int)transform.position.x - 1, (int)transform.position.y);

            if (grid.GetTileAtPosition(newPos).gotCaisse)
            {
                if (newPos.x - 1 >= 0)
                {
                    if (!grid.States[(int)newPos.x - 1][(int)newPos.y].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x - 1,newPos.y)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.MoveCaisse(newPos, new Vector2(newPos.x - 1, newPos.y));
                    }
                }
            }
            else if(grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red) 
                transform.position = newPos;
        }
    }

    public void GoRight()
    {
        if (transform.position.x + 1 < grid.width)
        {
            Vector2 newPos = new Vector2((int)transform.position.x + 1, (int)transform.position.y);
            if (grid.GetTileAtPosition(newPos).gotCaisse)
            {
                if (newPos.x + 1 < grid.width)
                {
                    if (!grid.States[(int)newPos.x + 1][(int)newPos.y].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x + 1, newPos.y)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.MoveCaisse(newPos, new Vector2(newPos.x + 1, newPos.y));
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
        }
    }

    public void GoTop()
    {
        if (transform.position.y + 1 < grid.height)
        {
            Vector2 newPos = new Vector2((int)transform.position.x, (int)transform.position.y + 1);
            if (grid.GetTileAtPosition(newPos).gotCaisse)
            {
                if (newPos.y + 1 < grid.height)
                {
                    if (!grid.States[(int)newPos.x][(int)newPos.y + 1].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y + 1)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.MoveCaisse(newPos, new Vector2(newPos.x, newPos.y + 1));
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
        }
    }

    public void GoDown()
    {
        if (transform.position.y - 1 >= 0)
        {
            Vector2 newPos = new Vector2((int)transform.position.x, (int)transform.position.y - 1);
            if (grid.GetTileAtPosition(newPos).gotCaisse)
            {
                if (newPos.y - 1 >= 0)
                {
                    if (!grid.States[(int)newPos.x][(int)newPos.y - 1].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y - 1)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.MoveCaisse(newPos, new Vector2(newPos.x, newPos.y - 1));
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
        }
    }
}
