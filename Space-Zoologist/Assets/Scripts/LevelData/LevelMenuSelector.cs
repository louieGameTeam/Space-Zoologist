using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to pass level data from level select to level scene
// Destroyed by level data loader after use
public class LevelMenuSelector : MonoBehaviour
{
    [HideInInspector] public string levelName = "Level1E1";

    private void Awake()
    {
        // Why on earth are you destroying the level data loader?!
        LevelDataLoader levelDataLoader = FindObjectOfType<LevelDataLoader>();
        if (levelDataLoader != null)
        {
            Destroy(levelDataLoader.gameObject);
        }
        DontDestroyOnLoad(this);
    }
}
