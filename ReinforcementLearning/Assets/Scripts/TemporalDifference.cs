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
        float res = 0f;

        foreach (float value in actionValue)
        {
            if (value == max)
                res = 1 - epsilon + epsilon / actionValue.Count;
            else
                res = epsilon / actionValue.Count;
            proba.Add(res);
        }

        float random = Random.Range(0.001f, 1);
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
                possibleActions[x].Add(controller.GetPossibleActions(grid.States[x][y], grid.States).Select(x => (x.action, Random.Range(0.01f, 0.99f))).ToList());
            }
        }
    }

    public void SARSA() {
        //TODO
        generate_policy();
        State nextState;
        Action nextAction;
        int x;
        int y;
        int xN;
        int yN;
        for(int i  = 0; i < nbIt; i++)
        {
            State current_state;
            do
            {
                int randStateWidth = Random.Range(0, grid.width);
                int randStateHeight = Random.Range(0, grid.height);
                current_state = grid.States[randStateWidth][randStateHeight];
            } while (current_state.action == Action.None);
            Action current_action = E_greedy(current_state);
            int nbAction = 0;
            while(current_state.action != Action.Win && nbAction < 100)
            {
                nextState = controller.getNextState(current_state, current_action);
                nextAction = E_greedy(nextState);
                x = (int)current_state.pos.x;
                y = (int)current_state.pos.y;

                xN = (int)nextState.pos.x;
                yN = (int)nextState.pos.y;
                float currentValue = possibleActions[x][y].Where(a => a.Item1 == current_action).ToList()[0].Item2;
                float nextValue = possibleActions[xN][yN].Where(a => a.Item1 == nextAction).ToList()[0].Item2;
                float newValue = currentValue  + alpha * (nextState.reward + gamma * nextValue - currentValue);
                possibleActions[x][y].Where(a => a.Item1 == current_action).ToList()[0] = (current_action, newValue);
                current_state = nextState;
                current_action = nextAction;
                nbAction++;
            }
        }
        for (x = 0; x < grid.width; x++)
        {
            for (y = 0; y < grid.height; y++)
            {
                var bestAction = possibleActions[x][y].OrderByDescending(x => x.Item2).First();
                grid.States[x][y].action = bestAction.Item1;
                grid.States[x][y].value = bestAction.Item2;
            }
        }
                grid.ChangeGrid();
    }

    public void QLearning() {
        //TODO
    }
}
