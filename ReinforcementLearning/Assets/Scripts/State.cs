using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actions possibles dans le monde
/// </summary>
public enum Action
{
    Top = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    None = 4,
    Win = 5
}
public class State
{
    public List<float> value;
    public List<float> reward;
    public List<Action> action;
    public bool hasCaisse;
    public Vector2 pos;
    public bool isWin;
    public State()
    {
        value = new List<float> { 0f, 0f, 0f, 0f };
        reward = new List<float> { -1f, -1f, -1f, -1f };
        action = new List<Action> { Action.Top, Action.Down, Action.Left, Action.Right };
        hasCaisse = false;
        pos = Vector2.zero;
        isWin = false;
    }

    public State(State previousState)
    {
        value = previousState.value;
        reward = previousState.reward;
        action = previousState.action;
        hasCaisse = previousState.hasCaisse;
        pos = previousState.pos;
    }


}
