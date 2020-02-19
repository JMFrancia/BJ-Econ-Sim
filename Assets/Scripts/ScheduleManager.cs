using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    public static ScheduleManager instance;

    SimplePriorityQueue<BaseScheduleItem> schedule = new SimplePriorityQueue<BaseScheduleItem>();
    Dictionary<int, BaseScheduleItem> itemDict = new Dictionary<int, BaseScheduleItem>();

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

    public bool RemoveScheduleItem(int id) { 
        if(itemDict.ContainsKey(id)) {
            return schedule.TryRemove(itemDict[id]);
        }
        return false;
    }

    //Returns ID of new schedule item
    public int AddScheduleItem(int steps, Action callback, string startingMessage = null, string completionMessage = null) { 
        if(steps < 0) {
            //Invalid start time
            return -1;
        }
        if (steps == 0) {
            callback.Invoke();
        }
        int scheduledStep = steps + StepController.StepNumber;
        ScheduleItem item = new ScheduleItem(callback, scheduledStep, completionMessage);
        schedule.Enqueue(item, scheduledStep);
        itemDict[item.ID] = item;
        if(startingMessage != null) {
            LogManager.instance.AddToLog(startingMessage);
        }
        return item.ID;
    }

    //Not really used, keeping in case of future use.
    //Needs to be updated to use itemDict
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
        LogManager.instance.AddToLog($"Scheduled {eventName} with {param} for step {scheduledStep}");
    }

    void OnStep(int stepNumber) {
        while (schedule.Count > 0 && stepNumber >= schedule.First.ScheduledStep) {
            BaseScheduleItem item = schedule.Dequeue();
            itemDict.Remove(item.ID);
            item.Activate();
        }
    }

    abstract class BaseScheduleItem {
        public string EventName { get; protected set; }
        public int ScheduledStep { get; protected set; }
        public string CompletionMessage { get; protected set; }
        public int ID { get; protected set; }

        protected BaseScheduleItem(string eventName, int scheduledStep, string completionMessage = null) {
            EventName = eventName;
            ScheduledStep = scheduledStep;
            CompletionMessage = completionMessage;
        }

        public abstract void Activate();
    }

    [Serializable]
    class ScheduleItem : BaseScheduleItem { 
        public Action CallBack { get; private set; }

        static int currentID = 0;
        const int MAX_ID = 9999999;  //Reflects max # of items to schedule

        public ScheduleItem(Action callBack, int scheduledStep, string completionMessage = null) : base("", scheduledStep, completionMessage) {
            CallBack = callBack;
            ID = currentID++ % MAX_ID;
        }

        public override void Activate()
        {
            LogManager.instance.AddToLog(CompletionMessage);
            CallBack.Invoke();
        }
    }

    //Not really used, keeping in case of future use
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
                LogManager.instance.AddToLog(CompletionMessage);
            }
        }
    }
}
