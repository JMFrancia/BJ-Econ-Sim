using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public static int Pollen {
        get {
            return _pollen;
        }
    }

    public static int Nectar {
        get {
            return _nectar;
        }
    }

    public static int Workers { 
        get {
            return _workers;
        }
    }

    static int _pollen = 0;
    static int _nectar = 0;
    static int _workers = 5;

    [SerializeField] Text pollenText;
    [SerializeField] Text workersText;
    [SerializeField] Text nectarText;

    private void Awake()
    {
        instance = this;

        workersText.text = _workers.ToString();
        pollenText.text = _pollen.ToString();
        nectarText.text = _nectar.ToString();
    }

    public void Initialize(int pollen, int nectar, int workers)
    {
        _pollen = pollen;
        _nectar = nectar;
        _workers = workers;
    }

    public bool RemoveWorker()
    {
        return RemoveResource(ref _workers, 1, workersText);
    }

    public bool RemoveNectar(int amt)
    {
        return RemoveResource(ref _nectar, amt, nectarText);
    }

    public bool RemovePollen(int amt)
    {
        return RemoveResource(ref _pollen, amt, pollenText);
    }

    public int AddWorker() {
        return AddResource(ref _workers, 1, workersText);
    }

    public int AddNectar(int amt) {
        return AddResource(ref _nectar, amt, nectarText);
    }

    public int AddPollen(int amt) {
        return AddResource(ref _pollen, amt, pollenText);
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
