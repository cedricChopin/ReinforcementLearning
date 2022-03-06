using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GridWorld_Controller : AI_Controller
{


    public override State getNextState(State actualState, int x)
    {
        State NextState = null;
        switch (actualState.action)
        {
            case Action.Top:
                if (x + grid.height < policy.States.Count)
                {
                    NextState = policy.States[x + grid.height];
                }
                break;
            case Action.Down:
                if (x - grid.height >= 0)
                {
                    NextState = policy.States[x - grid.height];
                }
                break;
            case Action.Right:
                if ((x + 1) % grid.width != 0)
                {
                    NextState = policy.States[x + 1];
                }
                break;
            case Action.Left:
                if (x % grid.width != 0)
                {
                    NextState = policy.States[x - 1];
                }
                break;

        }

        return NextState;


    }

    public override Action getBestAction(int indexState)
    {
        float bestReward = -1;
        Action bestAction = Action.None;

        if (isPossibleAction(Action.Left, indexState) && bestReward < calculateValue(indexState - 1))
        {
            bestReward = calculateValue(indexState - 1);
            bestAction = Action.Left;
        }
        if (isPossibleAction(Action.Right, indexState) && bestReward < calculateValue(indexState + 1))
        {
            bestReward = calculateValue(indexState + 1);
            bestAction = Action.Right;
        }
        if (isPossibleAction(Action.Down, indexState) && bestReward < calculateValue(indexState - grid.height))
        {
            bestReward = calculateValue(indexState - grid.height);
            bestAction = Action.Down;
        }
        if (isPossibleAction(Action.Top, indexState) && bestReward < calculateValue(indexState + grid.height))
        {
            bestAction = Action.Top;
        }
        Assert.IsTrue(bestAction != Action.None);
        return bestAction;
    }

    public override List<State> GetPossibleActions(State s)
    {
        List<State> possibleStates = new List<State>();
        int index = policy.States.IndexOf(s);
        if (s.action != Action.None && s.action != Action.Win)
        {
            if (isPossibleAction(Action.Top, index))
            {
                State tmp = copyState(index + grid.height);
                tmp.action = Action.Top;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(Action.Down, index))
            {
                State tmp = copyState(index - grid.height);
                tmp.action = Action.Down;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(Action.Right, index))
            {
                State tmp = copyState(index + 1);
                tmp.action = Action.Right;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(Action.Left, index))
            {
                State tmp = copyState(index - 1);
                tmp.action = Action.Left;
                possibleStates.Add(tmp);
            }
        }
        return possibleStates;
    }
}
