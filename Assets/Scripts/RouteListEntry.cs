using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteListEntry : MonoBehaviour
{
    public enum FlowerRarity { 
        COMMON,
        SEASONAL,
        RARE,
        UNIQUE
    }

    public int Distance { 
        get {
            return _distance;
        }
        set {
            _distance = value;
            distanceText.text = value.ToString();
        }
    }

    public FlowerRarity Rarity { 
        get {
            return _rarity;
        }
        set {
            _rarity = value;
            rarityText.text = value.ToString();
        }
    }

    public int Resources { 
        get {
            return _resources;
        }

        set {
            _resources = value;
            resourcestext.text = value.ToString();
        }
    }

    public int WorkersAssigned { 
        get {
            return _workersAssigned;
        }
        set {
            _workersAssigned = value;
            assignedWorkerstext.text = $"{_workersAssigned} / {_workerCapacity}";
        }
    }

    public int WorkerCapacity
    {
        get
        {
            return _workerCapacity;
        }
        set
        {
            _workerCapacity = value;
            assignedWorkerstext.text = $"{_workersAssigned} / {_workerCapacity}";
        }
    }

    [SerializeField] Text rarityText;
    [SerializeField] Text distanceText;
    [SerializeField] Text resourcestext;
    [SerializeField] Text assignedWorkerstext;

    FlowerRarity _rarity;
    int _distance;
    int _resources;
    int _workersAssigned;
    int _workerCapacity;

    public void OnPlusButtonPressed() { 
        if(_workersAssigned < _workerCapacity && ResourceManager.instance.RemoveWorker()) {
            _workersAssigned++;
            UpdateAssignedWorkersText();
        }
    }

    public void OnMinusButtonPressed() { 
        if(_workersAssigned > 0) {
            _workersAssigned--;
            ResourceManager.instance.AddWorker();
            UpdateAssignedWorkersText();
        }
    }

    void UpdateAssignedWorkersText() {
        assignedWorkerstext.text = $"{_workersAssigned} / {_workerCapacity}";
    }

    public void OnDeleteButtonPressed() {
        Destroy(gameObject);
    }

    public void Initialize(FlowerRarity rarity, int distance, int resources, int workerCapcity) {
        this.Distance = distance;
        this.Rarity = rarity;
        this.WorkerCapacity = workerCapcity;
        this.Resources = resources;
        this.WorkersAssigned = 0;
    }
}
