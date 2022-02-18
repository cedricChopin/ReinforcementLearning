using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using TMPro;

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
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                State state = new State();
                state.reward = 0;
                state.value = 0;
                if (x + y * gridManager.height == nbState - 1)
                {
                    state.reward = 1;
                    state.action = Action.None;
                }

                state.action = (Action)Random.Range(0, 4);
                States.Add(state);

            }
        }

        List<State> test = new List<State>();
        test.Add(States[States.Count - 1]);
        test.Add(States[States.Count - 3]);
        test.Add(States[States.Count - 6]);

        //ValueIteration(States[States.Count - 2], test);
        
    }
    public void PolicyIteration()
    {
        policyStable = false;
        for (int x = 0; x < gridManager.width; x++)
        {
            
            for (int y = 0; y < gridManager.height; y++)
            {
                States[x + y * gridManager.height].reward = 0f;
                States[x + y * gridManager.height].value = 0f;
                States[x + y * gridManager.height].action = (Action)Random.Range(0, 4);
                if (gridManager.GetTileAtPosition(new Vector2(x, y)).renderer.color == Color.green)
                {
                    States[x + y * gridManager.height].reward = 1;
                    States[x + y * gridManager.height].action = Action.None;
                }
                else if (gridManager.GetTileAtPosition(new Vector2(x, y)).renderer.color == Color.red)
                {
                    States[x + y * gridManager.height].reward = -1;
                    States[x + y * gridManager.height].action = Action.None;
                }

            }
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

    public void ChangeGrid()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Tile tile = gridManager.GetTileAtPosition(new Vector2(x, y));
                if (States[x + y * gridManager.height].action == Action.Left)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "← \n" + States[x + y * gridManager.height].value.ToString("N3");
                else if (States[x + y * gridManager.height].action == Action.Right)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "→ \n" + States[x + y * gridManager.height].value.ToString("N3");
                else if (States[x + y * gridManager.height].action == Action.Top)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "↑ \n" + States[x + y * gridManager.height].value.ToString("N3");
                else if (States[x + y * gridManager.height].action == Action.Down)
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = "↓ \n" + States[x + y * gridManager.height].value.ToString("N3");
                else
                    tile.GetComponentInChildren<TextMeshProUGUI>().text = States[x + y * gridManager.height].value.ToString("N3");
            }
        }
    }

    void PolicyEvaluation()
    {
        int iteration = 0;
        do
        {
            delta = 0f;
            for (int i = 0; i < States.Count; i++)
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
        for (int i = 0; i < States.Count; i++)
        {
            Action temp = States[i].action;
            if (temp != Action.None)
            {
                States[i].action = getBestAction(i);
                if (temp != States[i].action)
                {
                    policyStable = false;
                }
            }
        }
    }
    Action getBestAction(int indexState)
    {
        float bestReward = -1;
        Action bestAction = Action.None;
        
        if (indexState % gridManager.width != 0 && bestReward < States[indexState - 1].reward + y * States[indexState - 1].value)
        {
            bestReward = States[indexState - 1].reward + y * States[indexState - 1].value;
            bestAction = Action.Left;
        }
        if ((indexState + 1)  % gridManager.width != 0 && bestReward < States[indexState + 1].reward + y * States[indexState + 1].value)
        {
            bestReward = States[indexState + 1].reward + y * States[indexState + 1].value;
            bestAction = Action.Right;
        }
        if (indexState - gridManager.height  >= 0 && bestReward < States[indexState - gridManager.height].reward + y * States[indexState - gridManager.height].value)
        {
            bestReward = States[indexState - gridManager.height].reward + y * States[indexState - gridManager.height].value;
            bestAction = Action.Down;
        }
        if (indexState + gridManager.height < States.Count && bestReward < States[indexState + gridManager.height].reward + y * States[indexState + gridManager.height].value)
        {
            bestReward = States[indexState + gridManager.height].reward + y * States[indexState + gridManager.height].value;
            bestAction = Action.Top;
        }
        Assert.IsTrue(bestAction != Action.None);
        return bestAction;
    }

    public void ValueIteration()
    {
        int iteration = 0;
        State currentState = States[0];

        State bestAction = new State();
        State bestOf = new State();
        do{

            delta = 0;
            for (int i = 0; i < States.Count - 1; i++)
            {
                List<State> possibleState = new List<State>();
                possibleState = GetPossibleActions(States[i]);
                float tmp = currentState.value;
                currentState.value = GetMaximumReward(possibleState, bestOf);
                delta = Mathf.Max(delta, Mathf.Abs(tmp - currentState.value));

                if (currentState.value > bestAction.value)
                {
                    bestAction = bestOf;
                    currentState = bestAction;
                }
                Debug.Log($"valeur case {i} : " + tmp);
            }
            iteration ++;
            Debug.Log("delta : " + delta);
        } while (delta > theta && iteration < 10000);
    }

    float GetMaximumReward(List<State> states, State bestAction)
    {
        float value_reward = -10;
        foreach (State s in states)
        {
            value_reward = (value_reward< s.value+s.reward) ? y*s.value+s.reward : value_reward;
            bestAction = s;
        }

        return value_reward;
    }

    List<State> GetPossibleActions(State s)
    {
        List<State> possibleStates = new List<State>();
        int index = States.IndexOf(s);
        switch (s.action)
        {
            case Action.Top:
                if (index + gridManager.height < States.Count)
                {
                    States[index + gridManager.height].action = Action.Top;
                    possibleStates.Add(States[index + gridManager.height]);
                }
                break;
            case Action.Down:
                if (index - gridManager.height >= 0)
                {
                    States[index + gridManager.height].action = Action.Down;
                    possibleStates.Add(States[index - gridManager.height]);
                }
                break;
            case Action.Right:
                if ((index + 1) % gridManager.width != 0)
                {
                    States[index + 1].action = Action.Right;
                    possibleStates.Add(States[index + 1]);
                }
                break;
            case Action.Left:
                if (index % gridManager.width != 0)
                {
                    States[index - 1].action = Action.Right;
                    possibleStates.Add(States[index - 1]);
                }
                break;
        }

        return possibleStates;
    }

}
