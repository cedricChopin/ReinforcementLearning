using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class Sokoban_Controller : AI_Controller
{
    public override State getNextState(State actualState, Action action, List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State NextState = null;
        int x = (int)actualState.pos.x;
        int y = (int)actualState.pos.y;

        switch (action)
        {
            case Action.Top:
                NextState = GoTop(actualState, lstState, ref lstCaisse);
                break;
            case Action.Down:
                NextState = GoDown(actualState, lstState, ref lstCaisse);
                break;
            case Action.Right:
                NextState = GoRight(actualState, lstState, ref lstCaisse);
                break;
            case Action.Left:
                NextState = GoLeft(actualState, lstState, ref lstCaisse);
                break;

        }

        return NextState;
    }
    public override Action getBestAction(State state, List<List<State>> lstState)
    {
        float bestReward = -100;
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
        // TODO Sokoban
        return null;
    }

    public override bool isPossibleAction(State state, Action action, List<List<State>> lstState)
    {
        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        bool isPossible = false;
        bool canMoveCaisse = false;
        switch (action)
        {
            case Action.Top:
                isPossible = y + 1 < grid.height && lstState[x][y + 1].action != Action.None;
                if (lstState[x][y + 1].hasCaisse)
                { 
                    canMoveCaisse = y + 2 < grid.height && lstState[x][y + 2].action != Action.None && !lstState[x][y + 2].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;
            case Action.Down:
                isPossible = y - 1 >= 0 && lstState[x][y - 1].action != Action.None;
                if (lstState[x][y - 1].hasCaisse)
                {
                    canMoveCaisse = y - 2 >= 0 && lstState[x][y - 2].action != Action.None && !lstState[x][y - 2].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;
            case Action.Left:
                isPossible = x - 1 >= 0 && lstState[x - 1][y].action != Action.None;
                if (lstState[x - 1][y].hasCaisse)
                {
                    canMoveCaisse = x-1 >= 0 && lstState[x - 2][y].action != Action.None && !lstState[x - 2][y].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;
            case Action.Right:
                isPossible = x + 1 < grid.width && lstState[x + 1][y].action != Action.None;
                if (lstState[x + 1][y].hasCaisse)
                {
                    canMoveCaisse = x + 2 < grid.width && lstState[x + 2][y].action != Action.None && !lstState[x + 2][y].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;

        }

        return isPossible;
    }

    public override bool isWin(State state, Dictionary<GameObject, Vector2> lstCaisse)
    {
        var tmp = lstCaisse.Where(a => grid.listWin.Contains(a.Value));


        return tmp.Count() == lstCaisse.Count();
    }



    private State GoLeft(State actualState, List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        if (actualState.pos.x - 1 >= 0)
        {
            Vector2 newPos = new Vector2(actualState.pos.x - 1, actualState.pos.y);

            if (lstState[(int)newPos.x][(int)newPos.y].hasCaisse)
            {
                if (newPos.x - 1 >= 0)
                {
                    if (lstState[(int)newPos.x - 1][(int)newPos.y].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x - 1, newPos.y)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x - 1, newPos.y), lstState, ref lstCaisse);
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return nextState;
    }

    private State GoRight(State actualState, List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        if (actualState.pos.x + 1 < grid.width)
        {
            Vector2 newPos = new Vector2(actualState.pos.x + 1, actualState.pos.y);
            if (lstState[(int)newPos.x][(int)newPos.y].hasCaisse)
            {
                if (newPos.x + 1 < grid.width)
                {
                    if (!lstState[(int)newPos.x + 1][(int)newPos.y].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x + 1, newPos.y)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x + 1, newPos.y),lstState, ref lstCaisse); 
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return nextState;
    }

    private State GoTop(State actualState, List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        if (actualState.pos.y + 1 < grid.height)
        {
            Vector2 newPos = new Vector2(actualState.pos.x, actualState.pos.y + 1);
            if (lstState[(int)newPos.x][(int)newPos.y].hasCaisse)
            {
                if (newPos.y + 1 < grid.height)
                {
                    if (!lstState[(int)newPos.x][(int)newPos.y + 1].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y + 1)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x, newPos.y + 1), lstState, ref lstCaisse);
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;

            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return nextState;
    }

    private State GoDown(State actualState, List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        if (actualState.pos.y - 1 >= 0)
        {
            Vector2 newPos = new Vector2(actualState.pos.x, actualState.pos.y - 1);
            if (lstState[(int)newPos.x][(int)newPos.y].hasCaisse)
            {
                if (newPos.y - 1 >= 0)
                {
                    if (!lstState[(int)newPos.x][(int)newPos.y - 1].hasCaisse && grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y - 1)).rend.color != Color.red)
                    {
                        transform.position = newPos;
                        grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x, newPos.y - 1), lstState, ref lstCaisse);
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return nextState;
    }
}
