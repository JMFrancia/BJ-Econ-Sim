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

    public static int Bread { 
        get {
            return _bread;
        }
    }

    public static int CommonHoney { 
        get {
            return instance.honeyManager.Amount(FlowerType.Common);
        }
    }

    public static int SeasonalHoney
    {
        get
        {
            return instance.honeyManager.Amount(FlowerType.Seasonal);
        }
    }

    public static int RareHoney
    {
        get
        {
            return instance.honeyManager.Amount(FlowerType.Rare);
        }
    }

    public static int UniqueHoney
    {
        get
        {
            return instance.honeyManager.Amount(FlowerType.Unique);
        }
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
    static int _nectar = 0;
    static int _workers = 5;
    static int _bread = 0;
    static int _bucks = 0;

    [SerializeField] HoneyManager honeyManager;
    [SerializeField] Text pollenText;
    [SerializeField] Text workersText;
    [SerializeField] Text nectarText;
    [SerializeField] Text breadText;
    [SerializeField] Text bucksText;

    private void Awake()
    {
        instance = this;

        workersText.text = _workers.ToString();
        pollenText.text = _pollen.ToString();
        nectarText.text = _nectar.ToString();
        breadText.text = _bread.ToString();
        bucksText.text = _bucks.ToString();
    }

    public void Initialize(int workers, int bucks, int nectar, int pollen, int bread, int honey)
    {
        _pollen = pollen;
        _nectar = nectar;
        _workers = workers;
        _bread = bread;
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

    public int AddNectar(int amt) {
        return AddResource(ref _nectar, amt, nectarText);
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
