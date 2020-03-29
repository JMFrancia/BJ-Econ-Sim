using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleManager : MonoBehaviour
{

    public class Bundle {
        public FlowerType[] types { get; }
        public Dictionary<FlowerType, int> requirements { get; }
        public int value { get; }

        public Bundle(FlowerType[] types) {
            this.types = types;
            value = CalculateValue();
            requirements = new Dictionary<FlowerType, int>();

            for(int n = 0; n < types.Length; n++) { 
                if(requirements.ContainsKey(types[n])) {
                    requirements[types[n]]++;
                } else {
                    requirements[types[n]] = 0;
                }
            }
        }

        public Bundle(FlowerType type) : this(new FlowerType[] { type, type, type }) { }

        int CalculateValue() {
            int result = 0;
            bool same = true;
            for (int n = 0; n < types.Length; n++)
            {
                result += ControlManager.instance.Prices.HoneyValues[types[n]];
                if (n > 0 && types[n] != types[n - 1])
                {
                    same = false;
                }
            }
            if (same)
            {
                result *= Mathf.RoundToInt(1 - (1 / types.Length) * .75f);
            }
            return result;
        }
    }

    private void Start()
    {
        FlowerType[] types = new FlowerType[4] {
            FlowerType.Common,
            FlowerType.Seasonal,
            FlowerType.Rare,
            FlowerType.Unique
        };
        for(int n = 0; n < types.Length; n++) {
            BundleListManager.instance.AddBundle(GenerateGenericBundle(types[n]));
        }
    }

    public Bundle GenerateGenericBundle(FlowerType type) {
        return new Bundle(new FlowerType[3] {  type, type, type });
    }

    public Bundle GenerateRandomBundle() {
        FlowerType[] types = new FlowerType[3];

        Pool<FlowerType> bundleGenPool = new Pool<FlowerType>(ControlManager.instance.Prices.BundleGenPoolWeights.ToDictionary());
        bundleGenPool.RemoveOnGet = true;
        for (int n = 0; n < 3; n ++) {
            types[n] = bundleGenPool.Get();
            if (n == 0)
            {
                bundleGenPool.RemoveOnGet = false; //Ensure not 3 of a kind
            }
        }
        return new Bundle(types);
    }
}
