using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TestAndMetricsConfig
{
    #region Public Properties
    public LevelScaffold Scaffold => scaffold;
    public List<string> InitialTexts => initialTexts;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [FormerlySerializedAs("enclosureScaffold")]
    [Tooltip("Enclosure scaffolding information")]
    private LevelScaffold scaffold = null;
    [SerializeField]
    [TextArea(3, 10)]
    [Tooltip("Initial text for each scaffold level")]
    private List<string> initialTexts = null;
    #endregion

    #region Public Methods
    public string GetInitialText(LevelID id) => initialTexts[scaffold.ScaffoldLevel(id)];
    #endregion
}
