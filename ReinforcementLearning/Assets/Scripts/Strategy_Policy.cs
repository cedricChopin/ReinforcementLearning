using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Strategy_Policy : MonoBehaviour
{

    public List<AI_Type.Action> WorldStates;
    public int nbState = 16;

    public bool policyStable = false;
    public List<AI_Type.State> States;
    public List<AI_Type.Action> PiStates;
    public List<float> rewards;
    public List<AI_Type.Action> actions;
    public float y = 0.75f;

    public float delta = 0f;
    public float theta = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        States = new List<AI_Type.State>();
        for (int i = 0; i < nbState; i++)
        {
            AI_Type.State state = new AI_Type.State();
            state.totalReward = 0;
            if (i == nbState - 1)
            {
                state.value = 1;
                state.totalReward = 1;
            }
            
            state.InitialAction = (AI_Type.Action)Random.Range(0, 4);
            States.Add(state);
            
        }

        List<AI_Type.State> test = new List<AI_Type.State>();
        test.Add(States[States.Count - 1]);
        test.Add(States[States.Count - 3]);
        test.Add(States[States.Count - 6]);

        ValueIteration(States[States.Count - 2], test);
        /*while (policyStable == false)
        {
            PolicyEvaluation();
            PolicyImprovement();
        }*/
        rewards = States.Select(p => p.totalReward).ToList();
        actions = States.Select(p => p.InitialAction).ToList();
    }

    void PolicyEvaluation()
    {
        int iteration = 0;
        do
        {
            delta = 0f;
            for (int i = 0; i < States.Count; i++)
            {
                float tmp = States[i].totalReward;
                float rewardNextStep = 0;
                switch (States[i].InitialAction)
                {
                    case AI_Type.Action.Top:
                        if (i + 4 < States.Count)
                            rewardNextStep = States[i + 4].totalReward;
                        break;
                    case AI_Type.Action.Down:
                        if (i - 4 >= 0)
                            rewardNextStep = States[i - 4].totalReward;
                        break;
                    case AI_Type.Action.Right:
                        if (i + 1 < States.Count)
                            rewardNextStep = States[i + 1].totalReward;
                        break;
                    case AI_Type.Action.Left:
                        if (i - 1 >= 0)
                            rewardNextStep = States[i - 1].totalReward;
                        break;

                }
                AI_Type.State stateTmp = States[i];
                stateTmp.totalReward = tmp + y * rewardNextStep;
                States[i] = stateTmp;
                delta = Mathf.Max(delta, Mathf.Abs(tmp - States[i].totalReward));

            }
            iteration++;
            Debug.Log("Iteration : " + iteration);
        } while (delta > theta && iteration < 6);
    }

    List<AI_Type.Action> PolicyImprovement()
    {
        policyStable = true;
        Debug.Log("Training");
        for (int i = 0; i < nbState; i++)
        {
            AI_Type.Action temp = States[i].InitialAction;
            AI_Type.State best = States[i];
            best.InitialAction = getBestAction(i);
            States[i] = best;
            if(temp != States[i].InitialAction)
            {
                policyStable = false;
            }
        }
        return PiStates;
    }
    AI_Type.Action getBestAction(int indexState)
    {
        float bestReward = -1;
        AI_Type.Action bestAction = AI_Type.Action.Down;
        if (indexState - 1 >= 0 && bestReward < States[indexState - 1].totalReward)
        {
            bestReward = States[indexState - 1].totalReward;
            bestAction = AI_Type.Action.Left;
        }
        if (indexState + 1 < nbState && bestReward < States[indexState + 1].totalReward)
        {
            bestReward = States[indexState + 1].totalReward;
            bestAction = AI_Type.Action.Right;
        }
        if (indexState - 4 >= 0 && bestReward < States[indexState - 4].totalReward)
        {
            bestReward = States[indexState - 4].totalReward;
            bestAction = AI_Type.Action.Down;
        }
        if (indexState + 4 < nbState && bestReward < States[indexState + 4].totalReward)
        {
            bestReward = States[indexState + 4].totalReward;
            bestAction = AI_Type.Action.Top;
        }

        return bestAction;
    }

    public AI_Type.Action ValueIteration(AI_Type.State currentState, List<AI_Type.State> possibleStates)
    {
        int iteration = 0;

        AI_Type.State bestAction = new AI_Type.State();
        do{

            delta = 0;
            for (int i = 0; i < possibleStates.Count - 1; i++)
            {
                float tmp = currentState.value;
                currentState.value = GetMaximumReward(possibleStates);
                delta = Mathf.Max(delta, Mathf.Abs(tmp - currentState.value));
                currentState.InitialAction = possibleStates[i].InitialAction;

                if (currentState.value > bestAction.value)
                {
                    bestAction = currentState;
                }
                Debug.Log($"valeur case {i} : " + tmp);
            }
            iteration ++;
            Debug.Log("delta : " + delta);
        } while (delta > theta && iteration < 10000);
        return bestAction.InitialAction;
    }

    float GetMaximumReward(List<AI_Type.State> states)
    {
        float value_reward = -10;
        foreach (AI_Type.State s in states)
        {
            value_reward = (value_reward< s.value+s.totalReward) ? y*s.value+s.totalReward : value_reward;
        }
        return value_reward;
    }

}
