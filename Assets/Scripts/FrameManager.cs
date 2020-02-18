using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FrameManager : MonoBehaviour
{
    [SerializeField] Text frameCountText;
    [SerializeField] Button frameAddButton;
    [SerializeField] List<CellManager> cellsManagers = new List<CellManager>();

    public int Frames { get; private set; } = 1;

    private void Awake()
    {
        frameAddButton.onClick.RemoveListener(OnFrameAddButtonPress);
        frameAddButton.onClick.AddListener(OnFrameAddButtonPress);

        UpdateDisplay();
    }

    void UpdateDisplay() {
        cellsManagers.ForEach(cm => cm.UpdateDisplay());
        frameCountText.text = $"({Frames})";
    }

    void OnFrameAddButtonPress() {
        Frames++;
        UpdateDisplay();
    }
}
