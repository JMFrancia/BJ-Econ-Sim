using UnityEngine;
using System.Collections.Generic;
using System;


public class Route
{
    public string Name { get; private set; }
    public int Rarity { get; private set; }
    public int Distance { get; private set; }
    public int Resources { get; private set; }
    public int WorkersAssigned { get; private set; }
    public int WorkerCapacity { get; private set; }
    public bool Depleted { get; private set; }

    public Action OnChange;

    public Route(string name, int rarity, int distance, int resources, int workerCapcity) {
        Name = name;
        Rarity = rarity;
        Distance = distance;
        Resources = resources;
        WorkerCapacity = workerCapcity;
        Depleted = false;

        //travelTime = ControlManager.Times.TravelTime * Distance;
    }

    public bool HasCapacity() {
        return WorkersAssigned < WorkerCapacity;
    }

    public bool AddForager() {
        if(WorkersAssigned < WorkerCapacity)
        //if (WorkersAssigned < WorkerCapacity && ResourceManager.instance.RemoveWorker())
        {
            WorkersAssigned++;
            //ForagerManager.instance.AddForager(this));
            //ScheduleManager.instance.AddScheduleItem<Route>(travelTime, EventNames.FORAGER_ARRIVING_AT_RESOURCE, this, $"Forager arrived at route {Name}");
           // ScheduleManager.instance.AddScheduleItem<int>(travelTime, EventNames.FORAGER_ARRIVING_AT_RESOURCE, GetInstanceID(), $"Forager arrived at route {Name}");


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
            //ResourceManager.instance.AddWorker();
            ForagerManager.instance.ReturnForager();
            OnChange();
            return true;
        }
        return false;
    }

    public void RemoveAllForagers() { 
        while(WorkersAssigned > 0)
        {
            ForagerManager.instance.ReturnForager();
            WorkersAssigned--;
        }
    }

    public int GetResources(int amt) {
        int result = Mathf.Clamp(amt, 0, Resources);
        Resources -= result;
        if(Resources <= 0) {
            Depleted = true;
        }
        OnChange();
        return result;
    }

    //void OnForagerArrival(int id) {
    //    Debug.Log("Forager arrival detected with id " + id + " compared to local ID " + GetInstanceID());

    //    if (id != GetInstanceID())
    //        return;

    //    ScheduleManager.instance.AddScheduleItem<int>(ControlManager.Times.ForageTime, EventNames.FORAGER_DEPARTING_FROM_RESOURCE, GetInstanceID(), $"Forager has completed foraging route {Name}, returning to hive");   
    //}

    //void OnForagingComplete(int id) {
    //    if (id != GetInstanceID())
    //        return;

    //    Resources = Mathf.Max(Resources - ControlManager.Quantities.GatherRate, 0);
    //    ScheduleManager.instance.AddScheduleItem<int>(travelTime, EventNames.FORAGER_RETURNING_TO_HIVE, GetInstanceID(), $"Forager returning to hive from {Name}");    
    //}
}
