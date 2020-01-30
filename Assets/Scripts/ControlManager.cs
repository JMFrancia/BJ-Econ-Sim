﻿using UnityEngine;
using System.Collections;
using System;

public class ControlManager : MonoBehaviour
{

    public static ControlManager instance { get; private set; }

    //Bad use case to load serialized fields into static vars
    //Probs creating race condition
    //Use singleton
    public TimeControls Times { get { return _times; } }
    public QuantityControls Quantities { get { return _quantities; } }
    public RouteControls Routes { get { return _routes; } }
    public StartingControls StartingValues { get { return _start; } }

    [SerializeField] TimeControls _times;
    [SerializeField] QuantityControls _quantities;
    [SerializeField] RouteControls _routes;
    [SerializeField] StartingControls _start;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [Serializable]
    public class StartingControls
    {
        public int Step = 0;
        public int Workers = 5;
        public int Nectar = 0;
        public int Pollen = 0;
    }

    [Serializable]
    public class TimeControls
    {
        public int SecondsPerStep = 300;
        public int TravelTime = 2;
        public int ForageTime = 5;
    }

    [Serializable]
    public class QuantityControls
    {
        public int GatherRate = 3;
    }

    [Serializable]
    public class RouteControls
    {
        public int MaxDistance = 5;
        public int MinResources = 20;
        public int MaxResources = 50;
        public int RoutesPerForagingFrame = 3;
    }
}
