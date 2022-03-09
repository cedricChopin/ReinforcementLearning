using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Sokoban_Controller : AI_Controller
{
    public override State getNextState(State actualState, Action action)
    {
        State NextState = null;
        int x = (int)actualState.pos.x;
        int y = (int)actualState.pos.y;

        switch (action)
        {
            case Action.Top:
                if (y + 1 < grid.height)
                {
                    if (!grid.States[x][y + 1].hasCaisse)
                        NextState = grid.States[x][y + 1];
                    else
                    {
                        if (y + 2 < grid.height)
                        {
                            if (!(grid.States[x][y + 2].action == Action.None) &&
                                !(grid.States[x][y + 2].hasCaisse))
                            {
                                grid.MoveCaisse(new Vector2(x, y + 1), new Vector2(x, y + 2));
                                NextState = grid.States[x][y + 1];
                            }
                        }
                    }
                }
                break;
            case Action.Down:
                if (y - 1 >= 0)
                {
                    if (!grid.States[x][y - 1].hasCaisse)
                        NextState = grid.States[x][y - 1];
                    else
                    {
                        if (y - 2 >= 0)
                        {
                            if (!(grid.States[x][y - 2].action == Action.None) &&
                                !(grid.States[x][y - 2].hasCaisse))
                            {
                                grid.MoveCaisse(new Vector2(x, y - 1), new Vector2(x, y - 2));
                                NextState = grid.States[x][y - 1];
                            }
                        }
                    }
                }
                break;
            case Action.Right:
                if (x + 1 < grid.width)
                {
                    if (!grid.States[x+1][y].hasCaisse)
                        NextState = grid.States[x+1][y];
                    else
                    {
                        if (x + 2 < grid.width)
                        {
                            if (!(grid.States[x + 2][y].action == Action.None) &&
                                !(grid.States[x + 2][y].hasCaisse))
                            {
                                grid.MoveCaisse(new Vector2(x+1, y), new Vector2(x+2, y));
                                NextState = grid.States[x+1][y];
                            }
                        }
                    }
                }
                break;
            case Action.Left:
                if (x - 1 >= 0)
                {
                    if (!grid.States[x - 1][y].hasCaisse)
                        NextState = grid.States[x - 1][y];
                    else
                    {
                        if (x - 2 >= 0)
                        {
                            if (!(grid.States[x-2][y].action == Action.None) &&
                                !(grid.States[x-2][y].hasCaisse))
                            {
                                grid.MoveCaisse(new Vector2(x-1, y), new Vector2(x-2, y));
                                NextState = grid.States[x-2][y];
                            }
                        }
                    }
                }
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
}
