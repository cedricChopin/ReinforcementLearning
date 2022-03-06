using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GridWorld_Controller : AI_Controller
{


    public override State getNextState(State actualState, Action action)
    {
        State NextState = null;
        int x = grid.States.IndexOf(actualState);
        switch (action)
        {
            case Action.Top:
                if (x + grid.height < grid.States.Count)
                {
                    NextState = grid.States[x + grid.height];
                }
                break;
            case Action.Down:
                if (x - grid.height >= 0)
                {
                    NextState = grid.States[x - grid.height];
                }
                break;
            case Action.Right:
                if ((x + 1) % grid.width != 0)
                {
                    NextState = grid.States[x + 1];
                }
                break;
            case Action.Left:
                if (x % grid.width != 0)
                {
                    NextState = grid.States[x - 1];
                }
                break;

        }

        return NextState;


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

    public override Action getBestAction(State state, List<State> lstState)
    {
        float bestReward = -1;
        int indexState = grid.States.IndexOf(state);
        Action bestAction = Action.None;

        if (isPossibleAction(state, Action.Left, lstState) && bestReward < calculateValue(indexState - 1))
        {
            bestReward = calculateValue(indexState - 1);
            bestAction = Action.Left;
        }
        if (isPossibleAction(state, Action.Right, lstState) && bestReward < calculateValue(indexState + 1))
        {
            bestReward = calculateValue(indexState + 1);
            bestAction = Action.Right;
        }
        if (isPossibleAction(state, Action.Down, lstState) && bestReward < calculateValue(indexState - grid.height))
        {
            bestReward = calculateValue(indexState - grid.height);
            bestAction = Action.Down;
        }
        if (isPossibleAction(state, Action.Top, lstState) && bestReward < calculateValue(indexState + grid.height))
        {
            bestAction = Action.Top;
        }
        Assert.IsTrue(bestAction != Action.None);
        return bestAction;
    }

    public override List<State> GetPossibleActions(State s, List<State> lstState)
    {
        List<State> possibleStates = new List<State>();
        int index = grid.States.IndexOf(s);
        if (s.action != Action.None && s.action != Action.Win)
        {
            if (isPossibleAction(s, Action.Top, lstState))
            {
                State tmp = copyState(index + grid.height, lstState);
                tmp.action = Action.Top;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(s, Action.Down, lstState))
            {
                State tmp = copyState(index - grid.height, lstState);
                tmp.action = Action.Down;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(s, Action.Right, lstState))
            {
                State tmp = copyState(index + 1, lstState);
                tmp.action = Action.Right;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(s, Action.Left, lstState))
            {
                State tmp = copyState(index - 1, lstState);
                tmp.action = Action.Left;
                possibleStates.Add(tmp);
            }
        }
        return possibleStates;
    }
}
