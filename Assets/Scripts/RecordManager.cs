using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    //Column labels
    const string STEP_LABEL = "Step";
    const string WORKERS_LABEL = "Workers";
    const string NECTAR_LABEL = "Nectar";
    const string POLLEN_LABEL = "Pollen";
    const string FORAGERS_LABEL = "Foragers";
    const string FORAGING_CELLS_LABEL = "Foraging cells";
    const string FORAGING_FRAMES_LABEL = "Foraging Frames";

    struct Record {
        public int step;
        public int workers;
        public int nectar;
        public int pollen;
        public int foragers;
        public int foragingCells;
        public int foragingFrames;

        public Record(int step, int workers, int nectar, int pollen, int foragers, int foragingCells, int foragingFrames) {
            this.step = step;
            this.workers = workers;
            this.nectar = nectar;
            this.pollen = pollen;
            this.foragers = foragers;
            this.foragingCells = foragingCells;
            this.foragingFrames = foragingFrames;
        }

        public Dictionary<string, int> Get() {
            return new Dictionary<string, int>
            {
                {STEP_LABEL, step},
                {WORKERS_LABEL, workers},
                {NECTAR_LABEL, nectar},
                {POLLEN_LABEL, pollen},
                {FORAGERS_LABEL, foragers},
                {FORAGING_CELLS_LABEL, foragingCells},
                {FORAGING_FRAMES_LABEL, foragingFrames}
            };
        }
    }

    List<Record> AllRecords = new List<Record>();

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.STEP, RecordStep);
    }

    private void Disable()
    {
        EventManager.StartListening(EventNames.STEP, RecordStep);
    }

    void RecordStep(int step) {
        Record record = new Record(
            step: step,
            workers: ResourceManager.Workers,
            nectar: ResourceManager.Nectar,
            pollen: ResourceManager.Pollen,
            foragers: JobManager.instance.ForagingFrameManager.ActiveCells,
            foragingCells: JobManager.instance.ForagingFrameManager.Cells,
            foragingFrames: JobManager.instance.ForagingFrameManager.Frames
        );

        AllRecords.Add(record);
    }

    void WriteRecordsToCSV(bool vertical = true) {

        string totalRecord;

        if (vertical)
        {
            totalRecord = $"{STEP_LABEL},{WORKERS_LABEL},{NECTAR_LABEL},{POLLEN_LABEL},{FORAGERS_LABEL},{FORAGING_CELLS_LABEL},{FORAGING_FRAMES_LABEL}\n";
            foreach (Record r in AllRecords) {
                totalRecord += $"{r.step},{r.workers},{r.nectar},{r.pollen},{r.foragers},{r.foragingCells},{r.foragingFrames}\n";
            }
        }
        else
        {
            string stepRow = $"{STEP_LABEL},";
            string workersRow = $"{WORKERS_LABEL},";
            string nectarRow = $"{NECTAR_LABEL},";
            string pollenRow = $"{POLLEN_LABEL},";
            string foragersRow = $"{FORAGERS_LABEL},";
            string foragingCellsRow = $"{FORAGING_CELLS_LABEL},";
            string foragingFramesRow = $"{FORAGING_FRAMES_LABEL},";

            foreach (Record r in AllRecords)
            {
                Dictionary<string, int> thisRecord = r.Get();
                stepRow += $"{r.step},";
                workersRow += $"{r.workers},";
                nectarRow += $"{r.nectar},";
                pollenRow += $"{r.pollen},";
                foragersRow += $"{r.foragers},";
                foragingCellsRow += $"{r.foragingCells},";
                foragingFramesRow += $"{r.foragingFrames},";
            }

            totalRecord = $"{stepRow}\n{workersRow}\n{nectarRow}\n{pollenRow}\n{foragersRow}\n{foragingCellsRow}\n{foragingFramesRow}";
        }


        string fullPath = GetPath() + GenerateFileName();


        Debug.Log($"Writing session record to {fullPath}...");
        File.WriteAllText(fullPath, totalRecord);
        Debug.Log($"Record complete");
    }

    string GetPath() {
        #if UNITY_EDITOR
            return Application.dataPath + "/RecordedSessions/";
        #else
            return null;
        #endif
    }

    string GenerateFileName() {
        System.DateTime now = System.DateTime.Now;
        return $"{now.ToString("MMM", System.Globalization.CultureInfo.InvariantCulture)}_{now.Day}___{now.Hour}-{now.Minute}{now.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}___{AllRecords.Count}-Steps.csv";
    }

    private void OnApplicationQuit()
    {
        WriteRecordsToCSV();
    }
}
