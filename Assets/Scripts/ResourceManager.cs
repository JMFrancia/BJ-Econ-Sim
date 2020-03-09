using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    //Using properties w/ static private vars exclusively
    //so can use private vars as refs (see: add & remove resource methods)
    public static int Pollen {
        get {
            return _pollen;
        }
    }

    public static int TotalNectar {
        get {
            return instance.nectarManager.Total();
        }
    }

    public static int Nectar(FlowerType type) {
        return instance.nectarManager.Amount(type);
    }

    public static int Workers {
        get {
            return _workers;
        }
    }

    public static int Bread {
        get {
            return _bread;
        }
    }

    public static int Honey(FlowerType type) {
        return instance.honeyManager.Amount(type);
    }

    public static int TotalHoney
    {
        get
        {
            return instance.honeyManager.Total();
        }
    }

    public static int Bucks { 
        get {
            return _bucks;
        }
    }

    static int _pollen = 0;
    static int _workers = 5;
    static int _bread = 0;
    static int _bucks = 0;

    [SerializeField] NectarHoneyManager honeyManager;
    [SerializeField] NectarHoneyManager nectarManager;
    [SerializeField] Text pollenText;
    [SerializeField] Text workersText;
    [SerializeField] Text breadText;
    [SerializeField] Text bucksText;

    private void Awake()
    {
        instance = this;

        ControlManager.StartingControls init = ControlManager.instance.StartingValues;

        honeyManager.Initialize(init.Honey);
        nectarManager.Initialize(init.Nectar);
        _pollen = init.Pollen;
        _bread = init.Bread;
        _bucks = init.Bucks;
        _workers = init.Workers;

        workersText.text = _workers.ToString();
        pollenText.text = _pollen.ToString();
        breadText.text = _bread.ToString();
        bucksText.text = _bucks.ToString();
    }

    public bool RemoveWorker()
    {
        return RemoveResource(ref _workers, 1, workersText);
    }

    public bool RemoveNectar(FlowerType type, int amt)
    {
        return nectarManager.Remove(type, amt);
    }

    public bool RemovePollen(int amt)
    {
        return RemoveResource(ref _pollen, amt, pollenText);
    }
                                            
    public bool RemoveBread(int amt) {
        return RemoveResource(ref _bread, amt, breadText);
    }

    public bool RemoveHoney(FlowerType type, int amt) {
        return honeyManager.Remove(type, amt);
    }

    public bool RemoveBucks(int amt)
    {
        return RemoveResource(ref _bucks, amt, bucksText);
    }

    public int AddWorker() {
        return AddResource(ref _workers, 1, workersText);
    }

    public int AddNectar(FlowerType type, int amt) {
        return nectarManager.Add(type, amt);
    }

    public int AddPollen(int amt) {
        return AddResource(ref _pollen, amt, pollenText);
    }

    public int AddBread(int amt) {
        return AddResource(ref _bread, amt, breadText);
    }

    public int AddHoney(FlowerType type, int amt) {
        return honeyManager.Add(type, amt);
    }

    public int AddBucks(int amt) {
        return AddResource(ref _bucks, amt, bucksText);
    }

    bool RemoveResource(ref int res, int amt, Text resText) {
        if (res - amt >= 0)
        {
            res -= amt;
            resText.text = res.ToString();
            return true;
        }
        return false;
    }

    int AddResource(ref int res, int amt, Text resText) {
        res += amt;
        resText.text = res.ToString();
        return res;
    }
}
