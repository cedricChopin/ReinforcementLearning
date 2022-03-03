using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] public SpriteRenderer rend;

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
}
