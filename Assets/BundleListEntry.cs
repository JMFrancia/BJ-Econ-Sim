using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BundleListEntry : MonoBehaviour
{
    [SerializeField] Text bundleText;
    [SerializeField] Button sellButton;

    BundleManager.Bundle bundle;

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.HONEY_CHANGED, SetActive);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.HONEY_CHANGED, SetActive);
    }

    void SetActive() {
        bool active = bundle.requirements.Keys.All(type => ResourceManager.Honey(type) >= bundle.requirements[type]);
        sellButton.interactable = active;
        bundleText.color = active ? Color.black : Color.gray;
    }

    public void SetBundle(BundleManager.Bundle bundle)
    {
        this.bundle = bundle;
        bundleText.text = GenerateBundleLabel(bundle);
        sellButton.GetComponentInChildren<Text>().text = $"${bundle.value}";
        SetActive();
        sellButton.onClick.AddListener(OnSellButtonPress);
    }

    void OnSellButtonPress() 
    {
        for(int n = 0; n < bundle.types.Length; n++) {
            ResourceManager.instance.RemoveHoney(bundle.types[n], 1);
        }
        ResourceManager.instance.AddBucks(bundle.value);
        if(bundle.types[0] != bundle.types[1] || bundle.types[1] != bundle.types[2])
        {
            Destroy(gameObject);
        }
    }

    string GenerateBundleLabel(BundleManager.Bundle b) {
        string result = "";
        for(int n = 0; n < b.types.Length; n++) { 
            switch(b.types[n]) {
                case FlowerType.Common:
                    result += "C";
                    break;
                case FlowerType.Seasonal:
                    result += "S";
                    break;
                case FlowerType.Rare:
                    result += "R";
                    break;
                case FlowerType.Unique:
                    result += "U";
                    break;
            }
            if(n < b.types.Length - 1) {
                result += " + ";
            }
        }
        return result;
    }
}
