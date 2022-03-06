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
    public override Action getBestAction(State state, List<State> lstState)
    {
        // TODO Sokoban
        return Action.None;
    }

    public override List<State> GetPossibleActions(State s, List<State> lstState)
    {
        // TODO Sokoban
        return null;
    }

    public override bool isPossibleAction(State state, Action action, List<State> lstState)
    {
        int index = lstState.IndexOf(state);
        if (index < 0 || index >= grid.States.Count)
            return false;
        bool isPossible = false;
        switch (action)
        {
            case Action.Top:
                isPossible = index + grid.height < lstState.Count && lstState[index + grid.height].action != Action.None;
                break;
            case Action.Left:
                isPossible = index % grid.width != 0 && lstState[index - 1].action != Action.None;
                break;
            case Action.Down:
                isPossible = index - grid.height >= 0 && lstState[index - grid.height].action != Action.None;
                break;
            case Action.Right:
                isPossible = (index + 1) % grid.width != 0 && lstState[index + 1].action != Action.None;
                break;

        }

        return isPossible;
    }
}
