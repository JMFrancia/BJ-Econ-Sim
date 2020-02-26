using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteListController : MonoBehaviour
{

    [SerializeField] GameObject RouteListEntryPrefab;
    [SerializeField] Button AddRouteButton;

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

    Dictionary<FlowerTypeNames, FlowerType> flowerTypeDict;

    private void Awake()
    {
        minResources = ControlManager.instance.Routes.MinResources;
        maxResources = ControlManager.instance.Routes.MaxResources;
        maxRange = ControlManager.instance.Routes.MaxDistance;
        routesPerForagingFrame = ControlManager.instance.Routes.RoutesPerForagingFrame;

        AddRouteButton.onClick.RemoveListener(AddRoute);
        AddRouteButton.onClick.AddListener(AddRoute);

        flowerTypeDict = new Dictionary<FlowerTypeNames, FlowerType>() {
            { FlowerTypeNames.Common, ControlManager.instance.FlowerData.CommonFlower },
            { FlowerTypeNames.Seasonal, ControlManager.instance.FlowerData.RareFlower },
            { FlowerTypeNames.Rare, ControlManager.instance.FlowerData.SeasonalFlower },
            { FlowerTypeNames.Unique, ControlManager.instance.FlowerData.UniqueFlower }
        };
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

    /*
    Route GenerateRoute(int zone) {
//        FlowerType type = flowerTypeDict[typeName];
        //return new Route(
        //    name: type.Names[Random.Range(0, type.Names.Count - 1)],
        //    rarity: 0,

        //);
    }
    */

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
