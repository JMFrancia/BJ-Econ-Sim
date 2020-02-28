using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteListController : MonoBehaviour
{

    [SerializeField] GameObject RouteListEntryPrefab;
    [SerializeField] Button AddRouteButton;

    List<Pool<FlowerType>> zones;

    int minResources;
    int maxResources;
    int maxRange;
    int routesPerForagingFrame;

    enum FlowerTypeNames { 
        Common,
        Seasonal,
        Rare,
        Unique
    }

    Dictionary<FlowerTypeNames, FlowerData> flowerTypeDict;

    private void Awake()
    {
        minResources = ControlManager.instance.Routes.MinResources;
        maxResources = ControlManager.instance.Routes.MaxResources;
        maxRange = ControlManager.instance.Routes.MaxDistance;
        routesPerForagingFrame = ControlManager.instance.Routes.RoutesPerForagingFrame;

        AddRouteButton.onClick.RemoveListener(AddRoute);
        AddRouteButton.onClick.AddListener(AddRoute);
        /*
        flowerTypeDict = new Dictionary<FlowerTypeNames, FlowerData>() {
            { FlowerTypeNames.Common, ControlManager.instance.FlowerData.CommonFlower },
            { FlowerTypeNames.Seasonal, ControlManager.instance.FlowerData.RareFlower },
            { FlowerTypeNames.Rare, ControlManager.instance.FlowerData.SeasonalFlower },
            { FlowerTypeNames.Unique, ControlManager.instance.FlowerData.UniqueFlower }
        };
        */

        zones = new List<Pool<FlowerType>>();
        ControlManager.instance.MapData.ZoneData.ForEach(zone => {
            zones.Add(
                new Pool<FlowerType>(zone.Weights)
           );
        });
    }

    void AddRoute() {
        if (RouteListEntry.TotalRoutes >= routesPerForagingFrame * JobManager.instance.ForagingFrameManager.Frames)
            return;
        GameObject newRouteGO = Instantiate(RouteListEntryPrefab);
        newRouteGO.transform.parent = gameObject.transform;
        newRouteGO.transform.localScale = Vector3.one;
        newRouteGO.GetComponent<RouteListEntry>().Initialize(
            GenerateRandomRoute()
        );
    }

    Route GenerateRoute(int zone) { 
        if(zone < 0 || zone > zones.Count) {
            Debug.LogError($"Attempting to get invalid zone # {zone}, only {zones.Count} available");
            return null;
        }
        FlowerType type = zones[zone].Get();
        FlowerData data = ControlManager.instance.MapData.FlowerTypeData[type];
        
        return new Route(
            name: data.Names[UnityEngine.Random.Range(0, data.Names.Count - 1)],
            rarity: (int) type,
            distance: zone + 1,
            resources: UnityEngine.Random.Range(data.Resources.min, data.Resources.max),
            workerCapacity: UnityEngine.Random.Range(data.Size.min, data.Size.max)
        );
    }

    Route GenerateRandomRoute() {
        return new Route(
            name: "Random Flower",
            rarity: UnityEngine.Random.Range(1, 5),
            distance: UnityEngine.Random.Range(1, maxRange + 1),
            resources: UnityEngine.Random.Range(minResources / 10, (maxResources + 1) / 10) * 10,
            workerCapacity: UnityEngine.Random.Range(1, 6)
        );
    }

}
