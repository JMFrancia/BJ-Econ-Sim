using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteListEntry : MonoBehaviour
{
    public Route Route { get; private set; }

    [SerializeField] Text rarityText;
    [SerializeField] Text distanceText;
    [SerializeField] Text resourcesText;
    [SerializeField] Text assignedWorkerstext;

    public void Initialize(Route route)
    {
        Route = route;
        distanceText.text = route.Distance.ToString();
        rarityText.text = route.Rarity.ToString();
        Route.OnChange += OnRouteChange;
    }

    public void OnPlusButtonPressed() {
        Route.AddForager();
    }

    public void OnMinusButtonPressed() {
        Route.RemoveForager();
    }

    void OnRouteChange() {
        resourcesText.text = Route.Resources.ToString();
        assignedWorkerstext.text = $"{Route.WorkersAssigned} / {Route.WorkerCapacity}";
    }

    public void OnDeleteButtonPressed() {
        Destroy(gameObject);
    }
}
