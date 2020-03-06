using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class HoneytoriumManager : MonoBehaviour
{
    [Serializable]
    class HoneyMaker {
        public bool waitingOnNectar = true;
        public FlowerType honeyType;
        public int estTaskCompletion;
        public int taskId;

        public int CalcPriority() {
            return waitingOnNectar ? 0 : estTaskCompletion;
        }
    }

    public static HoneytoriumManager instance;

    [SerializeField] Button addHoneyMakerButton;
    [SerializeField] Button removeHoneyMakerButton;

    List<HoneyMaker> honeyMakers = new List<HoneyMaker>();

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

        addHoneyMakerButton.onClick.RemoveListener(AddHoneyMaker);
        addHoneyMakerButton.onClick.AddListener(AddHoneyMaker);

        removeHoneyMakerButton.onClick.RemoveListener(RemoveHoneyMaker);
        removeHoneyMakerButton.onClick.AddListener(RemoveHoneyMaker);
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

    void MakeHoney(HoneyMaker maker) {
        if(!maker.waitingOnNectar) {
            //Assume just finished making honey
            ResourceManager.instance.AddHoney(maker.honeyType, ControlManager.instance.Quantities.HoneyPerMake);
        }
        //Prioritize rarest nectars first
        FlowerType[] types = (FlowerType[])Enum.GetValues(typeof(FlowerType));
        maker.waitingOnNectar = true;
        for(int n = types.Length -1; n >= 0; n--) { 
            if(ResourceManager.instance.RemoveNectar(types[n], ControlManager.instance.Quantities.NectarPerHoney)) {
                maker.waitingOnNectar = false;
                maker.honeyType = types[n];
                maker.estTaskCompletion = ControlManager.instance.Times.HoneyMakingTime + StepController.StepNumber;
                string startingMessage = $"HoneyMaker {maker} beginning on {types[n]} honey. Est. completion: {maker.estTaskCompletion}";
                string endingMessage = $"HoneyMaker {maker} completed making {types[n]} honey.";
                Action callback = () => MakeHoney(maker);

                maker.taskId = ScheduleManager.instance.AddScheduleItem(ControlManager.instance.Times.HoneyMakingTime, callback, startingMessage, endingMessage);
                break;
            }
        }
        //If waiting on Nectar, check every step until it's available
        if(maker.waitingOnNectar) {
            maker.taskId = ScheduleManager.instance.AddScheduleItem(1, () => MakeHoney(maker));
        }
    }
}
