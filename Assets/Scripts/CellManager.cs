using UnityEngine;
using UnityEngine.UI;

public class CellManager : MonoBehaviour
{
    [SerializeField] int cellsPerFrame = 4;

    [SerializeField] Button cellAddButton;
    [SerializeField] Text cellCountText;

    public int Cells { get; private set; } = 0;
    public int ActiveCells { get; private set; }

    FrameManager frame;

    private void Awake()
    {
        frame = GetComponentInParent<FrameManager>();
        cellAddButton.onClick.RemoveListener(OnCellAddButtonPress);
        cellAddButton.onClick.AddListener(OnCellAddButtonPress);
    }

    public bool HasFreeCell()
    {
        return ActiveCells < Cells;
    }

    public bool ActivateCell()
    {
        if (HasFreeCell())
        {
            ActiveCells++;
            UpdateDisplay();
            return true;
        }
        return false;
    }

    public void DeactivateCell()
    {
        ActiveCells = Mathf.Max(ActiveCells - 1, 0);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        //Solves race condition where UpdateDisplay() called before Awake()
        if(frame == null) {
            frame = GetComponentInParent<FrameManager>();
        }
        cellCountText.text = $"{ActiveCells} / {Cells} / {frame.Frames * cellsPerFrame}";
    }

    void OnCellAddButtonPress()
    {
        Cells = Mathf.Min(Cells + 1, frame.Frames * cellsPerFrame);
        UpdateDisplay();
    }
}
