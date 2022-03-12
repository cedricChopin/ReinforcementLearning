using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AI_Controller : MonoBehaviour
{
    public List<Vector2> way; // Liste contenant le chemin à suivre
    public Strategy_Policy policy; //Script contenant PolicyIteration et ValueIteration
    public GridManager grid; //Gestion de la grille
    protected Vector2 currentPos; //Position actuelle de Madeline

    [SerializeField]
    protected bool activated = false; //True si elle a encore des actions à effectuée

    protected float time = 1f; //Temps entre deux mouvements
    protected int i = 0;
    protected bool move = false;

    public void Start()
    {
        way = new List<Vector2>();
        transform.position = grid.GetTileAtPosition(new Vector2(0,0)).transform.position;
        currentPos = Vector2.zero;
    }

    /*public void LateUpdate()
    {
        if (activated == true)
        {
            time -= Time.deltaTime;
            if (move)
                transform.position = Vector3.Lerp(transform.position, new Vector3(way[i].x, way[i].y, 0), Time.deltaTime * 3);

            if (Vector3.Distance(transform.position, way[i]) < 0.02 && move)
                move = false;
            if (time < 0 && i < way.Count)
            {
                //transform.position = way[i];
                time = 1f;
                i++;
                move = true;

            }
            if (i >= way.Count) activated = false;
        }
    }*/

    /// <summary>
    /// Permet d'activer Madeline pour qu'elle avance dans le monde
    /// </summary>
    public virtual void ActivatedAI()
    {
        
    }

    /// <summary>
    /// Calcul la trajectoire à prendre
    /// </summary>
    public virtual void LaunchAI()
    {
        return; 
    }
    /// <summary>
    /// Retourne l'etat suivant
    /// </summary>
    /// <param name="actualState">Etat actuel</param>
    /// <param name="x">Indice de l'etat actuel dans la liste d'etats</param>
    /// <returns></returns>
    public  virtual State getNextState(State actualState, Action action, ref List<List<State>> lstState, ref Dictionary<GameObject, Vector2> lstCaisse) { return null; }

    /// <summary>
    /// Retourne la meilleure action
    /// </summary>
    /// <param name="indexState">Indice de l'etat actuel</param>
    /// <returns></returns>
    public virtual Action getBestAction(State state, List<List<State>> lstState) { return Action.None; }

    /// <summary>
    /// Retourne les actions possibles à partir de l'etat actuel
    /// </summary>
    /// <param name="s">Etat actuel</param>
    /// <returns></returns>
    public virtual List<State> GetPossibleActions(State s, List<List<State>> lstState) { return null; }

    /// <summary>
    /// Renvoie si l'action est possible à l'indice donné
    /// </summary>
    /// <param name="action">Action probable</param>
    /// <param name="index">Indice de l'etat actuel</param>
    /// <returns></returns>
    public virtual bool isPossibleAction(State state, Action action, List<List<State>> lstState) { return false; }

    public virtual bool isWin(State state, Dictionary<GameObject, Vector2> lstCaisse)
    {
        return false;
    }

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
            if (value_reward < policy.gamma * s.value + s.reward)
            {
                value_reward = policy.gamma * s.value + s.reward;
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
    public float calculateValue(State state)
    {
        return state.reward + policy.gamma * state.value;
    }

    /// <summary>
    /// Permet la copie d'un etat
    /// </summary>
    /// <param name="index">Indice de l'etat a copié</param>
    /// <returns></returns>
    public State copyState(State state)
    {
        State res = new State();
        res.value = state.value;
        res.reward = state.reward;
        res.hasCaisse = state.hasCaisse;
        res.action = state.action;
        res.pos = state.pos;
        return res;
    }

    
    

    

    
}
