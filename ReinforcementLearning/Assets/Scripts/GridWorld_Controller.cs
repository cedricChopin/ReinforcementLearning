using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GridWorld_Controller : AI_Controller
{
    public void LateUpdate()
    {
        if (activated == true)
        {
            time -= Time.deltaTime;
            if (move)
                transform.position = Vector3.Lerp(transform.position, new Vector3(way[i].x, way[i].y, 0), Time.deltaTime * 3);

            if (Vector3.Distance(transform.position, way[i]) < 0.02 && move)
                move = false;
            if (time < 0 && i < way.Count)
            {
                //transform.position = way[i];
                time = 1f;
                i++;
                move = true;

            }
            if (i >= way.Count) activated = false;
        }
    }

    /// <summary>
    /// Calcul la trajectoire à prendre
    /// </summary>
    public override void LaunchAI()
    {
        way = new List<Vector2>();
        currentPos.x = (int)transform.position.x;
        currentPos.y = (int)transform.position.y;
        State currentState = grid.States[(int)currentPos.x][(int)currentPos.y];
        int nbIter = 0;
        while (currentState.action != Action.Win && nbIter < 100)
        {
            switch (currentState.action)
            {
                case Action.Top:
                    currentPos += Vector2.up;
                    break;
                case Action.Down:
                    currentPos += Vector2.down;
                    break;
                case Action.Left:
                    currentPos += Vector2.left;
                    break;
                case Action.Right:
                    currentPos += Vector2.right;
                    break;
            }
            way.Add(currentPos);
            currentState = grid.States[(int)currentPos.x][(int)currentPos.y];
            nbIter++;
        }
    }
    public override void ActivatedAI()
    {
        LaunchAI();
        i = 0;
        activated = true;
        move = true;
    }

    public override State getNextState(State actualState, Action action, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State NextState = null;
        int x = (int)actualState.pos.x;
        int y = (int)actualState.pos.y;
        switch (action)
        {
            case Action.Top:
                if (y + 1 < grid.height)
                    NextState = lstState[x][y + 1];
                break;
            case Action.Down:
                if (y - 1 >= 0)
                    NextState = lstState[x][y - 1];
                break;
            case Action.Right:
                if (x + 1 < grid.width)
                    NextState = lstState[x + 1][y];
                break;
            case Action.Left:
                if (x - 1 >= 0)
                    NextState = lstState[x - 1][y];
                break;

        }

        return NextState;


    }

    public override bool isPossibleAction(State state, Action action, List<List<State>> lstState)
    {
        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        bool isPossible = false;
        switch (action)
        {
            case Action.Top:
                isPossible = y + 1 < grid.height && lstState[x][y + 1].action != Action.None;
                break;
            case Action.Down:
                isPossible = y - 1 >= 0 && lstState[x][y - 1].action != Action.None;
                break;
            case Action.Left:
                isPossible = x - 1 >= 0 && lstState[x - 1][y].action != Action.None;
                break;
            case Action.Right:
                isPossible = x + 1 < grid.width && lstState[x + 1][y].action != Action.None;
                break;

        }

        return isPossible;
    }

    public override Action getBestAction(State state, List<List<State>> lstState)
    {
        float bestReward = float.MinValue;
        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        Action bestAction = Action.None;

        if (isPossibleAction(state, Action.Left, lstState) && bestReward < calculateValue(lstState[x - 1][y]))
        {
            bestReward = calculateValue(lstState[x - 1][y]);
            bestAction = Action.Left;
        }
        if (isPossibleAction(state, Action.Right, lstState) && bestReward < calculateValue(lstState[x + 1][y]))
        {
            bestReward = calculateValue(lstState[x + 1][y]);
            bestAction = Action.Right;
        }
        if (isPossibleAction(state, Action.Down, lstState) && bestReward < calculateValue(lstState[x][y - 1]))
        {
            bestReward = calculateValue(lstState[x][y - 1]);
            bestAction = Action.Down;
        }
        if (isPossibleAction(state, Action.Top, lstState) && bestReward < calculateValue(lstState[x][y + 1]))
        {
            bestAction = Action.Top;
        }
        Assert.IsTrue(bestAction != Action.None);
        return bestAction;
    }

    public override List<State> GetPossibleActions(State s, List<List<State>> lstState)
    {
        List<State> possibleStates = new List<State>();
        int x = (int)s.pos.x;
        int y = (int)s.pos.y;
        if (s.action != Action.None && s.action != Action.Win)
        {
            if (isPossibleAction(s, Action.Top, lstState))
            {
                State tmp = copyState(lstState[x][y + 1]);
                tmp.action = Action.Top;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(s, Action.Down, lstState))
            {
                State tmp = copyState(lstState[x][y - 1]);
                tmp.action = Action.Down;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(s, Action.Right, lstState))
            {
                State tmp = copyState(lstState[x + 1][y]);
                tmp.action = Action.Right;
                possibleStates.Add(tmp);
            }
            if (isPossibleAction(s, Action.Left, lstState))
            {
                State tmp = copyState(lstState[x - 1][y]);
                tmp.action = Action.Left;
                possibleStates.Add(tmp);
            }
        }
        return possibleStates;
    }

    public override bool isWin(State state, Dictionary<GameObject, Vector2> lstCaisse)
    {
        return state.action == Action.Win;
    }
}
