using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NectarHoneyManager : MonoBehaviour
{
    Dictionary<FlowerType, int> resDict = new Dictionary<FlowerType, int>(){
        {FlowerType.Common, 0},
        {FlowerType.Seasonal, 0},
        {FlowerType.Rare, 0},
        {FlowerType.Unique, 0}
    };

    [SerializeField] Text commonText;
    [SerializeField] Text seasonalText;
    [SerializeField] Text rareText;
    [SerializeField] Text uniqueText;
    [SerializeField] Text totalText;

    private void Awake()
    {
        UpdateDisplay();
    }

    public void Initialize(Dictionary<FlowerType, int> dict) {
        if(dict != null)
        {
            resDict = dict;
        }
        UpdateDisplay();
    }

    public int Add(FlowerType type, int amt) {
        resDict[type] += amt;
        UpdateDisplay(type);
        EventManager.TriggerEvent(EventNames.HONEY_CHANGED);
        return resDict[type];
    }

    public bool Remove(FlowerType type, int amt) {
        if (resDict[type] >= amt) {
            resDict[type] -= amt;
            UpdateDisplay(type);
            EventManager.TriggerEvent(EventNames.HONEY_CHANGED);
            return true;
        }
        return false;
    }

    public int Amount(FlowerType type) {
        return resDict[type];
    }

    public int Total() {
        return resDict.Values.Sum();
    }

    private void UpdateDisplay()
    {
        commonText.text = resDict[FlowerType.Common].ToString();
        seasonalText.text = resDict[FlowerType.Seasonal].ToString();
        rareText.text = resDict[FlowerType.Rare].ToString();
        uniqueText.text = resDict[FlowerType.Unique].ToString();
        UpdateTotalDisplay();
    }

    void UpdateDisplay(FlowerType type) {
        switch(type) {
            case FlowerType.Common:
                commonText.text = resDict[FlowerType.Common].ToString();
                break;
            case FlowerType.Seasonal:
                seasonalText.text = resDict[FlowerType.Seasonal].ToString();
                break;
            case FlowerType.Rare:
                rareText.text = resDict[FlowerType.Rare].ToString();
                break;
            case FlowerType.Unique:
                uniqueText.text = resDict[FlowerType.Unique].ToString();
                break;
        }
        UpdateTotalDisplay();
    }

    void UpdateTotalDisplay() {
        totalText.text = Total().ToString();
    }
}
