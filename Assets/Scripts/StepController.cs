using System.Collections;
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

    public static int StepNumber { get; private set; } = 0;
    public static bool Auto { get; private set; } = false;
    public static float AutoSpeed { get; private set; } = 5f;

    TimeSpan timeElapsed = new TimeSpan();
    TimeSpan timePerStep = new TimeSpan(300);

    Coroutine autoStepRoutine;

    private void Awake()
    {
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
        Debug.Log("Beginning auto step with speed " + AutoSpeed);
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
        Debug.Log("Changing auto step speed to " + AutoSpeed);
    }
}
