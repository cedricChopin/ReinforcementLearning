using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Strategy_Policy : MonoBehaviour
{

    public List<AI_Type.Action> WorldStates;
    public int nbState = 16;

    public bool policyStable = false;
    public List<float> States;
    public List<AI_Type.Action> PiStates;
    public float y = 0.75f;
    // Start is called before the first frame update
    void Start()
    {


        States = new List<float>();
        PiStates = new List<AI_Type.Action>();
        for (int i = 0; i < nbState; i++)
        {
            States.Add(0);
            PiStates.Add((AI_Type.Action)Random.Range(0, 4));
        }
        States[nbState - 1] = 1;
        PolicyEvaluation();
        PolicyImprovement();
    }

    void PolicyEvaluation()
    {
        float delta = 0f;
        do
        {
            for (int i = 0; i < nbState; i++)
            {
                float tmp = States[i];
                float rewardNextStep = 0;
                switch (PiStates[i])
                {
                    case AI_Type.Action.Down:
                        if (i + 4 < nbState - 1)
                            rewardNextStep = States[i + 4];
                        break;
                    case AI_Type.Action.Top:
                        if (i - 4 >= 0)
                            rewardNextStep = States[i - 4];
                        break;
                    case AI_Type.Action.Right:
                        if (i + 1 < nbState - 1)
                            rewardNextStep = States[i + 1];
                        break;
                    case AI_Type.Action.Left:
                        if (i - 1 >= 0)
                            rewardNextStep = States[i - 1];
                        break;

                }
                States[i] = rewardNextStep + y * getPathValue(i, 100);
                delta = Mathf.Max(delta, Mathf.Abs(tmp - States[i]));

            }
        } while (delta < 0);
    }
    float getPathValue(int indexState, int nbIterMax)
    {
        if(indexState == nbState - 1 || nbIterMax == 0)
        {
            return States[indexState];
        }
        else
        {
            Debug.Log(PiStates[indexState]);
            switch (PiStates[indexState])
            {
                case AI_Type.Action.Down:
                    if (indexState + 4 < nbState - 1)
                        return States[indexState] + getPathValue(indexState + 4, --nbIterMax);
                    else
                    {
                        return States[indexState];
                    }
                case AI_Type.Action.Top:
                    if (indexState - 4 >= 0)
                        return States[indexState] + getPathValue(indexState - 4, --nbIterMax);
                    else
                    {
                        return States[indexState];
                    }
                case AI_Type.Action.Right:
                    if (indexState + 1 < nbState - 1)
                        return States[indexState] + getPathValue(indexState + 1, --nbIterMax);
                    else
                    {
                        return States[indexState];
                    }
                case AI_Type.Action.Left:
                    if (indexState - 1 >= 0)
                        return States[indexState] + getPathValue(indexState - 1, --nbIterMax);
                    else
                    {
                        return States[indexState];
                    }
                default:
                    return States[indexState];

            }
            return States[indexState];
        }
    }

    List<AI_Type.Action> PolicyImprovement()
    {
        policyStable = true;
        Debug.Log("Training");
        for (int i = 0; i < nbState; i++)
        {
            AI_Type.Action temp = PiStates[i];
            PiStates[i] = getBestAction(i);
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
        float bestReward = 0;
        AI_Type.Action bestAction = AI_Type.Action.Down;
        if (indexState - 1 >= 0 && bestReward < States[indexState - 1])
        {
            bestReward = States[indexState - 1];
            bestAction = AI_Type.Action.Left;
        }
        if (indexState + 1 < nbState && bestReward < States[indexState + 1])
        {
            bestReward = States[indexState + 1];
            bestAction = AI_Type.Action.Right;
        }
        if (indexState - 4 >= 0 && bestReward < States[indexState - 4])
        {
            bestReward = States[indexState - 4];
            bestAction = AI_Type.Action.Down;
        }
        if (indexState + 4 < nbState && bestReward < States[indexState + 4])
        {
            bestReward = States[indexState + 4];
            bestAction = AI_Type.Action.Top;
        }

        return bestAction;
    }

}
