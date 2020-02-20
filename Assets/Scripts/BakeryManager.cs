using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class BakeryManager : MonoBehaviour
{
    enum BakerState { 
        WaitingOnPollen,
        Baking
    }

    [Serializable]
    class Baker {
        public BakerState state;
        public int estTaskCompletion;
        public int taskId;

        public Baker() {
            state = BakerState.WaitingOnPollen;
            estTaskCompletion = 99;
        }

        public int CalcPriority() {
            return (state == BakerState.WaitingOnPollen) ? 0 : estTaskCompletion;
        }
    }

    public static BakeryManager instance;

    [SerializeField] Button addBakerButton;
    [SerializeField] Button removeBakerButton;

    CellManager bakeryCellManager;
    List<Baker> bakers = new List<Baker>();

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

        addBakerButton.onClick.RemoveListener(AddBaker);
        addBakerButton.onClick.AddListener(AddBaker);

        removeBakerButton.onClick.RemoveListener(RemoveBaker);
        removeBakerButton.onClick.AddListener(RemoveBaker);

    }

    void OnAddBakerButtonPress() { }

    void OnRemoveBakerButtonPress() { }

    void AddBaker() {
        if(JobManager.instance.BakeryCellManager.HasFreeCell() && 
           ResourceManager.instance.RemoveWorker()
        ) {
            JobManager.instance.BakeryCellManager.ActivateCell();
            Baker baker = new Baker();
            bakers.Add(baker);
            BakeBread(baker);
        }
    }

    void RemoveBaker() {
        if (bakers.Count == 0)
            return;
        List<Baker> sortedList = bakers.OrderByDescending(f => f.CalcPriority()).ToList();

        Baker baker = sortedList[0];
        ResourceManager.instance.AddWorker();
        JobManager.instance.BakeryCellManager.DeactivateCell();
        ScheduleManager.instance.RemoveScheduleItem(baker.taskId);
        bakers.Remove(baker);
    }

    void BakeBread(Baker baker) { 
        //If was already in baking state, assume just completed baking
        if(baker.state == BakerState.Baking) {
            ResourceManager.instance.AddBread(ControlManager.instance.Quantities.BreadPerBake);
        }
        if (ResourceManager.instance.RemovePollen(ControlManager.instance.Quantities.PollenPerBread)) {
            baker.state = BakerState.Baking;
            baker.estTaskCompletion = ControlManager.instance.Times.BakingTime + StepController.StepNumber;
            string startingMessage = $"Baker {baker} starting to bake bread. Est completion: {baker.estTaskCompletion}";
            string endingMessage = $"Baker {baker} finished baking bread";
            Action callback = () => BakeBread(baker);

            baker.taskId = ScheduleManager.instance.AddScheduleItem(ControlManager.instance.Times.BakingTime, callback, startingMessage, endingMessage);
        } else {
            //If not enough pollen to make bread, just keep checking every step
            baker.state = BakerState.WaitingOnPollen;
            baker.taskId = ScheduleManager.instance.AddScheduleItem(1, () => BakeBread(baker));
        }
    }
}
