﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepController : MonoBehaviour
{
    [SerializeField] Text stepText;
    [SerializeField] Text timeText;
    [SerializeField] Button stepButton;
    [SerializeField] Toggle autoToggle;
    [SerializeField] InputField speedInput;

    public static int StepNumber { get; private set; }
    public static bool Auto { get; private set; } = false;
    public static float AutoSpeed { get; private set; } = 5f;

    TimeSpan timeElapsed;
    TimeSpan timePerStep;

    Coroutine autoStepRoutine;

    private void Awake()
    {
        StepNumber = ControlManager.instance.StartingValues.Step;
        timePerStep = new TimeSpan(ControlManager.instance.Times.SecondsPerStep);
        if(StepNumber > 0) {
            timeElapsed = new TimeSpan(StepNumber * timePerStep.TotalSeconds);
        } else {
            timeElapsed = new TimeSpan();
        }

        stepButton.onClick.RemoveListener(Step);
        stepButton.onClick.AddListener(Step);

        autoToggle.onValueChanged.RemoveListener(OnAutoToggle);
        autoToggle.onValueChanged.AddListener(OnAutoToggle);

        speedInput.onValueChanged.AddListener(OnSpeedChange);

        UpdateDisplay();
    }

    void Step() {
        StepNumber++;
        timeElapsed.Increment(timePerStep);
        UpdateDisplay();
        EventManager.TriggerEvent(EventNames.STEP, StepNumber);
    }

    void UpdateDisplay()
    {
        timeText.text = timeElapsed.ToString();
        stepText.text = StepNumber.ToString();
    }

    void OnAutoToggle(bool val) {
        stepButton.interactable = !val;
        speedInput.gameObject.SetActive(val);
        Auto = val;
        if(val) {
            autoStepRoutine = StartCoroutine(AutoStep()); 
        } else {
            StopCoroutine(autoStepRoutine);
        }
    }

    IEnumerator AutoStep() {
        while(Auto) {
            Step();
            yield return new WaitForSeconds(AutoSpeed);
        }
    }

    void OnSpeedChange(string val) {
        float oldVal = AutoSpeed;
        try
        {
            AutoSpeed = float.Parse(val);
        } catch (System.FormatException e) {
            AutoSpeed = oldVal;
        }
    }
}
