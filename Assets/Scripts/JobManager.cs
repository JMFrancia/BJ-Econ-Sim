using UnityEngine;
using System.Collections;

public class JobManager : MonoBehaviour
{
    public static JobManager instance;

    public FrameManager ForagingFrameManager {
        get {
            return foragingFrameManager;
        }
    }

    public FrameManager NursingFrameManager
    {
        get
        {
            return nursingFrameManager;
        }
    }

    public CellManager ForagingCellManager { 
        get { 
            return foragingCellManager; 
        } 
    }

    public CellManager NursingCelLManager
    {
        get
        {
            return nursingCellManager;
        }
    }

    public CellManager BakeryCellManager
    {
        get
        {
            return bakeryCellManager;
        }
    }

    [SerializeField] FrameManager foragingFrameManager;
    [SerializeField] FrameManager nursingFrameManager;

    [SerializeField] CellManager foragingCellManager;
    [SerializeField] CellManager nursingCellManager;
    [SerializeField] CellManager bakeryCellManager;

    private void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }
}
