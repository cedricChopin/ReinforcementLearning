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

        if (indexState % grid.width != 0 && bestReward < policy.States[indexState - 1].reward + policy.y * policy.States[indexState - 1].value)
        {
            bestReward = policy.States[indexState - 1].reward + policy.y * policy.States[indexState - 1].value;
            bestAction = Action.Left;
        }
        if ((indexState + 1) % grid.width != 0 && bestReward < policy.States[indexState + 1].reward + policy.y * policy.States[indexState + 1].value)
        {
            bestReward = policy.States[indexState + 1].reward + policy.y * policy.States[indexState + 1].value;
            bestAction = Action.Right;
        }
        if (indexState - grid.height >= 0 && bestReward < policy.States[indexState - grid.height].reward + policy.y * policy.States[indexState - grid.height].value)
        {
            bestReward = policy.States[indexState - grid.height].reward + policy.y * policy.States[indexState - grid.height].value;
            bestAction = Action.Down;
        }
        if (indexState + grid.height < policy.States.Count && bestReward < policy.States[indexState + grid.height].reward + policy.y * policy.States[indexState + grid.height].value)
        {
            bestReward = policy.States[indexState + grid.height].reward + policy.y * policy.States[indexState + grid.height].value;
            bestAction = Action.Top;
        }
        Assert.IsTrue(bestAction != Action.None);
        return bestAction;
    }

    public override List<State> GetPossibleActions(State s)
    {
        List<State> possibleStates = new List<State>();
        if (s.action == Action.None || s.action == Action.Win)
        {
            return possibleStates;
        }
        int index = policy.States.IndexOf(s);
        if (s.action != Action.None)
        {
            if (index + grid.height < policy.States.Count && policy.States[index + grid.height].action != Action.None)
            {
                State tmp = new State();
                tmp.value = policy.States[index + grid.height].value;
                tmp.reward = policy.States[index + grid.height].reward;
                tmp.action = Action.Top;
                possibleStates.Add(tmp);
            }
            if (index - grid.height >= 0 && policy.States[index - grid.height].action != Action.None)
            {
                State tmp = new State();
                tmp.value = policy.States[index - grid.height].value;
                tmp.reward = policy.States[index - grid.height].reward;
                tmp.action = Action.Down;
                possibleStates.Add(tmp);
            }
            if ((index + 1) % grid.width != 0 && policy.States[index + 1].action != Action.None)
            {
                State tmp = new State();
                tmp.value = policy.States[index + 1].value;
                tmp.reward = policy.States[index + 1].reward;
                tmp.action = Action.Right;
                possibleStates.Add(tmp);
            }
            if (index % grid.width != 0 && policy.States[index - 1].action != Action.None)
            {
                State tmp = new State();
                tmp.value = policy.States[index - 1].value;
                tmp.reward = policy.States[index - 1].reward;
                tmp.action = Action.Left;
                possibleStates.Add(tmp);
            }
        }
        return possibleStates;
    }
}
