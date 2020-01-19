﻿using UnityEngine;
using UnityEngine.UI;

public class RouteListEntry : MonoBehaviour
{
    //Keeping total routes on RouteListEntry instead of RouteListController
    //so RouteListEntry doesn't need ref. to RouteListController
    public static int TotalRoutes { get; private set; } = 0;
    public Route Route { get; private set; }

    [SerializeField] Text rarityText;
    [SerializeField] Text distanceText;
    [SerializeField] Text resourcesText;
    [SerializeField] Text foragersText;
    [SerializeField] Button plusButton;
    [SerializeField] Button minusButton;

    public void Initialize(Route route)
    {

        Route = route;
        distanceText.text = route.Distance.ToString();
        rarityText.text = route.Rarity.ToString();

        Route.OnChange -= OnRouteChange;
        Route.OnChange += OnRouteChange;
        plusButton.onClick.RemoveListener(OnPlusButtonPressed);
        minusButton.onClick.RemoveListener(OnMinusButtonPressed);
        plusButton.onClick.AddListener(OnPlusButtonPressed);
        minusButton.onClick.AddListener(OnMinusButtonPressed);

        OnRouteChange();

        TotalRoutes++;
    }

    void OnPlusButtonPressed() {
        Route.AddForager();
    }

    void OnMinusButtonPressed() {
        if(!Route.RemoveForager()) {
            DestroyRouteListEntry();
        }
    }

    void OnRouteChange() {
        resourcesText.text = Route.Resources.ToString();
        foragersText.text = $"{Route.WorkersAssigned} / {Route.WorkerCapacity}";
    }

    void DestroyRouteListEntry() {
        for (int n = 0; n < Route.WorkersAssigned; n++) {
            Route.RemoveForager();
        }
        TotalRoutes--;
        Destroy(gameObject);
    }
}
