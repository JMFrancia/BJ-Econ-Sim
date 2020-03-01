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
            JobManager.instance.NursingCellManager.ActivateCell();
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
        JobManager.instance.NursingCellManager.DeactivateCell();
        ScheduleManager.instance.RemoveScheduleItem(maker.taskId);
        honeyMakers.Remove(maker);
    }

    //Need to create honey order system to continue
    void MakeHoney(HoneyMaker maker) { 
        if(maker.waitingOnNectar) { 
           // if(ResourceManager.)
        }
    }
}
