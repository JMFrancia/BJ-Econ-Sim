﻿using System;
using UnityEngine;

//Refactor to contain list of actual foragers, along with their states and inventory
public class ForagerManager : MonoBehaviour
{
    public static ForagerManager instance;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public bool AddForager(Route route) { 
        if(route.HasCapacity() && ResourceManager.instance.RemoveWorker())
        {
            route.AddForager();
            SendForagerToRoute(route);
            return true;
        }
        return false;
    }

    public void ReturnForager() {
        ResourceManager.instance.AddWorker();
    }

    void SendForagerToRoute(Route route)
    {
        int travelTime = CalcRouteTravelTime(route);
        string departingMessage = $"Forager leaving hive for route {route.Name}. Est. time: {travelTime}";
        string arrivingMessage = $"Forager arrived at route {route.Name}";
        Action callback = () => BeginForaging(route);

        //Schedule begin foraging upon arrival
        ScheduleManager.instance.AddScheduleItem(travelTime, callback, departingMessage, arrivingMessage);
    }

    void BeginForaging(Route route) {
        if (route.Depleted)
        {
            Debug.Log($"Route {route.Name} depleted");
            return;
        }

        string startingMessage = $"Forager beginning foraging on route {route.Name}";
        string endingMessage = $"Forager ended foraging on route {route.Name}";
        Action callback = () => EndForaging(route);

        //Schedule end of foraging
        ScheduleManager.instance.AddScheduleItem(ControlManager.Times.ForageTime, callback, startingMessage, endingMessage);
    }

    void EndForaging(Route route) {
        if (route.Depleted)
        {
            Debug.Log($"Route {route.Name} depleted");
            return;
        }
        //Schedule return to hive
        int res = route.GetResources(ControlManager.Quantities.GatherRate);
        string departingMessage = $"Forager leaving from route {route.Name} to return to hive with {res} resources";
        string arrivingMessage = $"Forager returned successfully to hive with {res} resources";
        Action callback = () => ReturnToHive(route, res);


        ScheduleManager.instance.AddScheduleItem(CalcRouteTravelTime(route), callback, departingMessage, arrivingMessage);
    }

    void ReturnToHive(Route route, int res) {
        ResourceManager.instance.AddNectar(res);
        ResourceManager.instance.AddPollen(res);

        //if route still open, repeat
        if(!route.Depleted) {
            SendForagerToRoute(route);
        }
    }

    int CalcRouteTravelTime(Route route) { 
        return route.Distance * ControlManager.Times.TravelTime; 
    }
}
