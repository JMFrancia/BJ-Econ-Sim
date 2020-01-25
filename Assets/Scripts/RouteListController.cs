using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteListController : MonoBehaviour
{
//    int MAX_DISTANCE = 12;

    [SerializeField] GameObject RouteListEntryPrefab;

    [SerializeField] int maxRange = 5;
    [SerializeField] Button AddRouteButton;
    int routesPerForagingFrame = 3;

    private void Awake()
    {
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
        Route result = new Route("Rose", 1, 10, 10, 3);
        return result;
    }

}
