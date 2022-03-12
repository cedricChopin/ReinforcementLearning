using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemporalDifference : MonoBehaviour
{
    public float gamma = 0.9f;
    
    public int nbIt = 1000;
    [SerializeField] private GridManager grid;

    [SerializeField] AI_Controller controller;
    private List<List<List<(Action, float)>>> possibleActions;

    public float epsilon = 0.1f;
    public float alpha = 0.5f;

    //du sel coule sur mes larmes
    private Action E_greedy(State state)
    {
        Action action = Action.None;

        int x = (int)state.pos.x;
        int y = (int)state.pos.y;
        List<float> actionValue = new List<float>();
        List<float> proba = new List<float>();

        foreach(var ac in possibleActions[x][y])
        {
            actionValue.Add(ac.Item2);
        }
        float max = actionValue.Max();
        float res;

        foreach (float value in actionValue)
        {
            if (value == max)
                res = 1 - epsilon + epsilon / actionValue.Count;
            else
                res = epsilon / actionValue.Count;
            proba.Add(res);
        }

        float random = Random.Range(0f, 0.99f);
        float cumul = 0f;

        //ready for spaghetti code ?
        for(int i = 0; i < proba.Count; i++)
        {
            cumul += proba[i];
            if(random < cumul)
            {
                action = possibleActions[x][y][i].Item1;
                break;
            }
        }

        return action;
    }

    private void generate_policy()
    {
        possibleActions = new List<List<List<(Action, float)>>>();
        for (int x = 0; x < grid.width; x++)
        {
            possibleActions.Add(new List<List<(Action, float)>>());
            for (int y = 0; y < grid.height; y++)
            {
                List<(Action, float)> actions = new List<(Action, float)>();
                actions.Add((Action.Top, Random.Range(-2f, 2f)));
                actions.Add((Action.Down, Random.Range(-2f, 2f)));
                actions.Add((Action.Left, Random.Range(-2f, 2f)));
                actions.Add((Action.Right, Random.Range(-2f, 2f)));
                possibleActions[x].Add(actions);
            }
        }
    }

    public void SARSA()
    {
        grid.InitGrid();
        generate_policy();

        State nextState;
        Action nextAction;
        int x;
        int y;
        int xN;
        int yN;
        for (int i = 0; i < nbIt; i++)
        {
            State current_state;
            do
            {
                int randStateWidth = Random.Range(0, grid.width);
                int randStateHeight = Random.Range(0, grid.height);
                current_state = grid.States[randStateWidth][randStateHeight];
            } while (current_state.action == Action.None);

            Action current_action = E_greedy(current_state);

            while (current_state.action != Action.Win)
            {
                x = (int)current_state.pos.x;
                y = (int)current_state.pos.y;
                nextState = controller.getNextState(current_state, current_action);

                if (nextState == null)
                {
                    possibleActions[x][y][(int)current_action] = (current_action, -2f);
                    current_action = E_greedy(current_state);
                    continue;
                }
                nextAction = E_greedy(nextState);


                xN = (int)nextState.pos.x;
                yN = (int)nextState.pos.y;
                float currentValue = possibleActions[x][y][(int)current_action].Item2;
                float nextValue = possibleActions[xN][yN][(int)nextAction].Item2;
                float newValue = currentValue + alpha * (nextState.reward + gamma * nextValue - currentValue);
                possibleActions[x][y][(int)current_action] = (current_action, newValue);
                current_state = nextState;
                current_action = nextAction;
            }
        }
        for (x = 0; x < grid.width; x++)
        {
            for (y = 0; y < grid.height; y++)
            {
                if (grid.States[x][y].action != Action.None && grid.States[x][y].action != Action.Win)
                {
                    var bestAction = possibleActions[x][y].OrderByDescending(x => x.Item2).First();
                    grid.States[x][y].action = bestAction.Item1;
                    grid.States[x][y].value = bestAction.Item2;
                }
            }
        }
        grid.ChangeGrid();
    }

    public void QLearning() {
        grid.InitGrid();
        generate_policy();

        State nextState;
        int x;
        int y;
        int xN;
        int yN;

        for (int i = 0; i < nbIt; i++)
        {
            State current_state;
            do
            {
                int randStateWidth = Random.Range(0, grid.width);
                int randStateHeight = Random.Range(0, grid.height);
                current_state = grid.States[randStateWidth][randStateHeight];
            } while (current_state.action == Action.None);

            while (current_state.action != Action.Win)
            {
                x = (int)current_state.pos.x;
                y = (int)current_state.pos.y;
                Action current_action = E_greedy(current_state);
                nextState = controller.getNextState(current_state, current_action);

                if (nextState == null)
                {
                    possibleActions[x][y][(int)current_action] = (current_action, -2f);
                    current_action = E_greedy(current_state);
                    continue;
                }

                xN = (int)nextState.pos.x;
                yN = (int)nextState.pos.y;
                float currentValue = possibleActions[x][y][(int)current_action].Item2;
                var bestAction = possibleActions[xN][yN].OrderByDescending(x => x.Item2).First();
                float nextValue = bestAction.Item2;
                float newValue = currentValue + alpha * (nextState.reward + gamma * nextValue - currentValue);
                possibleActions[x][y][(int)current_action] = (current_action, newValue);
                current_state = nextState;
            }
        }
        for (x = 0; x < grid.width; x++)
        {
            for (y = 0; y < grid.height; y++)
            {
                if (grid.States[x][y].action != Action.None && grid.States[x][y].action != Action.Win)
                {
                    var bestAction = possibleActions[x][y].OrderByDescending(x => x.Item2).First();
                    grid.States[x][y].action = bestAction.Item1;
                    grid.States[x][y].value = bestAction.Item2;
                }
            }
        }
        grid.ChangeGrid();
    }
}
