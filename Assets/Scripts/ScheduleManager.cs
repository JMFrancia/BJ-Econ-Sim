using System;
using UnityEngine;
using Priority_Queue;

public class ScheduleManager : MonoBehaviour
{
    public static ScheduleManager instance;

    SimplePriorityQueue<BaseScheduleItem> schedule = new SimplePriorityQueue<BaseScheduleItem>();

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
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.STEP, OnStep);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.STEP, OnStep);
    }

    public void AddScheduleItem<T>(int steps, string eventName, T param, string completionMessage = null) {
        if (steps < 0)
        {
            return;
        }
        if (steps == 0)
        {
            new ScheduleItem<T>(eventName, param, 0, completionMessage).Activate();
            return;
        }
        int scheduledStep = steps + StepController.StepNumber;
        schedule.Enqueue(new ScheduleItem<T>(eventName, param, scheduledStep, completionMessage), scheduledStep);
        Debug.Log("Scheduled " + eventName + " with " + param + " for step " + scheduledStep);
    }

    /*
    private void Start()
    {
        schedule.Enqueue(new ScheduleItem<bool>("test", true, 1), 1);
        BaseScheduleItem test = schedule.Dequeue();

        Debug.Log("Event name: " + test.EventName);
        Debug.Log("Scheduled step: " + test.ScheduledStep);
        if(test is ScheduleItem<bool> boolScheduleItem) {
            Debug.Log("Data: " + boolScheduleItem.Data);
        }
        //Debug.Log("Data: " + test.Data);  //Doesn't work
        test.Activate();
    }
    */

    void OnStep(int stepNumber) {
        Debug.Log($"Step {stepNumber}: schedule count {schedule.Count}");
        while (schedule.Count > 0 && stepNumber >= schedule.First.ScheduledStep) {
            //Debug.Log("Why is " + schedule.First.ScheduledStep + " more than " + stepNumber + "?");
            //Debug.Log("Dequeuing schedule item " + schedule.First.EventName + " with scheduled step " + schedule.First.ScheduledStep);
            schedule.Dequeue().Activate();
        }
    }

    abstract class BaseScheduleItem {
        public string EventName { get; protected set; }
        public int ScheduledStep { get; protected set; }
        public string CompletionMessage { get; protected set; }

        protected BaseScheduleItem(string eventName, int scheduledStep, string completionMessage = null) {
            EventName = eventName;
            ScheduledStep = scheduledStep;
            CompletionMessage = completionMessage;
        }

        public abstract void Activate();
    }

    class ScheduleItem<T>: BaseScheduleItem { 
        public T Data { get; private set; }

        public ScheduleItem(string eventName, T data, int scheduledStep, string completionMessage = null) : base(eventName, scheduledStep, completionMessage) {
            Data = data;
        }

        public override void Activate() {
            if (typeof(T) == typeof(bool))
            {
                bool data = (bool)Convert.ChangeType(Data, typeof(bool));
                EventManager.TriggerEvent(EventName, data);
            }
            if (typeof(T) == typeof(int))
            {
                int data = (int)Convert.ChangeType(Data, typeof(int));
                EventManager.TriggerEvent(EventName, data);
            }
            if (typeof(T) == typeof(float))
            {
                float data = (float)Convert.ChangeType(Data, typeof(float));
                EventManager.TriggerEvent(EventName, data);
            }
            if (typeof(T) == typeof(Route)) 
            {
                Route data = (Route)Convert.ChangeType(Data, typeof(Route));
                EventManager.TriggerEvent(EventName, data);    
            }
            if (CompletionMessage != null)
            {
                Debug.Log(CompletionMessage);
            }
        }
    }
}
