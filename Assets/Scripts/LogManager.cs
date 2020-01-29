using UnityEngine;
using System.Collections.Generic;

public class LogManager : MonoBehaviour
{
    public static LogManager instance;

    List<string> Log = new List<string>();

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.STEP, OnStep);
    }

    private void OnDisable()
    {
        EventManager.StartListening(EventNames.STEP, OnStep);
    }

    public void DisplayLogAtCurrentStep() {
        DisplayLogAtStep(StepController.StepNumber);
    }

    void OnStep(int stepNumber) {
        DisplayLogAtStep(stepNumber - 1);
    }

    public void AddToLog(string msg) {
        FillBlanksInLog();
        Log[StepController.StepNumber] += $"* {msg}\n";
    }

    //Returns copy
    public List<string> GetLog()
    {
        return new List<string>(Log);
    }

    void DisplayLogAtStep(int step) {
        string logAtStep = GetLogAtStep(step);
        if(!string.IsNullOrEmpty(logAtStep))
        {
            Debug.Log($"Step {step}:\n{logAtStep}");
        }
    }

    public string GetLogAtStep(int step) {
        if(step > StepController.StepNumber && step >= 0)
        {
            Debug.LogError($"Attempting to get log from invalid step {step}");
            return null;
        }
        FillBlanksInLog();
        return Log[step];
    }

    void FillBlanksInLog() {
        while (StepController.StepNumber > Log.Count - 1)
        {
            Log.Add("");
        }
    }
}
