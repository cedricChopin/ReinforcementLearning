using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public float value;
    public float reward;
    public Action action;
    public bool hasCaisse;
    public Vector2 pos;
    public State()
    {
        value = 0;
        reward = 0;
        action = Action.None;
        hasCaisse = false;
        pos = Vector2.zero;
    }
}
