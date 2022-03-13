using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case
{
    public List<State> possibleActions;
    public bool hasCaisse;
    public Vector2 pos;
    public Case()
    {
        possibleActions = new List<State>();
        pos = Vector2.zero;
    }


}
