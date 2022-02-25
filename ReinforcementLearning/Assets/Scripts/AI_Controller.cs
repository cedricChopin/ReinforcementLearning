using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AI_Controller : MonoBehaviour
{
    public List<Vector2> way;
    public Strategy_Policy policy;
    public GridManager grid;
    private Vector2 currentPos;

    [SerializeField]
    private bool activated = false;

    float time = 2f;
    int i = 0;
    bool move = false;

    public void Start()
    {
        transform.position = grid.GetTileAtPosition(new Vector2(0,0)).transform.position;
        currentPos = Vector2.zero;
    }

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
    public  virtual State getNextState(State actualState, int x) { return null; }

    public virtual Action getBestAction(int indexState){ return Action.None; }

    public virtual List<State> GetPossibleActions(State s) { return null; }

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

    
    public void ActivatedAI()
    {
        LaunchAI();
        i = 0;
        activated = true;
        move = true;
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
}
