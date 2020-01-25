using UnityEngine;
using System.Collections;

public class JobManager : MonoBehaviour
{
    public static JobManager instance;

    [SerializeField]
    public FrameCellManager ForagingFrameManager { 
        get { 
            return foragingFrameManager; 
        } 
    }

    [SerializeField]
    FrameCellManager foragingFrameManager;

    private void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }
}
