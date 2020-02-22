using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NurseryManager : MonoBehaviour
{
    enum NurseState {
        WaitingOnEgg,
        WaitingOnBread,
        Nursing
    }

    [Serializable]
    class Nurse {
        public NurseState state = NurseState.WaitingOnEgg;
        public int estTaskCompletion;
        public int taskId;

        public int CalcPriority()
        {
            return (state == NurseState.WaitingOnEgg) ? 0 : estTaskCompletion;
        }
    }

    public static NurseryManager instance;

    [SerializeField] Button addNurseButton;
    [SerializeField] Button removeNurseButton;

    List<Nurse> nurses = new List<Nurse>();
    [SerializeField] [ReadOnly]
    Queue<Nurse> nursesWaitingOnEggs = new Queue<Nurse>();

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

        addNurseButton.onClick.RemoveListener(AddNurse);
        addNurseButton.onClick.AddListener(AddNurse);

        removeNurseButton.onClick.RemoveListener(RemoveNurse);
        removeNurseButton.onClick.AddListener(RemoveNurse);
    }

    private void Start()
    {
        ScheduleQueenLayEgg();
        Debug.Log("Queen is active!");
    }

    void AddNurse() {
        if (JobManager.instance.NursingCellManager.HasFreeCell() &&
            ResourceManager.instance.RemoveWorker()
        )
        {
            JobManager.instance.NursingCellManager.ActivateCell();
            Nurse nurse = new Nurse();
            nurses.Add(nurse);
            nursesWaitingOnEggs.Enqueue(nurse);
            NurseBee(nurse);
        } 
    }

    void RemoveNurse() {
        if (nurses.Count == 0)
            return;
        List<Nurse> sortedList = nurses.OrderByDescending(n => n.CalcPriority()).ToList();

        Nurse nurse = sortedList[0];
        ResourceManager.instance.AddWorker();
        JobManager.instance.NursingCellManager.DeactivateCell();
        ScheduleManager.instance.RemoveScheduleItem(nurse.taskId);
        nurses.Remove(nurse);
    }

    void ScheduleQueenLayEgg(Nurse nurse = null) {
        Debug.Log("Problem?");
        int nextEggEst = ControlManager.instance.Times.EggLayTime + StepController.StepNumber;

        string startingMessage = "";
        if (nurse != null)
        {
            startingMessage = $"Egg laid for nurse {nurse}. Next one due: {nextEggEst}";
        } else {
            startingMessage = $"Next egg due: {nextEggEst}";
        }

        Action callback = QueenlayEgg;

        ScheduleManager.instance.AddScheduleItem(ControlManager.instance.Times.EggLayTime, callback, startingMessage);
    }

    void QueenlayEgg() {
        Nurse nurse = null;
        while(nurse == null && nursesWaitingOnEggs.Count > 0) { 
            nurse = nursesWaitingOnEggs.Dequeue();
            //Handles edge case where nurse in queue was already removed
            if(!nurses.Contains(nurse)) {
                nurse = null;
            }
        }
        //No valid nurses avialable
        if(nurse == null) {
            //If no nurses available, keep checking until one is
            ScheduleManager.instance.AddScheduleItem(1, QueenlayEgg);
            return;
        }

        nurse.state = NurseState.WaitingOnBread;
        NurseBee(nurse);

        ScheduleQueenLayEgg(nurse);
    }

    void NurseBee(Nurse nurse) { 
        //If was already in a nursing state, assume just completed nursing
        if(nurse.state == NurseState.Nursing) {
            ResourceManager.instance.AddWorker();

            nurse.state = NurseState.WaitingOnEgg;
            nursesWaitingOnEggs.Enqueue(nurse);
        } else if (nurse.state == NurseState.WaitingOnBread) {
            if (ResourceManager.instance.RemoveBread(ControlManager.instance.Quantities.BreadPerBee))
            {
                nurse.state = NurseState.Nursing;
                nurse.estTaskCompletion = ControlManager.instance.Times.NursingTime + StepController.StepNumber;
                string startingMessage = $"Nurse {nurse} nursing new bee. Est. completion: {nurse.estTaskCompletion}";
                string endingMessage = $"New worker is born from nurse {nurse}!";
                Action callback = () => NurseBee(nurse);

                nurse.taskId = ScheduleManager.instance.AddScheduleItem(ControlManager.instance.Times.NursingTime, callback, startingMessage, endingMessage);
            }
            else
            {
                //If waiting on bread, check every step until bread is available
                nurse.state = NurseState.WaitingOnBread;
                nurse.taskId = ScheduleManager.instance.AddScheduleItem(1, () => NurseBee(nurse));
            }
        }
    }
}
