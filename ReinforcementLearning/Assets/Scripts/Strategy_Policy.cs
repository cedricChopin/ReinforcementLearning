using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public enum Action
{
    Top,
    Down,
    Left,
    Right,
    None
}
public class Strategy_Policy : MonoBehaviour
{
    
    public List<AI_Type.Action> WorldStates;
    public int nbState;

    public bool policyStable = false;
    public List<State> States;
    public List<float> rewards;
    public List<float> values;
    public List<Action> actions;
    public float y = 0.75f;

    public float delta = 0f;
    public float theta = 0.01f;
    public GameObject Tiles;
    private GridManager gridManager;
    // Start is called before the first frame update
    void Start()
    {
        gridManager = Tiles.GetComponent<GridManager>();
        nbState = gridManager.width * gridManager.height;
        States = new List<State>();
        for (int i = 0; i < nbState; i++)
        {
            State state = new State();
            state.reward = 0;
            state.value = 0;
            
            
            state.action = (Action)Random.Range(0, 4);
            if (i == nbState - 1)
            {
                state.reward = 1;
                state.action = Action.None;
            }
            States.Add(state);
            
        }
        while (policyStable == false)
        {
            PolicyEvaluation();
            PolicyImprovement();
            rewards = States.Select(p => p.reward).ToList();
            actions = States.Select(p => p.action).ToList();
            values = States.Select(p => p.value).ToList();
        }
        
    }

    void PolicyEvaluation()
    {
        
        do
        {
            delta = 0f;
            for (int i = 0; i < States.Count - 1; i++)
            {
                float tmp = States[i].value;
                State NextState = null;
                switch (States[i].action)
                {
                    case Action.Top:
                        if (i + gridManager.height < States.Count)
                        {
                            NextState = States[i + gridManager.height];
                        }
                        break;
                    case Action.Down:
                        if (i - gridManager.height >= 0)
                        {
                            NextState = States[i - gridManager.height];
                        }
                        break;
                    case Action.Right:
                        if ((i +1) % gridManager.width != 0) { 
                            NextState = States[i + 1];
                        }
                        break;
                    case Action.Left:
                        if (i % gridManager.width != 0) { 
                            NextState = States[i - 1];
                        }
                        break;
                    case Action.None:
                        NextState = States[i];
                        break;

                }
                
                if (NextState != null)
                {
                    States[i].value = NextState.reward + y * NextState.value;
                    delta = Mathf.Max(delta, Mathf.Abs(tmp - States[i].value));
                }

            }
        } while (delta >= theta);
    }

    void PolicyImprovement()
    {
        policyStable = true;
        Debug.Log("Training");
        for (int i = 0; i < States.Count - 1; i++)
        {
            Action temp = States[i].action;
            States[i].action = getBestAction(i);
            if (temp != States[i].action)
            {
                policyStable = false;
            }
        }
    }
    Action getBestAction(int indexState)
    {
        float bestReward = -1;
        Action bestAction = Action.None;
        
        if (indexState % gridManager.width != 0 && bestReward < States[indexState - 1].reward + y * States[indexState - 1].value)
        {
            bestReward = States[indexState - 1].value;
            bestAction = Action.Left;
        }
        if ((indexState + 1)  % gridManager.width != 0 && bestReward < States[indexState + 1].reward + y * States[indexState + 1].value)
        {
            bestReward = States[indexState + 1].value;
            bestAction = Action.Right;
        }
        if (indexState - gridManager.height  >= 0 && bestReward < States[indexState - gridManager.height].reward + y * States[indexState - gridManager.height].value)
        {
            bestReward = States[indexState - gridManager.height].value;
            bestAction = Action.Down;
        }
        if (indexState + gridManager.height < States.Count && bestReward < States[indexState + gridManager.height].reward + y * States[indexState + gridManager.height].value)
        {
            bestReward = States[indexState + gridManager.height].value;
            bestAction = Action.Top;
        }
        Debug.Log("Best action for " + indexState + " reward:  " + bestReward + " action: " + bestAction);
        Assert.IsTrue(bestAction != Action.None);
        return bestAction;
    }

    AI_Type.Action ValueIteration(AI_Type.State currentState, List<AI_Type.State> states)
    {
        AI_Type.State bestState = new AI_Type.State();
        bestState.totalReward = 0;
        List<AI_Type.State> newStates = new List<AI_Type.State>();
        while (delta >= theta)
        {
            delta = 0;
            for (int i = 0; i < states.Count; i++)
            {
                AI_Type.State tmp = currentState;
                currentState.totalReward = tmp.totalReward + y * states[i].totalReward;
                tmp.InitialAction = states[i].InitialAction;
                newStates.Add(tmp);
                delta = Mathf.Max(delta, Mathf.Abs(tmp.totalReward - currentState.totalReward));
            }
        }
        
        for(int i = 0; i < newStates.Count; i++)
        {
            if (newStates[i].totalReward >= bestState.totalReward)
                bestState = newStates[i];
        }

        return bestState.InitialAction;
    }

}
