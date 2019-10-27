using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteListController : MonoBehaviour
{
//    int MAX_DISTANCE = 12;

    [SerializeField] GameObject RouteListEntryPrefab;

    private void Awake()
    {
        for(int n = 0; n < 3; n ++) {
            GameObject newRoute = Instantiate(RouteListEntryPrefab);
            newRoute.transform.parent = gameObject.transform;
            newRoute.transform.localScale = Vector3.one;

            newRoute.GetComponent<RouteListEntry>().Initialize(
                new Route("Rose", Route.FlowerRarity.COMMON, 10, 10, 3)
            );


        }
    }

    //RouteListEntry GenerateRandomRouteListEntry(int distance) {
    //    int closeness;
    //    if(distance < MAX_DISTANCE * .25) {
    //        closeness = 0;
    //    } else if (distance < MAX_DISTANCE * .5) {
    //        closeness = 1;
    //    } else if (distance < MAX_DISTANCE * .75) {
    //        closeness = 2;
    //    } else {
    //        closeness = 3;
    //    }
    //}

}
