using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban_Controller : AI_Controller
{
    public override State getNextState(State actualState, Action action)
    {
        // TODO Sokoban
        return null;
    }
    public override Action getBestAction(State state, List<List<State>> lstState)
    {
        // TODO Sokoban
        return Action.None;
    }

    public override List<State> GetPossibleActions(State s, List<List<State>> lstState)
    {
        // TODO Sokoban
        return null;
    }

    public override bool isPossibleAction(State state, Action action, List<List<State>> lstState)
    {
        return false;
    }
}
