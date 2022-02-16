using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Type
{
    enum Action
    {
        Top,
        Down,
        Left,
        Right
    }

    struct State
    {
        Vector2 pos;
        float distanceToGoal;
        List<nextState> nextState;
        float totalReward;
    }

    struct nextState
    {
        State s;
        float reward;
    }
}
