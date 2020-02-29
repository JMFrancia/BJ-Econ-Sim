using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class HoneyManager : MonoBehaviour
{
    static Dictionary<FlowerType, int> honeyDict = new Dictionary<FlowerType, int> {
        {FlowerType.Common, 0},
        {FlowerType.Seasonal, 0},
        {FlowerType.Rare, 0},
        {FlowerType.Unique, 0}
    };

    [SerializeField] Text commonHoneyText;
    [SerializeField] Text seasonalHoneyText;
    [SerializeField] Text rareHoneyText;
    [SerializeField] Text uniqueHoneyText;
    [SerializeField] Text totalText;

    private void Awake()
    {
        UpdateDisplay();
    }

    public void Initialize(Dictionary<FlowerType, int> dict) {
        honeyDict = dict;
        UpdateDisplay();
    }

    public int Add(FlowerType type, int amt) {
        honeyDict[type] += amt;
        UpdateDisplay(type);
        return honeyDict[type];
    }

    public bool Remove(FlowerType type, int amt) {
        if (honeyDict[type] >= amt) {
            honeyDict[type] -= amt;
            UpdateDisplay(type);
            return true;
        }
        return false;
    }

    public int Amount(FlowerType type) {
        return honeyDict[type];
    }

    public int Total() {
        return honeyDict.Values.Sum();
    }

    private void UpdateDisplay()
    {
        commonHoneyText.text = honeyDict[FlowerType.Common].ToString();
        seasonalHoneyText.text = honeyDict[FlowerType.Seasonal].ToString();
        rareHoneyText.text = honeyDict[FlowerType.Rare].ToString();
        uniqueHoneyText.text = honeyDict[FlowerType.Unique].ToString();
        UpdateTotalDisplay();
    }

    void UpdateDisplay(FlowerType type) {
        switch(type) {
            case FlowerType.Common:
                commonHoneyText.text = honeyDict[FlowerType.Common].ToString();
                break;
            case FlowerType.Seasonal:
                seasonalHoneyText.text = honeyDict[FlowerType.Seasonal].ToString();
                break;
            case FlowerType.Rare:
                rareHoneyText.text = honeyDict[FlowerType.Rare].ToString();
                break;
            case FlowerType.Unique:
                uniqueHoneyText.text = honeyDict[FlowerType.Unique].ToString();
                break;
        }
        UpdateTotalDisplay();
    }

    void UpdateTotalDisplay() {
        totalText.text = Total().ToString();
    }
}
