using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AI_Controller : MonoBehaviour
{
    public List<Vector2> way; // Liste contenant le chemin à suivre
    public Strategy_Policy policy; //Script contenant PolicyIteration et ValueIteration
    public GridManager grid; //Gestion de la grille
    private Vector2 currentPos; //Position actuelle de Madeline

    [SerializeField]
    private bool activated = false; //True si elle a encore des actions à effectuée

    float time = 2f; //Temps entre deux mouvements
    int i = 0;
    bool move = false;

    public void Start()
    {
        transform.position = grid.GetTileAtPosition(new Vector2(0,0)).transform.position;
        currentPos = Vector2.zero;
    }

    public void LateUpdate()
    {
        if (activated == true)
        {
            time -= Time.deltaTime;
            if (move)
                transform.position = Vector3.Lerp(transform.position, new Vector3(way[i].x, way[i].y, 0), Time.deltaTime * time);

            if (Vector3.Distance(transform.position, way[i]) < 0.02 && move)
                move = false;
            if (time < 0 && i < way.Count)
            {
                //transform.position = way[i];
                time = 2f;
                i++;
                move = true;

            }
            if (i >= way.Count) activated = false;
        }
    }

    /// <summary>
    /// Permet d'activer Madeline pour qu'elle avance dans le monde
    /// </summary>
    public void ActivatedAI()
    {
        LaunchAI();
        i = 0;
        activated = true;
        move = true;
    }

    /// <summary>
    /// Calcul la trajectoire à prendre
    /// </summary>
    public void LaunchAI()
    {
        way = new List<Vector2>();
        
        State currentState = policy.States[(int)currentPos.x + (int)currentPos.y * grid.height];

        while(currentState.action != Action.Win)
        {
            switch (currentState.action)
            {
                case Action.Top:
                    currentPos += Vector2.up;
                    break;
                case Action.Down:
                    currentPos += Vector2.down;
                    break;
                case Action.Left:
                    currentPos += Vector2.left;
                    break;
                case Action.Right:
                    currentPos += Vector2.right;
                    break;
            }
            way.Add(currentPos);
            currentState = policy.States[(int)currentPos.x + (int)currentPos.y * grid.height];
        }
    }
    /// <summary>
    /// Retourne l'etat suivant
    /// </summary>
    /// <param name="actualState">Etat actuel</param>
    /// <param name="x">Indice de l'etat actuel dans la liste d'etats</param>
    /// <returns></returns>
    public  virtual State getNextState(State actualState, int x) { return null; }

    /// <summary>
    /// Retourne la meilleure action
    /// </summary>
    /// <param name="indexState">Indice de l'etat actuel</param>
    /// <returns></returns>
    public virtual Action getBestAction(int indexState){ return Action.None; }

    /// <summary>
    /// Retourne les actions possibles à partir de l'etat actuel
    /// </summary>
    /// <param name="s">Etat actuel</param>
    /// <returns></returns>
    public virtual List<State> GetPossibleActions(State s) { return null; }

    /// <summary>
    /// Retourne l'etat possédant la reward la plus haute
    /// </summary>
    /// <param name="states">Liste des actions possibles</param>
    /// <returns></returns>
    public (float, Action) GetMaximumReward(List<State> states)
    {
        Action bestAction = new Action();
        float value_reward = -10;
        foreach (State s in states)
        {
            if (value_reward < policy.y * s.value + s.reward)
            {
                value_reward = policy.y * s.value + s.reward;
                bestAction = s.action;
            }
        }

        return (value_reward, bestAction);
    }

    /// <summary>
    /// Utilisé dans PolicyIteration et ValueIteration, permet de calculer la valeur d'une case
    /// </summary>
    /// <param name="index">Indice de l'etat actuel</param>
    /// <returns></returns>
    public float calculateValue(int index)
    {
        return policy.States[index].reward + policy.y * policy.States[index].value;
    }

    /// <summary>
    /// Permet la copie d'un etat
    /// </summary>
    /// <param name="index">Indice de l'etat a copié</param>
    /// <returns></returns>
    public State copyState(int index)
    {
        State res = new State();
        res.value = policy.States[index + 1].value;
        res.reward = policy.States[index + 1].reward;
        res.hasCaisse = policy.States[index + 1].hasCaisse;
        return res;
    }

    /// <summary>
    /// Renvoie si l'action est possible à l'indice donné
    /// </summary>
    /// <param name="action">Action probable</param>
    /// <param name="index">Indice de l'etat actuel</param>
    /// <returns></returns>
    public bool isPossibleAction(Action action, int index)
    {
        if (index < 0 || index >= policy.States.Count)
            return false;
        bool isPossible = false;
        switch (action)
        {
            case Action.Top:
                isPossible = index + grid.height < policy.States.Count && policy.States[index + grid.height].action != Action.None;
                break;
            case Action.Left:
                isPossible = index % grid.width != 0 && policy.States[index - 1].action != Action.None;
                break;
            case Action.Down:
                isPossible = index - grid.height >= 0 && policy.States[index - grid.height].action != Action.None;
                break;
            case Action.Right:
                isPossible = (index + 1) % grid.width != 0 && policy.States[index + 1].action != Action.None;
                break;

        }

        return isPossible;
    }

    

    
}
