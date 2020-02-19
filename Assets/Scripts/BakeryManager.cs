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

        addBakerButton.onClick.RemoveListener(OnAddBakerButtonPress);
        addBakerButton.onClick.AddListener(OnAddBakerButtonPress);

        removeBakerButton.onClick.RemoveListener(OnRemoveBakerButtonPress);
        removeBakerButton.onClick.AddListener(OnRemoveBakerButtonPress);

        bakeryCellManager = JobManager.instance.BakeryCellManager;
    }

    void OnAddBakerButtonPress() { }

    void OnRemoveBakerButtonPress() { }

    void AddBaker() {
        if(bakeryCellManager.HasFreeCell() && 
           ResourceManager.instance.RemoveWorker()
        ) {
            Baker newBaker = new Baker();

        }
    }

    void RemoveBaker() {
        if (bakers.Count == 0)
            return;
        List<Baker> sortedList = bakers.OrderByDescending(f => f.CalcPriority()).ToList();



        ResourceManager.instance.AddWorker();
        bakeryCellManager.DeactivateCell();
    }

    void BakeBread(Baker baker) { 
        if(ResourceManager.instance.RemoveBread(ControlManager.instance.Quantities.PollenPerBread)) {
            baker.state = BakerState.Baking;
            baker.estTaskCompletion = ControlManager.instance.Times.BakingTime + StepController.StepNumber;
            string startingMessage = $"Baker {baker} starting to bake bread. Est completion: {baker.estTaskCompletion}";
            string endingMessage = $"Baker {baker} finished baking bread";
            Action callback = () => BakeBread(baker);

            ScheduleManager.instance.AddScheduleItem(ControlManager.instance.Times.BakingTime, startingMessage, endingMessage);
        } else {
            //If not enough pollen to make bread, just keep checking every step
            ScheduleManager.instance.AddScheduleItem(1, () => BakeBread(baker));
        }
    }


}
