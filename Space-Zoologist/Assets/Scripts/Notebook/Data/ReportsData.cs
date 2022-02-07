using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReportsData : NotebookDataModule
{
    #region Public Typedefs
    [System.Serializable]
    public class Entry
    {
        public LevelID level;
        public QuizInstance quiz;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of levels and the quiz that player last took with them")]
    private List<Entry> entries = new List<Entry>();
    #endregion

    #region Constructors
    public ReportsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public QuizInstance GetQuiz(LevelID level)
    {
        Entry entry = GetEntry(level);

        // If you got an entry return its quiz
        if (entry != null) return entry.quiz;
        // If you did not get an entry then return null
        else return null;
    }
    public void SetQuiz(LevelID level, QuizInstance quiz)
    {
        Entry entry = GetEntry(level);

        // If entry already exists then set its quiz
        if (entry != null)
        {
            entry.quiz = quiz;
        }
        // If no entry yet exists then add it here
        else
        {
            Debug.Log("New quiz added to reports");
            entries.Add(new Entry
            {
                level = level,
                quiz = quiz
            });
        }
    }
    #endregion

    #region Private Methods
    private Entry GetEntry(LevelID level) => entries.Find(entry => entry.level == level);
    #endregion
}
