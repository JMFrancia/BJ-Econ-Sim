using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteListController : MonoBehaviour
{

    //TO-DO refactor flower data as structs, centralized in DataController, containing relevant info
    readonly string[] FLOWER_NAMES = new string[] {
        "Rose",
        "Aster",
        "Daisy",
        "Lilac",
        "Cherry-Blossom",
        "Sunflower",
        "Violet",
        "Lily",
        "Poppy",
        "Appricot",
        "Apple",
        "Walnut"
    };

    [SerializeField] GameObject RouteListEntryPrefab;
    [SerializeField] Button AddRouteButton;

    int minResources;
    int maxResources;
    int maxRange;
    int routesPerForagingFrame;

    private void Awake()
    {
        minResources = ControlManager.instance.Routes.MinResources;
        maxResources = ControlManager.instance.Routes.MaxResources;
        maxRange = ControlManager.instance.Routes.MaxDistance;
        routesPerForagingFrame = ControlManager.instance.Routes.RoutesPerForagingFrame;

        AddRouteButton.onClick.RemoveListener(AddRoute);
        AddRouteButton.onClick.AddListener(AddRoute);
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

    Route GenerateRandomRoute() {
        return new Route(
            name: FLOWER_NAMES[Random.Range(0, FLOWER_NAMES.Length)],
            rarity: Random.Range(1, 5),
            distance: Random.Range(1, maxRange + 1),
            resources: Random.Range(minResources / 10, (maxResources + 1) / 10) * 10,
            workerCapacity: Random.Range(1, 6)
        );
    }

}
