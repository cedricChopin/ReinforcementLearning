using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;

    private Color previousColor;
    public void Init(bool isOffset)
    {
        renderer.color = (isOffset) ? baseColor : offsetColor;
    }

    private void OnMouseDown()
    {
        if (renderer.color == baseColor)
        {
            previousColor = baseColor;
            renderer.color = Color.red;
        }

        else if(renderer.color == offsetColor)
        {
            previousColor = offsetColor;
            renderer.color = Color.red;
        }
        else
        {
            renderer.color = previousColor;
        }
    }
}
