using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// Actions possibles dans le monde
/// </summary>
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

    public bool policyStable = false;
    public List<State> States;
    public List<float> rewards; // Liste des rewards, sert au debug
    public List<float> values; // Liste des values, sert au debug
    public List<Action> actions; // Liste des actions, sert au debug
    public float y = 0.75f; //Gamma

    public float delta = 0f; 
    public float theta = 0.01f;
    public GameObject Tiles; // Toutes les tuiles
    private GridManager gridManager; //Gestion de la grille

    [SerializeField] GameObject AI_controller;
    private AI_Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = Tiles.GetComponent<GridManager>();
        controller = AI_controller.GetComponent<AI_Controller>();
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

    /// <summary>
    /// Algorithme de PolicyIteration
    /// </summary>
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


    
    /// <summary>
    /// Evaluation de la Policy
    /// </summary>
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
    /// <summary>
    /// Amélioration des actions
    /// </summary>
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
    
    /// <summary>
    /// Algorithme de ValueIteration
    /// </summary>
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
