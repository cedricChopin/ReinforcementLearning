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

    private float epsilon = 0.1f;
    private float alpha = 0.5f;

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
        for(int i = 0; i < possibleActions.Count; i++)
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
            possibleActions[x] = new List<List<(Action, float)>>();
            for (int y = 0; y < grid.height; y++)
            {
                possibleActions[x][y] = controller.GetPossibleActions(grid.States[x][y], grid.States).Select(x => (x.action, Random.Range(0.01f, 0.99f))).ToList();
            }
        }
    }

    public void SARSA() {
        //TODO
        generate_policy();
        
        for(int i  = 0; i < nbIt; i++)
        {

        } 
    }

    public void QLearning() {
        //TODO
    }
}
