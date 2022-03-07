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
    public float gamma = 0.75f; //Gamma

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
            for (int x = 0; x < gridManager.width; x++)
            {
                for (int y = 0; y < gridManager.height; y++)
                {
                    float tmp = gridManager.States[x][y].value;

                    State NextState = controller.getNextState(gridManager.States[x][y], gridManager.States[x][y].action);

                    if (NextState != null)
                    {
                        gridManager.States[x][y].value = NextState.reward + gamma * NextState.value;
                        delta = Mathf.Max(delta, Mathf.Abs(tmp - gridManager.States[x][y].value));
                    }
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
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Action temp = gridManager.States[x][y].action;
                if (temp != Action.None && temp != Action.Win)
                {
                    gridManager.States[x][y].action = controller.getBestAction(gridManager.States[x][y], gridManager.States);
                    if (temp != gridManager.States[x][y].action)
                    {
                        policyStable = false;
                    }
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
            for (int x = 0; x < gridManager.width; x++)
            {
                for (int y = 0; y < gridManager.height; y++)
                {

                    float tmp = gridManager.States[x][y].value;
                    List<State> possibleState = controller.GetPossibleActions(gridManager.States[x][y], gridManager.States);
                    if (possibleState.Count > 0)
                        (gridManager.States[x][y].value, gridManager.States[x][y].action) = controller.GetMaximumReward(possibleState);
                    delta = Mathf.Max(delta, Mathf.Abs(tmp - gridManager.States[x][y].value));
                }
            }
        } while (delta > theta);

        gridManager.ChangeGrid();
    }

    

    

}
