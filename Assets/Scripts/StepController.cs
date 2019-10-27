using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepController : MonoBehaviour
{
    public static int step {
        get;
        private set;
    }

    [SerializeField] Text stepText;

    public void Step() {
        step++;
        stepText.text = step.ToString();
        EventManager.TriggerEvent(EventNames.STEP, step);
    }
}
