using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public float value;
    public float reward;
    public Action action;
    public State()
    {
        value = 0;
        reward = 0;
        action = Action.Down;
    }
}
