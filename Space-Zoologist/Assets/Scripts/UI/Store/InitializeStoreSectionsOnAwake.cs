using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeStoreSectionsOnAwake : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of the store sections to initialize")]
    private StoreSection[] sections;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        foreach(StoreSection section in sections)
        {
            section.Initialize();
        }
    }
    #endregion
}
