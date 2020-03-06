using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

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
    public MapControls MapData { get { return _flowers; } }

    [SerializeField] TimeControls _times;
    [SerializeField] QuantityControls _quantities;
    [SerializeField] RouteControls _routes;
    [SerializeField] StartingControls _start;
    [SerializeField] MapControls _flowers;

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

        [Serializable]
        public class NectarHoneyDict : UnitySerializedDictionary<FlowerType, int> { }

        public int Step = 0;
        public int Bread = 0;
        public int Bucks = 0;
        public int Workers = 5;
        public int Pollen = 0;
        [SerializeField]
        public NectarHoneyDict Nectar = new NectarHoneyDict() {
            { FlowerType.Common, 0 },
            { FlowerType.Seasonal, 0 },
            { FlowerType.Rare, 0 },
            { FlowerType.Unique, 0 }
        };
        [SerializeField]
        public NectarHoneyDict Honey = new NectarHoneyDict() {
            { FlowerType.Common, 0 },
            { FlowerType.Seasonal, 0 },
            { FlowerType.Rare, 0 },
            { FlowerType.Unique, 0 }
        };
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
        public int HoneyMakingTime = 5;
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
        public int NectarPerHoney = 3;
        public int HoneyPerMake = 1;
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
    public class MapControls
    {
        [Serializable]
        public struct ZoneWeights
        {
            [HideInInspector]
            public Dictionary<FlowerType, int> Weights;
            public int CommonFlowerWeight;
            public int SeasonalFlowerWeight;
            public int RareFlowerWeight;
            public int UniqueFlowerWeight;

            public ZoneWeights(int common = 0, int seasonal = 0, int rare = 0, int unique = 0)
            {
                CommonFlowerWeight = common;
                SeasonalFlowerWeight = seasonal;
                RareFlowerWeight = rare;
                UniqueFlowerWeight = unique;

                Weights = new Dictionary<FlowerType, int>() {
                    {FlowerType.Common, common},
                    {FlowerType.Seasonal, seasonal},
                    {FlowerType.Rare, rare},
                    {FlowerType.Unique, unique}
                };
            }
        }

        public List<ZoneWeights> ZoneData = new List<ZoneWeights>() {
            new ZoneWeights(5, 2),
            new ZoneWeights(5, 3),
            new ZoneWeights(5, 3, 1),
            new ZoneWeights(4, 3, 1),
            new ZoneWeights(4, 4, 2),
            new ZoneWeights(4, 4, 3, 1),
            new ZoneWeights(3, 4, 3, 2),
            new ZoneWeights(2, 3, 3, 3)
        };

        [Serializable]
        public class FlowerDataDictionary : UnitySerializedDictionary<FlowerType, FlowerData> { }

        public FlowerDataDictionary FlowerTypeData = new FlowerDataDictionary() {
            {
                 FlowerType.Common,
                 new FlowerData(
                    minZone: 1,
                    resources: new IntRange(3, 5),
                    size: new IntRange(3, 5),
                    names: new List<string>() {
                        "Lily",
                        "Poppy",
                        "Daisy"
                    }
                 )
            },
            {
                FlowerType.Seasonal,
                new FlowerData(
                    minZone: 1,
                    resources: new IntRange(2, 4),
                    size: new IntRange(2, 5),
                    names: new List<string>{
                        "Apricot",
                        "Apple",
                        "Walnut"
                    }
                )
            },
            {
                FlowerType.Rare,
                new FlowerData(
                    minZone: 2,
                    resources: new IntRange(2, 4),
                    size: new IntRange(1, 4),
                    names: new List<string>{
                        "Cherry-Blossom",
                        "Rose",
                        "Sunflower"
                    }
                )
            },
            {
                FlowerType.Unique,
                 new FlowerData(
                    minZone: 4,
                    resources: new IntRange(1, 3),
                    size: new IntRange(1, 3),
                    names: new List<string>{
                        "Aster",
                        "Orchid",
                        "Chocolate Cosmos"
                    }
                )
            }
        };
    }
}

public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private List<TKey> keyData = new List<TKey>();

    [SerializeField, HideInInspector]
    private List<TValue> valueData = new List<TValue>();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
        {
            this[this.keyData[i]] = this.valueData[i];
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        this.keyData.Clear();
        this.valueData.Clear();

        foreach (var item in this)
        {
            this.keyData.Add(item.Key);
            this.valueData.Add(item.Value);
        }
    }
}