using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    private int reward;
    private List<State_Sokoban> states;

    public void Init()
    {
        states = new List<State_Sokoban>();
        states[0].agentPos = transform.position;
        
        foreach(var caisse in grid.listCaisse)
        {
            states[0].boxLocations.Add(caisse.Value);
        }
        
        reward = 0;
    }

    public void Transition(State_Sokoban s, Action action)
    {
        Vector2 currentPos = s.agentPos;
        Vector2 newPos = Vector2.zero;
        
        switch (action)
        {
            case Action.Left:
                newPos = new Vector2(currentPos.x - 1, currentPos.y);
                break;
            case Action.Right:
                newPos = new Vector2(currentPos.x + 1, currentPos.y);
                break;
            case Action.Top:
                newPos = new Vector2(currentPos.x, currentPos.y + 1);
                break;
            case Action.Down:
                newPos = new Vector2(currentPos.x, currentPos.y - 1);
                break;
        }

        Vector2 dir = newPos - currentPos;
        State_Sokoban newState = new State_Sokoban();

        if (grid.GetTileAtPosition(newPos) != null)
        {
            if (grid.GetTileAtPosition(newPos).gotCaisse)
            {
                if (grid.GetTileAtPosition(newPos + dir) != null && !grid.GetTileAtPosition(newPos + dir).gotCaisse)
                {
                    transform.position = newPos;
                    grid.MoveCaisse(newPos, newPos + dir);
                    newState.agentPos = newPos;
                    newState.boxLocations = s.boxLocations.Where(x => x != newPos).ToList();
                    newState.boxLocations.Add(newPos + dir);
                    newState.goalLocations = s.goalLocations;

                    if (grid.GetTileAtPosition(newPos + dir).rend.color == Color.green)
                        reward += 1000;
                    else reward -= 1;
                }
            }
            else if (grid.GetTileAtPosition(newPos).rend.color != Color.red)
            {
                transform.position = newPos;
                reward -= 1;
                newState.agentPos = newPos;
                newState.boxLocations = s.boxLocations;
                newState.goalLocations = s.goalLocations;
            }
        }
        else
        {
            newState = s;
            reward -= 2;
        }

        states.Add(newState);
    }
}
