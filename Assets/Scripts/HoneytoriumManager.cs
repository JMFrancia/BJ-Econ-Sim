using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class HoneytoriumManager : MonoBehaviour
{
    [Serializable]
    class HoneyMaker {
        public bool waiting = true;
        public FlowerType honeyType;
        public int estTaskCompletion;
        public int taskId;

        public int CalcPriority() {
            return waiting ? 0 : estTaskCompletion;
        }
    }

    public static HoneytoriumManager instance;

    [SerializeField] Button addHoneyMakerButton;
    [SerializeField] Button removeHoneyMakerButton;
    [SerializeField] Button addCommonOrderButton;
    [SerializeField] Button addSeasonalOrderButton;
    [SerializeField] Button addRareOrderButton;
    [SerializeField] Button addUniqueOrderButton;
    [SerializeField] Text commonOrderText;
    [SerializeField] Text seasonalOrderText;
    [SerializeField] Text rareOrderText;
    [SerializeField] Text uniqueOrderText;
    [SerializeField] Text totalOrdersText;

    bool autoOrder = true;

    List<HoneyMaker> honeyMakers = new List<HoneyMaker>();
    Dictionary<FlowerType, int> orders = new Dictionary<FlowerType, int>() {
        { FlowerType.Common, 0 },
        { FlowerType.Seasonal, 0 },
        { FlowerType.Rare, 0 },
        { FlowerType.Unique, 0 }
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        AddButtonCallback(ref addHoneyMakerButton, AddHoneyMaker);
        AddButtonCallback(ref removeHoneyMakerButton, RemoveHoneyMaker);
        AddButtonCallback(ref addCommonOrderButton, () => AddHoneyOrder(FlowerType.Common));
        AddButtonCallback(ref addSeasonalOrderButton, () => AddHoneyOrder(FlowerType.Seasonal));
        AddButtonCallback(ref addRareOrderButton, () => AddHoneyOrder(FlowerType.Rare));
        AddButtonCallback(ref addUniqueOrderButton, () => AddHoneyOrder(FlowerType.Unique));

        UpdateOrdersDisplay();
    }

    void UpdateOrdersDisplay() {
        FlowerType[] types = (FlowerType[])Enum.GetValues(typeof(FlowerType));

        commonOrderText.text = orders[FlowerType.Common].ToString();
        seasonalOrderText.text = orders[FlowerType.Seasonal].ToString();
        rareOrderText.text = orders[FlowerType.Rare].ToString();
        uniqueOrderText.text = orders[FlowerType.Unique].ToString();
        totalOrdersText.text = types.Sum((type) => orders[type]).ToString();
    }

    void AddButtonCallback(ref Button button, UnityAction callback) {
        button.onClick.RemoveListener(callback);
        button.onClick.AddListener(callback);
    }

    void AddHoneyOrder(FlowerType type) {
        if (autoOrder)
            return;
        //Require that orders don't exceed nectar cost
        if (ResourceManager.Nectar(type) >= ((orders[type] + 1) * ControlManager.instance.Quantities.NectarPerHoney)) {
            orders[type]++;
            UpdateOrdersDisplay();
        }
    }

    void AddHoneyMaker() {
        if(JobManager.instance.HoneyCellManager.HasFreeCell() &&
            ResourceManager.instance.RemoveWorker()) {
            JobManager.instance.HoneyCellManager.ActivateCell();
            HoneyMaker honeyMaker = new HoneyMaker();
            honeyMakers.Add(honeyMaker);
            MakeHoney(honeyMaker);
        }
    }

    void RemoveHoneyMaker() {
        if (honeyMakers.Count == 0)
            return;
        List<HoneyMaker> sortedList = honeyMakers.OrderByDescending(n => n.CalcPriority()).ToList();
        HoneyMaker maker = sortedList[0];
        ResourceManager.instance.AddWorker();
        JobManager.instance.HoneyCellManager.DeactivateCell();
        ScheduleManager.instance.RemoveScheduleItem(maker.taskId);
        honeyMakers.Remove(maker);
    }

    void MakeHoney(HoneyMaker maker)
    {
        if (!maker.waiting)
        {
            //Assume just finished making honey
            ResourceManager.instance.AddHoney(maker.honeyType, ControlManager.instance.Quantities.HoneyPerMake);
            maker.waiting = true;
        }

        //Prioritize rarest nectars first
        FlowerType[] types = (FlowerType[])Enum.GetValues(typeof(FlowerType));

        for (int n = types.Length - 1; n >= 0; n--)
        {
            if (autoOrder || orders[types[n]] > 0)
            {
                if (ResourceManager.instance.RemoveNectar(types[n], ControlManager.instance.Quantities.NectarPerHoney))
                {
                    if (!autoOrder)
                    {
                        orders[types[n]] = Math.Max(0, orders[types[n]] - 1);
                        UpdateOrdersDisplay();
                    }
                    maker.waiting = false;
                    maker.honeyType = types[n];
                    maker.estTaskCompletion = ControlManager.instance.Times.HoneyMakingTime + StepController.StepNumber;
                    string startingMessage = $"HoneyMaker {maker} beginning on {types[n]} honey. Est. completion: {maker.estTaskCompletion}";
                    string endingMessage = $"HoneyMaker {maker} completed making {types[n]} honey.";
                    Action callback = () => MakeHoney(maker);

                    maker.taskId = ScheduleManager.instance.AddScheduleItem(ControlManager.instance.Times.HoneyMakingTime, callback, startingMessage, endingMessage);
                    return;
                }
            }
        }

        //If waiting on Nectar, check every step until it's available
        if (maker.waiting)
        {
            maker.taskId = ScheduleManager.instance.AddScheduleItem(1, () => MakeHoney(maker));
        }
    }
}
