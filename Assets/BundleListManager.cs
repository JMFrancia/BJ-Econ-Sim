using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class BundleListManager : MonoBehaviour
{
    public static BundleListManager instance;

    [SerializeField] GameObject bundleListEntryPrefab;
    [SerializeField] VerticalLayoutGroup parentGroup;

    private void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void AddBundle(BundleManager.Bundle bundle) {
        GameObject newBundle = Instantiate(bundleListEntryPrefab, parentGroup.transform);
        newBundle.GetComponent<BundleListEntry>().SetBundle(bundle);
        FlowerType type = bundle.types[0];
        if(bundle.types.All(t => t == type)) {
            newBundle.transform.SetAsLastSibling();
        }
    }
}
