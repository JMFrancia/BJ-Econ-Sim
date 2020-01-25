using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameCellManager : MonoBehaviour
{
    const int CELLS_PER_FRAME = 8;

    [SerializeField] Text cellCountText;
    [SerializeField] Text frameCountText;
    [SerializeField] Button cellAddButton;
    [SerializeField] Button frameAddButton;

    public int Frames { get; private set; } = 1;
    public int Cells { get; private set; } = 3;
    public int ActiveCells { get; private set; }

    public bool HasFreeCell() {
        return ActiveCells < Cells;
    }

    public bool ActivateCell() { 
        if(HasFreeCell()) {
            ActiveCells++;
            UpdateDisplay();
            return true;
        }
        return false;
    }

    public void DeactivateCell() {
        ActiveCells = Mathf.Max(ActiveCells - 1, 0);
        UpdateDisplay();
    }

    private void Awake()
    {
        frameAddButton.onClick.RemoveListener(OnFrameAddButtonPress);
        frameAddButton.onClick.AddListener(OnFrameAddButtonPress);

        cellAddButton.onClick.RemoveListener(OnCellAddButtonPress);
        cellAddButton.onClick.AddListener(OnCellAddButtonPress);

        UpdateDisplay();
    }

    void UpdateDisplay() {
        cellCountText.text = $"{ActiveCells} / {Cells} / {Frames * CELLS_PER_FRAME}";
        frameCountText.text = $"({Frames})";
    }

    void OnCellAddButtonPress()
    {
        Cells = Mathf.Min(Cells + 1, Frames * CELLS_PER_FRAME);
        UpdateDisplay();
    }

    void OnFrameAddButtonPress() {
        Frames++;
        UpdateDisplay();
    }


}
