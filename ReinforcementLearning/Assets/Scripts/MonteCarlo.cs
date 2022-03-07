using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using TMPro;

public class MonteCarlo : MonoBehaviour
{

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

    State getNextStateAction(State actualState, Action action)
    {
        State nextState = null;



        return nextState;
    }


    public void first_visit_mc()
    {
        gridManager.InitGrid();
        var episode = generate_random_episode();
    }

    List<State> generate_random_episode()
    {
        List<State> episode = new List<State>();
        /*bool done = false;
        int randState = Random.Range(0, gridManager.States.Count);
        State current_state = gridManager.States[randState];
        current_state.value = -1;
        episode.Add(current_state);

        while (!done)
        {
            Action action = (Action)Random.Range(0, 4);
            State nextState;
            if (controller.isPossibleAction(current_state, action, gridManager.States))
                nextState = controller.getNextState(current_state, action);
            else
                nextState = current_state;
            episode.Add(nextState);
            if(nextState.action == Action.Win)
            {
                done = true;
            }
            current_state = nextState;
        }*/

        return episode;
    }

    

    

}
