﻿using UnityEngine;
using System.Collections.Generic;
using System;


public class Route
{
    public string Name { get; private set; }
    public FlowerType Type { get; private set; }
    public int Distance { get; private set; }
    public int Resources { get; private set; }
    public int WorkersAssigned { get; private set; }
    public int WorkerCapacity { get; private set; }
    public bool Depleted { get; private set; }
    public bool Closed { get; private set;  }

    public Action OnChange;

    public Route(string name, FlowerType type, int distance, int resources, int workerCapacity) {
        Name = name;
        Type = type;
        Distance = distance;
        Resources = resources;
        WorkerCapacity = workerCapacity;
        Depleted = false;
        Closed = false;
    }

    public bool HasCapacity() {
        return WorkersAssigned < WorkerCapacity;
    }

    public bool AddForager() {
        if(WorkersAssigned < WorkerCapacity && !Depleted)
        {
            WorkersAssigned++;
            OnChange();
            return true;
        }
        return false;
    }

    public bool RemoveForager(bool registerChange = true, bool removeFromForageMgr = true)
    {
        if (WorkersAssigned > 0)
        {
            WorkersAssigned--;

            //Returning rando forager even if should be removing specific one
            if(removeFromForageMgr)
                ForagerManager.instance.ReturnForager(this);
            if (WorkersAssigned == 0) {
                Closed = true;
            }
            if(registerChange)
                OnChange();
            return true;
        }
        return false;
    }

    public void RemoveAllForagers()
    {
        while (RemoveForager(false)) { }
    }

    public int GetResources(int amt) {
        int result = Mathf.Clamp(amt, 0, Resources);
        Resources -= result;
        if(Resources <= 0 && !Depleted) {
            Depleted = true;
            EventManager.TriggerEvent(EventNames.ROUTE_DEPLETED, this);
        }
        OnChange();
        return result;
    }
}
