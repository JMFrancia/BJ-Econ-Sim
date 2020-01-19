using UnityEngine;
using System.Collections.Generic;
using System;

public class Route : MonoBehaviour
{
    //public enum FlowerRarity
    //{
    //    COMMON,
    //    SEASONAL,
    //    RARE,
    //    UNIQUE
    //}

    public string Name { get; private set; }
//    public FlowerRarity Rarity { get; private set; }
    public int Rarity { get; private set; }
    public int Distance { get; private set; }
    public int Resources { get; private set; }
    public int WorkersAssigned { get; private set; }
    public int WorkerCapacity { get; private set; }

//    public Queue<Flower> flowers;

    public Action OnChange;

    public Route(string name, int rarity, int distance, int resources, int workerCapcity) {
        Name = name;
        Rarity = rarity;
        Distance = distance;
        Resources = resources;
        WorkerCapacity = workerCapcity;

        //flowers = new Queue<Flower>();
        //for(int n = 0; n < WorkerCapacity; n++) {
        //    flowers.Enqueue(new Flower(resPerFlower));
        //}
    }

    //Re-do everything to work with foragers

    public bool AddForager() {
        if (WorkersAssigned < WorkerCapacity && ResourceManager.instance.RemoveWorker())
        {
            WorkersAssigned++;
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




    //public class Flower {

    //    public bool occupied;
    //    public int resources;

    //    public Flower(int resources) {
    //        this.resources = resources;
    //    }

    //    public int Forage() {
    //        //resources -= Mathf.Min(VariableManager.Foraging.resPerVisit, resources);
    //        return resources;
    //    }
    //}
}
