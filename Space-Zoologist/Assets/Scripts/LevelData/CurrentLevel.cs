using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to pass level data from level select to level scene, destroyed by level data loader after use
public class CurrentLevel : MonoBehaviour
{
    [HideInInspector] public string levelName = "Level1E1";

    private void Awake()
    {
        LevelDataLoader levelDataLoader = FindObjectOfType<LevelDataLoader>();
        if (levelDataLoader != null)
        {
            Destroy(levelDataLoader);
        }
        DontDestroyOnLoad(this);
    }
}
