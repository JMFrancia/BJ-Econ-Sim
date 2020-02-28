using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public bool removeOnGet = true;
    List<Poolable<T>> pList = new List<Poolable<T>>();

    Dictionary<int, Poolable<T>> poolableDict = new Dictionary<int, Poolable<T>>();

    int poolableIdCounter = 0;

    List<int> pool = new List<int>();
    int top = 0;

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
    }

    public T Get() {
        int pId = pool[UnityEngine.Random.Range(0, pool.Count)];
        T result = poolableDict[pId].item;
        if (removeOnGet) {
            Remove(pId, poolableDict[pId].weight);
        }
        return result;
    }

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
