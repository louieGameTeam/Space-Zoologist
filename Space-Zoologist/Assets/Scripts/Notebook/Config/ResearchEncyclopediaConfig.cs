using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Notebook/Encyclopedia")]
public class ResearchEncyclopediaConfig : ScriptableObject
{
    #region Public Properties
    public List<ResearchEncyclopediaArticleConfig> Articles => articles;
    #endregion

    #region Private Editor Fields
    // Private editor data
    [SerializeField]
    [FormerlySerializedAs("articlesList")]
    [Tooltip("List of all the articles in this encyclopedia")]
    private List<ResearchEncyclopediaArticleConfig> articles = null;
    #endregion

    #region Public Methods
    // Get the article with the given ID
    public ResearchEncyclopediaArticleConfig GetArticle(ResearchEncyclopediaArticleID id)
    {
        return articles.Find(article => article.ID == id);
    }
    public ResearchEncyclopediaArticleConfig GetArticle(int index)
    {
        if (index >= 0 && index < articles.Count) return articles[index];
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEncyclopediaConfig)}: " +
            $"attempting to get article at index {index} on encyclopedia '{name}', " +
            $"but no such article exists");
    }
    #endregion
}
