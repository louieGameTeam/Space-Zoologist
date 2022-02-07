using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataEditor
{
    #region Public Fields
    public static readonly string notebookFileName = "sz.notebook";
    public static readonly string notebookFilePath = Path.Combine(Application.persistentDataPath, notebookFileName);
    #endregion

    #region Public Methods
    [MenuItem("File/Delete Save Data")]
    public static void DeleteSaveData() => SaveData.Delete();
    [MenuItem("File/Delete Save Data", true)]
    public static bool SaveDataExists() => SaveData.SaveFileExists();
    [MenuItem("File/Report File Path")]
    public static void ReportFilePath() => Debug.Log(SaveData.filePath);

    [MenuItem("File/Delete Notebook Data")]
    public static void DeleteNotebookData() => File.Delete(notebookFilePath);
    [MenuItem("File/Delete Notebook Data", true)]
    public static bool NotebookDataExists() => File.Exists(notebookFilePath);
    [MenuItem("File/Report Notebook File Path")]
    public static void ReportNotebookFilePath() => Debug.Log(notebookFilePath);
    #endregion
}
