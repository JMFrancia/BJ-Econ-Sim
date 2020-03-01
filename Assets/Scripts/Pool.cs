using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Pool is a generic-classed data structure
 * As input, it is given a set of T, with associated integer weights for each member.
 * As output, Pool will randomly return a member of the set, with probability 
 * based on its weight (out of sum of all weights).
 */

public class Pool<T> : MonoBehaviour
{
    [Serializable]
    struct Poolable<K> {
        public K item;
        public int weight;

        public Poolable(K item, int weight) {
            this.item = item;
            this.weight = weight;
        }
    }

    /* If true, will remove a member upon its return from Get() */
    public bool RemoveOnGet = false;

    /*
     * pool is list of all pIds
     */
    List<int> pool = new List<int>();
    List<Poolable<T>> pList = new List<Poolable<T>>();
    Dictionary<int, Poolable<T>> poolableDict = new Dictionary<int, Poolable<T>>();

    int poolableIdCounter = 0;

    public Pool(Dictionary<T, int> items = null) {
        if(items != null) {
            Add(items);
        }
    }

    private void Awake()
    {
        pList.ForEach(Add);
    }

    void Add(Poolable<T> p) {
        Add(p.item, p.weight, false);
    }

    void Add(T item, int weight, bool newPoolable) {
        Poolable<T> p = new Poolable<T>(item, weight);

        if (newPoolable) {
            pList.Add(p);
        }

        poolableDict.Add(poolableIdCounter, p);

        List<int> newItem = new List<int>();
        for (int n = 0; n < weight; n++)
        {
            newItem.Add(poolableIdCounter);
        }

        poolableIdCounter++;
        pool.AddRange(newItem);
    }

    public void Add(T item, int weight) {
        Add(item, weight, true);
    }

    public void Add(Dictionary<T, int> items) { 
        foreach(T key in items.Keys) {
            Add(key, items[key]);
        }
        TryOptimizeSize();
    }

    /*
     * Returns a T member randomly based on its weight 
     */
    public T Get() {
        if(pool.Count == 0) {
            Debug.LogError("Trying to Get() from empty pool");
            return default(T);
        }
        int pId = pool[UnityEngine.Random.Range(0, pool.Count)];
        T result = poolableDict[pId].item;
        if (RemoveOnGet) {
            Remove(pId, poolableDict[pId].weight);
        }
        return result;
    }

    /*
     * Removes all instances of poolable from pool. 
     * I thought I could use weight to make efficient using RemoveRange,
     * but apparently it operates in O(N) time anyway, so might as well just
     * provide pId alone and do manually    
     */
    public void Remove(int pId, int weight)
    {
        int start = 0;
        for(int n = 0; n < pool.Count; n++) { 
            if(pool[n] == pId) {
                start = n;
                break;
            }
        }
        pool.RemoveRange(start, weight);
        pList.Remove(poolableDict[pId]);
        poolableDict.Remove(pId);
        TryOptimizeSize();
    }

    /* 
     * Attempt to optimize pool size by dividing all poolable weights by their
     * greatest common denomenator
     */
    void TryOptimizeSize() {
        if (pList.Count < 2)
            return;

        //Get GCD of 2 smallest member weights with weight size > 1
        List<Poolable<T>> sorted = new List<Poolable<T>>(pList);
        sorted.Sort((p1, p2) => p1.weight.CompareTo(p2.weight));
        while(sorted.Count > 1 && sorted[0].weight == 1) {
            sorted.RemoveAt(0);
        }
        if (sorted.Count < 2)
            return;

        int gcd = EuclidGCDByMod(sorted[0].weight, sorted[1].weight);

        if(gcd > 1) {
            //if all member weights divisible by a GCD > 1, can optimize size by doing so and re-creating pool
            for(int n = 1; n < pList.Count; n++) { 
                if(pList[n].weight % gcd != 0) {
                    return;
                }
            }
            pool.Clear();
            pList.ForEach(poolable => {
                poolable.weight /= gcd;
                Add(poolable);
            });
        }
    }

    /*
     * Euclid's GCD algorithm, modified to save time using modulus   
     */   
    public int EuclidGCDByMod(int value1, int value2)
    {
        while (value1 != 0 && value2 != 0)
        {
            if (value1 > value2)
                value1 %= value2;
            else
                value2 %= value1;
        }
        return Math.Max(value1, value2);
    }

    /*
     * Part 1:
     * 
     * Say you want to build a data structure called an object pool. It has the following methods:
     *     
     * Add(Object obj, int weight): add the object O to the pool with given weight. 
     * Remove(Object obj): remove the object from the pool
     * Get(): Randomly pick an object out of the pool, using the weights to bias the result.     
     * 
     * So for example say you add 3 objects:
     *     
     *      Add(o1, 1);
     *      Add(o2, 2);
     *      Add(o3, 3);
     * 
     * Then you call get(). There should be a 1/6 chance you get o1, 2/6 you get o2, and 3/6 you get o3.
     * 
     * How would you implement this data structure? 
     * 
     * Part 2:
     *     
     * Now say the pool also removes an object when it's returned by get(). 
     * How would this affect implementation / runtime?     
     */
}
