using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class Sokoban_Controller : AI_Controller
{
    [SerializeField]
    private Sokoban sokoban;
    
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
        while (!isWin(currentState, grid.listCaisse) && nbIter < 100)
        {
            Action currentAction = greedy(currentState);
            switch (currentAction)
            {
                case Action.Top:
                    sokoban.GoTop();
                    break;
                case Action.Down:
                    sokoban.GoDown();
                    break;
                case Action.Left:
                    sokoban.GoLeft();
                    break;
                case Action.Right:
                    sokoban.GoRight();
                    break;
            }
            currentPos.x = (int)transform.position.x;
            currentPos.y = (int)transform.position.y;
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

    
    public override (State, float) getNextState(State actualState, Action action, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State NextState = null;
        int x = (int)actualState.pos.x;
        int y = (int)actualState.pos.y;
        float reward = -1f;

        switch (action)
        {
            case Action.Top:
                (NextState, reward) = GoTop(actualState, ref lstState, ref lstCaisse);
                break;
            case Action.Down:
                (NextState, reward) = GoDown(actualState, ref lstState, ref lstCaisse);
                break;
            case Action.Right:
                (NextState, reward) = GoRight(actualState, ref lstState, ref lstCaisse);
                break;
            case Action.Left:
                (NextState, reward) = GoLeft(actualState, ref lstState, ref lstCaisse);
                break;

        }

        return (NextState, reward);
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
                isPossible = y + 1 < grid.height && !grid.isObstacle(lstState[x][y + 1]);
                if (lstState[x][y + 1].hasCaisse)
                { 
                    canMoveCaisse = y + 2 < grid.height && !grid.isObstacle(lstState[x][y + 2]) && !lstState[x][y + 2].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;
            case Action.Down:
                isPossible = y - 1 >= 0 && !grid.isObstacle(lstState[x][y - 1]);
                if (lstState[x][y - 1].hasCaisse)
                {
                    canMoveCaisse = y - 2 >= 0 && !grid.isObstacle(lstState[x][y - 2]) && !lstState[x][y - 2].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;
            case Action.Left:
                isPossible = x - 1 >= 0 && !grid.isObstacle(lstState[x - 1][y]);
                if (lstState[x - 1][y].hasCaisse)
                {
                    canMoveCaisse = x - 2 >= 0 && !grid.isObstacle(lstState[x - 2][y]) && !lstState[x - 2][y].hasCaisse;
                    isPossible = isPossible && canMoveCaisse;
                }
                break;
            case Action.Right:
                isPossible = x + 1 < grid.width && !grid.isObstacle(lstState[x + 1][y]);
                if (lstState[x + 1][y].hasCaisse)
                {
                    canMoveCaisse = x + 2 < grid.width && !grid.isObstacle(lstState[x + 2][y]) && !lstState[x + 2][y].hasCaisse;
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



    private (State, float) GoLeft(State actualState, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        float reward = -1f;
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
                        reward = grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x - 1, newPos.y), ref lstState, ref lstCaisse);
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return (nextState, reward);
    }

    private (State, float) GoRight(State actualState, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        float reward = -1f;
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
                        reward = grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x + 1, newPos.y), ref lstState, ref lstCaisse); 
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return (nextState, reward);
    }

    private (State, float) GoTop(State actualState, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        float reward = -1f;
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
                        reward = grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x, newPos.y + 1), ref lstState, ref lstCaisse);
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;

            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return (nextState, reward);
    }

    private (State, float) GoDown(State actualState, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse)
    {
        State nextState = null;
        float reward = -1f;
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
                        reward = grid.SimulateMoveCaisse(newPos, new Vector2(newPos.x, newPos.y - 1), ref lstState, ref lstCaisse);
                    }
                }
            }
            else if (grid.GetTileAtPosition(new Vector2(newPos.x, newPos.y)).rend.color != Color.red)
                transform.position = newPos;
            nextState = lstState[(int)newPos.x][(int)newPos.y];
        }
        return (nextState, reward);
    }
}
