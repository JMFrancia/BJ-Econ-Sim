using UnityEngine;
using System.Collections;
using System;

public class ControlManager : MonoBehaviour
{

    public static ControlManager instance;

    public static TimeControls Times { get; private set; }
    public static QuantityControls Quantities { get; private set; }

    [SerializeField] TimeControls times;
    [SerializeField] QuantityControls quantities;

    private void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        Times = times;
        Quantities = quantities;
    }

    [Serializable]
    public class TimeControls {
        public int TravelTime = 2;
        public int ForageTime = 5;
    }

    [Serializable]
    public class QuantityControls {
        public int GatherRate = 3;
    }
}
