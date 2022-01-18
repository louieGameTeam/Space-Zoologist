using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataEditor
{
    #region Public Methods
    [MenuItem("File/Delete Save Data")]
    public static void DeleteSaveData() => SaveData.Delete();
    [MenuItem("File/Delete Save Data", true)]
    public static bool SaveDataExists() => SaveData.SaveFileExists();
    [MenuItem("File/Report File Path")]
    public static void ReportFilePath() => Debug.Log(SaveData.filePath);
    #endregion
}
