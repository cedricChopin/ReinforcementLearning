using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using TMPro;

public class MonteCarlo : MonoBehaviour
{

    public float gamma = 0.75f; //Gamma

    public float delta = 0f;
    public float theta = 0.01f;
    public int nbIter = 100;
    public GameObject Tiles; // Toutes les tuiles
    private GridManager gridManager; //Gestion de la grille


    [SerializeField] GameObject AI_controller;
    private AI_Controller controller;
    private List<List<float>> returns;
    private List<List<int>> nbVisit;

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
        returns = new List<List<float>>();
        nbVisit = new List<List<int>>();
        for (int x = 0; x < gridManager.width; x++)
        {
            returns.Add(new List<float>());
            nbVisit.Add(new List<int>());
            for (int y = 0; y < gridManager.height; y++)
            {
                returns[x].Add(0f);
                nbVisit[x].Add(0);
            }
        }
        gridManager.InitGrid();
        for (int nbEpisode = 1; nbEpisode < nbIter; nbEpisode++)
        {
            List<State> episode = generate_random_episode();
            List<State> already_visited = new List<State>();
            for (int t = episode.Count - 2; t > 0; t--)
            {
                if (!already_visited.Contains(episode[t]))
                {
                    already_visited.Add(episode[t]);
                    float G = 0;
                    for (int idx = t; idx < episode.Count; idx++)
                    {
                        G = gamma * (G + episode[idx].value);
                    }

                    returns[(int)episode[t].pos.x][(int)episode[t].pos.y] += G;
                    nbVisit[(int)episode[t].pos.x][(int)episode[t].pos.y] += 1;
                    
                }

            }
        }

        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                if (nbVisit[x][y] != 0)
                {
                    gridManager.States[x][y].value = returns[x][y] / (float)nbVisit[x][y];
                }

            }
        }
        //Met à jour la policy en fonction des values des cases
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                if(gridManager.States[x][y].action != Action.None && gridManager.States[x][y].action != Action.Win)
                    gridManager.States[x][y].action = controller.getBestAction(gridManager.States[x][y], gridManager.States);
            }
        }
        gridManager.ChangeGrid();
    }
    List<State> generate_random_episode()
    {
        List<State> episode = new List<State>();
        bool done = false;
        State current_state;
        do
        {
            int randStateWidth = Random.Range(0, gridManager.width);
            int randStateHeight = Random.Range(0, gridManager.height);
            current_state = gridManager.States[randStateWidth][randStateHeight];
        } while (current_state.action == Action.None);
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


            if (nextState.action == Action.Win)
            {
                done = true;
            }
            episode.Add(nextState);
            current_state = nextState;
        }

        return episode;
    }





}
