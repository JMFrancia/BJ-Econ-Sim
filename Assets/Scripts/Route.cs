﻿using UnityEngine;
using System.Collections.Generic;
using System;


//Hooks never getting called (including event listeners) because Route never attached to GO / use RouteListEntry or set from RouteListEntry or redesign
public class Route : MonoBehaviour
{
 
    public string Name { get; private set; }
    public int Rarity { get; private set; }
    public int Distance { get; private set; }
    public int Resources { get; private set; }
    public int WorkersAssigned { get; private set; }
    public int WorkerCapacity { get; private set; }

    int travelTime;

    public Action OnChange;

    private void OnEnable()
    {
        Debug.Log("Forager arrival event listener set on route " + GetInstanceID());
        EventManager.StartListening(EventNames.FORAGER_ARRIVING_AT_RESOURCE, OnForagerArrival);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.FORAGER_ARRIVING_AT_RESOURCE, OnForagerArrival);
    }

    public Route(string name, int rarity, int distance, int resources, int workerCapcity) {
        Name = name;
        Rarity = rarity;
        Distance = distance;
        Resources = resources;
        WorkerCapacity = workerCapcity;

        travelTime = ControlManager.Times.TravelTime * Distance;
    }

    public bool AddForager() {
        if (WorkersAssigned < WorkerCapacity && ResourceManager.instance.RemoveWorker())
        {
            WorkersAssigned++;

            //ScheduleManager.instance.AddScheduleItem<Route>(travelTime, EventNames.FORAGER_ARRIVING_AT_RESOURCE, this, $"Forager arrived at route {Name}");
            ScheduleManager.instance.AddScheduleItem<int>(travelTime, EventNames.FORAGER_ARRIVING_AT_RESOURCE, GetInstanceID(), $"Forager arrived at route {Name}");

            OnChange();
            return true;
        }
        return false;
    }

    public bool RemoveForager()
    {
        if (WorkersAssigned > 0)
        {
            WorkersAssigned--;
            ResourceManager.instance.AddWorker();
            OnChange();
            return true;
        }
        return false;
    }

    void OnForagerArrival(int id) {
        Debug.Log("Forager arrival detected with id " + id + " compared to local ID " + GetInstanceID());

        if (id != GetInstanceID())
            return;

        ScheduleManager.instance.AddScheduleItem<int>(ControlManager.Times.ForageTime, EventNames.FORAGER_DEPARTING_FROM_RESOURCE, GetInstanceID(), $"Forager has completed foraging route {Name}, returning to hive");   
    }

    void OnForagingComplete(int id) {
        if (id != GetInstanceID())
            return;

        Resources = Mathf.Max(Resources - ControlManager.Quantities.GatherRate, 0);
        ScheduleManager.instance.AddScheduleItem<int>(travelTime, EventNames.FORAGER_RETURNING_TO_HIVE, GetInstanceID(), $"Forager returning to hive from {Name}");    
    }
}
