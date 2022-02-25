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
    None,
    Win
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

    [SerializeField] GameObject AI_controller;
    private AI_Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = Tiles.GetComponent<GridManager>();
        controller = AI_controller.GetComponent<AI_Controller>();
        nbState = gridManager.width * gridManager.height;
        States = new List<State>();
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                State state = new State();

                state.action = (Action)Random.Range(0, 4);
                States.Add(state);

            }
        }

    }
    public void PolicyIteration()
    {
        policyStable = false;
        gridManager.InitGrid(ref States);
        while (policyStable == false)
        {
            PolicyEvaluation();
            PolicyImprovement();
            rewards = States.Select(p => p.reward).ToList();
            actions = States.Select(p => p.action).ToList();
            values = States.Select(p => p.value).ToList();
        }
        gridManager.ChangeGrid(ref States);
    }


    

    void PolicyEvaluation()
    {
        do
        {
            delta = 0f;
            for (int i = 0; i < States.Count; i++)
            {
                float tmp = States[i].value;
                
                State NextState = controller.getNextState(States[i], i);
                   
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
            if (temp != Action.None && temp != Action.Win)
            {
                States[i].action = controller.getBestAction(i);
                if (temp != States[i].action)
                {
                    policyStable = false;
                }
            }
        }
    }
    

    public void ValueIteration()
    {
        gridManager.InitGrid(ref States);
        do
        {
            delta = 0;
            for (int i = 0; i < States.Count - 1; i++)
            {

                float tmp = States[i].value;
                List<State> possibleState = controller.GetPossibleActions(States[i]);
                if (possibleState.Count > 0)
                    (States[i].value, States[i].action) = controller.GetMaximumReward(possibleState);
                delta = Mathf.Max(delta, Mathf.Abs(tmp - States[i].value));
            }
        } while (delta > theta);

        gridManager.ChangeGrid(ref States);
    }

    

    

}
