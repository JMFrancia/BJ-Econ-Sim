using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Refactor to contain list of actual foragers, along with their states and inventory
public class ForagerManager : MonoBehaviour
{
    enum ForagerState { 
        TravelingToFlower,
        Foraging,
        TravelingToHive    
    }

    class Forager {
        public ForagerState state;
        public int resources;
        public int estTaskCompletion;
        public Route Route { get; private set; }

        public Forager(Route r) {
            Route = r;
            resources = 0;
            state = ForagerState.TravelingToFlower;
            estTaskCompletion = 99;
        }

        //Higher # means further away from returning resources
        //Relative to all possible positions on the route
        public int CalcPriority()
        {
            int stagePriority = 0;
            switch (state) {
                case ForagerState.Foraging:
                    stagePriority = 100;
                    break;
                case ForagerState.TravelingToFlower:
                    stagePriority = 200;
                    break;
                default:
                    break;
            }
            return stagePriority + estTaskCompletion;
        }
    }


    public static ForagerManager instance;

    Dictionary<Route, List<Forager>> foragerDict = new Dictionary<Route, List<Forager>>();

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListeningClass(EventNames.ROUTE_DEPLETED, OnRouteDepleted);
    }

    private void OnDisable()
    {
        EventManager.StopListeningClass(EventNames.ROUTE_DEPLETED, OnRouteDepleted);
    }

    public bool AddForager(Route route) { 
        if(route.HasCapacity() && ResourceManager.instance.RemoveWorker())
        {
            route.AddForager();


            Forager forager = new Forager(route);

            if (foragerDict.ContainsKey(route)) {
                foragerDict[route].Add(forager);
            } else {
                foragerDict[route] = new List<Forager>() { forager };
            }

            SendToRoute(forager);

            return true;
        }
        return false;
    }

    //On route depleted, remove all workers not currently carrying resources
    void OnRouteDepleted(Route route) { 
        for(int n = 0; n < foragerDict[route].Count; n++) {
            Forager f = foragerDict[route][n];
            if(f.state != ForagerState.TravelingToHive) {
                foragerDict[route].RemoveAt(n);
                ResourceManager.instance.AddWorker();  //Why this giving us an extra worker? Forager should be returning to hive //Because the foragers in dict are copies
                n--;
            }
        }

        //foreach (Forager f in foragerDict[route]) { 
        //    if(f.state != ForagerState.TravelingToHive) {
        //        foragerDict[route].Remove(f);
        //        ResourceManager.instance.AddWorker();
        //    }
        //}
    }

    //On route closed, remove all workers immediately
    void OnRouteClose(Route route) { 
        foreach(Forager f in foragerDict[route]) {
            foragerDict[route].Remove(f);
            ResourceManager.instance.AddWorker();
        }
    }

    //Return forager least likely to return resources soon
    public void ReturnForager(Route route) {
        List<Forager> sortedList = foragerDict[route].OrderByDescending(f => f.CalcPriority()).ToList();
        ReturnForager(sortedList[0]);
    }

    //Return a specific forager
    void ReturnForager(Forager forager) {
        List<Forager> routeForagers = foragerDict[forager.Route];
        routeForagers.Remove(forager);
        /*
        foreach(Forager f in routeForagers) { 
            if(f.Equals(forager)) {
                routeForagers.Remove(f);
                break;
            }
        }
        */
        if(routeForagers.Count == 0) {
            foragerDict.Remove(forager.Route);
        }
        ResourceManager.instance.AddWorker();
    }

    //Check state and initialize next steps
    void OnForagerTaskCompleted(Forager forager) {
        switch(forager.state) {
            case ForagerState.TravelingToFlower:
                Forage(forager);
                break;
            case ForagerState.Foraging:
                CompleteForaging(forager);
                break;
            case ForagerState.TravelingToHive:
                ReturnToHive(forager);
                break;
        }
    }

    void SendToRoute(Forager forager)
    {
        forager.state = ForagerState.TravelingToFlower;

        int travelTime = CalcRouteTravelTime(forager.Route);
        forager.estTaskCompletion = travelTime + StepController.StepNumber;
        string startMessage = $"Forager {forager} leaving hive for route {forager.Route.Name}. Est. arrival: step {forager.estTaskCompletion}";
        string endMessage = $"Forager {forager} arrived at route {forager.Route.Name}";
        Action callback = () => OnForagerTaskCompleted(forager);

        //Schedule begin foraging upon arrival
        ScheduleManager.instance.AddScheduleItem(travelTime, callback, startMessage, endMessage);
    }


    void Forage(Forager forager)
    {
        forager.state = ForagerState.Foraging;

        forager.estTaskCompletion = ControlManager.Times.ForageTime + StepController.StepNumber;
        string startingMessage = $"Forager {forager} beginning foraging on route {forager.Route.Name}. Est. Completion: {forager.estTaskCompletion}";
        string endingMessage = $"Forager {forager} ended foraging on route {forager.Route.Name}";
        Action callback = () => OnForagerTaskCompleted(forager);

        //Schedule end of foraging
        ScheduleManager.instance.AddScheduleItem(ControlManager.Times.ForageTime, callback, startingMessage, endingMessage);
    }

    void CompleteForaging(Forager forager)
    {
        //Schedule return to hive
        forager.state = ForagerState.TravelingToHive;

        int travelTime = CalcRouteTravelTime(forager.Route);
        forager.resources = forager.Route.GetResources(ControlManager.Quantities.GatherRate);
        forager.estTaskCompletion = travelTime + StepController.StepNumber;
        string departingMessage = $"Forager {forager} leaving from route {forager.Route.Name} to return to hive with {forager.resources} resources. Est arrival: {forager.estTaskCompletion}";
        string arrivingMessage = $"Forager {forager} returned successfully to hive with {forager.resources} resources";
        Action callback = () => OnForagerTaskCompleted(forager);


        ScheduleManager.instance.AddScheduleItem(travelTime, callback, departingMessage, arrivingMessage);
    }

    void ReturnToHive(Forager forager) {
        ResourceManager.instance.AddNectar(forager.resources);
        ResourceManager.instance.AddPollen(forager.resources);

        if(forager.Route.Depleted) {

            //Resources being returned -> 
            forager.Route.RemoveForager(removeFromForageMgr: false);
            ReturnForager(forager);
        } else {
            SendToRoute(forager);
        }


        //if route still open, repeat
        //if (!forager.Route.Depleted && !forager.Route.Closed)
        //{
        //    SendToRoute(forager);
        //} else {
        //    ReturnForager(forager);
        //}
    }

    int CalcRouteTravelTime(Route route) { 
        return route.Distance * ControlManager.Times.TravelTime; 
    }
}
