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
            public float totalReward;
            public Action InitialAction;
        }
    }