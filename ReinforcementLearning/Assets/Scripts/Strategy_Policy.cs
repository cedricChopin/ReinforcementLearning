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
        

    }

    /// <summary>
    /// Algorithme de PolicyIteration
    /// </summary>
    public void PolicyIteration()
    {
        policyStable = false;
        gridManager.InitGrid();
        while (policyStable == false)
        {
            PolicyEvaluation();
            PolicyImprovement();
            rewards = gridManager.States.Select(p => p.reward).ToList();
            actions = gridManager.States.Select(p => p.action).ToList();
            values = gridManager.States.Select(p => p.value).ToList();
        }
        gridManager.ChangeGrid();
    }


    
    /// <summary>
    /// Evaluation de la Policy
    /// </summary>
    void PolicyEvaluation()
    {
        do
        {
            delta = 0f;
            for (int i = 0; i < gridManager.States.Count; i++)
            {
                float tmp = gridManager.States[i].value;
                
                State NextState = controller.getNextState(gridManager.States[i], gridManager.States[i].action);
                   
                if (NextState != null)
                {
                    gridManager.States[i].value = NextState.reward + y * NextState.value;
                    delta = Mathf.Max(delta, Mathf.Abs(tmp - gridManager.States[i].value));
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
        for (int i = 0; i < gridManager.States.Count; i++)
        {
            Action temp = gridManager.States[i].action;
            if (temp != Action.None && temp != Action.Win)
            {
                gridManager.States[i].action = controller.getBestAction(gridManager.States[i], gridManager.States);
                if (temp != gridManager.States[i].action)
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
        gridManager.InitGrid();
        do
        {
            delta = 0;
            for (int i = 0; i < gridManager.States.Count; i++)
            {

                float tmp = gridManager.States[i].value;
                List<State> possibleState = controller.GetPossibleActions(gridManager.States[i], gridManager.States);
                if (possibleState.Count > 0)
                    (gridManager.States[i].value, gridManager.States[i].action) = controller.GetMaximumReward(possibleState);
                delta = Mathf.Max(delta, Mathf.Abs(tmp - gridManager.States[i].value));
            }
        } while (delta > theta);

        gridManager.ChangeGrid();
    }

    

    

}
