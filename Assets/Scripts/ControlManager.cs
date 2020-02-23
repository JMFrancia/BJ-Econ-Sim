using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public IntRange(int min, int max)
    {
        if(max < min) {
            max = min + 1;
        }
        this.min = min;
        this.max = max;
    }
}

public class ControlManager : MonoBehaviour
{

    public static ControlManager instance { get; private set; }

    //Bad use case to load serialized fields into static vars
    //Probs creating race condition
    //Using singleton instead
    public TimeControls Times { get { return _times; } }
    public QuantityControls Quantities { get { return _quantities; } }
    public RouteControls Routes { get { return _routes; } }
    public StartingControls StartingValues { get { return _start; } }
    public FlowerControls FlowerSettings { get { return _flowers; } }

    [SerializeField] TimeControls _times;
    [SerializeField] QuantityControls _quantities;
    [SerializeField] RouteControls _routes;
    [SerializeField] StartingControls _start;
    [SerializeField] FlowerControls _flowers;

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
        public int Bucks = 0;
        public int Workers = 5;
        public int Nectar = 0;
        public int Pollen = 0;
        public int Honey = 0;
    }

    [Serializable]
    public class TimeControls
    {
        public int SecondsPerStep = 300;
        public int TravelTime = 2;
        public int ForageTime = 5;
        public int BakingTime = 5;
        public int EggLayTime = 3;
        public int NursingTime = 5;
    }

    [Serializable]
    public class QuantityControls
    {
        public int GatherRate = 3;
        public int ForagingCellsPerFrame = 8;
        public int BakeryCellsPerFrame = 4;
        public int NursingCellsPerFrame = 4;
        public int PollenPerBread = 3;
        public int BreadPerBake = 1;
        public int BreadPerBee = 1;
    }

    [Serializable]
    public class RouteControls
    {
        public int MaxDistance = 5;
        public int MinResources = 20;
        public int MaxResources = 50;
        public int RoutesPerForagingFrame = 3;
    }

    [Serializable]
    public class FlowerControls
    {
        [Serializable]
        public struct RaritySettings {
            public int MinZone;
            public IntRange Resources;
            public IntRange Size;

            public RaritySettings(int minZone, IntRange resources, IntRange size) {
                MinZone = minZone;
                Resources = resources;
                Size = size;
            }
        }

        public RaritySettings CommonFlower = new RaritySettings(
            minZone: 1,
            resources: new IntRange(3, 5),
            size: new IntRange(3, 5)
        );
        public RaritySettings SeasonalFlower = new RaritySettings(
            minZone: 1,
            resources: new IntRange(2, 4),
            size: new IntRange(2, 5)
        );
        public RaritySettings RareFlower = new RaritySettings(
            minZone: 2,
            resources: new IntRange(2, 4),
            size: new IntRange(1, 4)
        );
        public RaritySettings UniqueFlower = new RaritySettings(
            minZone: 4,
            resources: new IntRange(1, 3),
            size: new IntRange(1, 3)
        );
    }
}
