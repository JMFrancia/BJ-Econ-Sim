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

    public FrameManager HoneyFrameManager { 
        get {
            return honeyFrameManager;
        }
    }

    public CellManager ForagingCellManager { 
        get { 
            return foragingCellManager; 
        } 
    }

    public CellManager NursingCellManager
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

    public CellManager HoneyCellManager { 
        get {
            return honeyCellManager;
        }
    }

    [SerializeField] FrameManager foragingFrameManager;
    [SerializeField] FrameManager nursingFrameManager;
    [SerializeField] FrameManager honeyFrameManager;

    [SerializeField] CellManager foragingCellManager;
    [SerializeField] CellManager nursingCellManager;
    [SerializeField] CellManager bakeryCellManager;
    [SerializeField] CellManager honeyCellManager;

    private void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }
}
