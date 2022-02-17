using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

    public class AI_Type
    {
        public enum Action
        {
            Top,
            Down,
            Left,
            Right
        }

        public struct State
        {
            public Vector2 pos;
            public float distanceToGoal;
            public List<nextState> nextState;
            public float totalReward;
            public Action InitialAction;
        }

        public struct nextState
        {
            public State s;
            public float reward;
        }
    }