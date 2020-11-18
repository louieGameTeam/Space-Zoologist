using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
/// <summary>
/// Temporary Class for testing only, do not integrate
/// </summary>
public class GridSaver : MonoBehaviour
{
    private TilePlacementController tilePlacementController;
    [SerializeField] private float savingCD = 1f;
    [SerializeField] private string directory = "Assets/SaveData/Grid/";
    private string sceneName;
    private string fullPath;
    private float CD = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        this.tilePlacementController = this.gameObject.GetComponent<TilePlacementController>();
        this.sceneName = SceneManager.GetActiveScene().name;
        this.fullPath = directory + sceneName + ".json";
    }

    // Update is called once per frame
    void Update()
    {
        CD = CD > 0f ? CD - Time.deltaTime : 0;
        if (Input.GetKeyDown(KeyCode.P) && CD <= 0f)
        {
            CD = savingCD;
            SaveGrid();
        }
    }
    private void SaveGrid()
    {
        Debug.Log("Saving Grid to " + fullPath);
        if (File.Exists(fullPath))
        {
            Debug.Log("Overwriting file at " + fullPath);
        }
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(JsonUtility.ToJson(this.tilePlacementController.Serialize()));
        }
    }
}
