using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Strategy_Policy : MonoBehaviour
{

    public List<AI_Type.Action> WorldStates;
    public int nbState = 16;

    public bool policyStable = false;
    public List<AI_Type.State> States;
    public List<AI_Type.Action> PiStates;
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
            if (i == nbState - 1)
            {
                state.totalReward = 1;
            }
            state.totalReward = 0;
            state.InitialAction = (AI_Type.Action)Random.Range(0, 4);
            States.Add(state);
            
        }
        PolicyEvaluation();
        PolicyImprovement();
    }

    void PolicyEvaluation()
    {
        delta = 0f;
        do
        {
            for (int i = 0; i < States.Count; i++)
            {
                float tmp = States[i].totalReward;
                float rewardNextStep = States[i].totalReward;
                switch (States[i].InitialAction)
                {
                    case AI_Type.Action.Top:
                        if (i + 4 < States.Count - 1)
                            rewardNextStep = States[i + 4].totalReward;
                        break;
                    case AI_Type.Action.Down:
                        if (i - 4 >= 0)
                            rewardNextStep = States[i - 4].totalReward;
                        break;
                    case AI_Type.Action.Right:
                        if (i + 1 < States.Count - 1)
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
            PolicyImprovement();
        } while (delta < theta);
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
            if(temp != PiStates[i])
            {
                policyStable = false;
            }
        }
        if(policyStable == false)
        {
            PolicyEvaluation();
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

    AI_Type.Action ValueIteration(AI_Type.State currentState, List<AI_Type.State> states)
    {
        AI_Type.State bestState = new AI_Type.State();
        bestState.totalReward = 0;
        List<AI_Type.State> newStates = new List<AI_Type.State>();
        while (delta >= theta)
        {
            delta = 0;
            for (int i = 0; i < states.Count; i++)
            {
                AI_Type.State tmp = currentState;
                currentState.totalReward = tmp.totalReward + y * states[i].totalReward;
                tmp.InitialAction = states[i].InitialAction;
                newStates.Add(tmp);
                delta = Mathf.Max(delta, Mathf.Abs(tmp.totalReward - currentState.totalReward));
            }
        }
        
        for(int i = 0; i < newStates.Count; i++)
        {
            if (newStates[i].totalReward >= bestState.totalReward)
                bestState = newStates[i];
        }

        return bestState.InitialAction;
    }

}
