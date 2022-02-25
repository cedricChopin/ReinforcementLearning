using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban_Controller : AI_Controller
{
    public override State getNextState(State actualState, int x)
    {
        // TODO Sokoban
        return null;
    }
    public override Action getBestAction(int indexState)
    {
        // TODO Sokoban
        return Action.None;
    }

    public override List<State> GetPossibleActions(State s)
    {
        // TODO Sokoban
        return null;
    }
}
