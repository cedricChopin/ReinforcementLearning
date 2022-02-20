using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    public List<Vector2> way;
    public Strategy_Policy policy;
    public GridManager grid;

    [SerializeField]
    private bool activated = false;

    float time = 2f;
    int i = 0;

    private void Start()
    {
        transform.position = grid.GetTileAtPosition(new Vector2(0,0)).transform.position;
    }

    public void LaunchAI()
    {
        way = new List<Vector2>();
        State currentState =  policy.States[0];
        Vector2 currentPos = Vector2.zero;

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

    public void ActivatedAI()
    {
        LaunchAI();
        activated = true;
    }

    private void Update()
    {
        if (activated == true)
        {
            time -= Time.deltaTime;
            if (time < 0 && i < way.Count)
            {
                transform.position = way[i];
                time = 2f;
                i++;
            }
            if (i >= way.Count) activated = false;
        }
    }
}
