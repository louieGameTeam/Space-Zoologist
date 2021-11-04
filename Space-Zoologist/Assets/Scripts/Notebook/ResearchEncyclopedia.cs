using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/Encyclopedia")]
public class ResearchEncyclopedia : ScriptableObject
{
    #region Public Properties
    // Public accessors
    public Dictionary<ResearchEncyclopediaArticleID, ResearchEncyclopediaArticle> Articles => articles;
    #endregion

    #region Private Editor Fields
    // Private editor data
    [SerializeField]
    [Tooltip("List of all the articles in this encyclopedia")]
    private List<ResearchEncyclopediaArticle> articlesList;
    #endregion

    #region Private Fields
    // Private data
    [Tooltip("Maps the id of the articles to the article itself for faster lookup")]
    private Dictionary<ResearchEncyclopediaArticleID, ResearchEncyclopediaArticle> articles = new Dictionary<ResearchEncyclopediaArticleID, ResearchEncyclopediaArticle>();
    #endregion

    #region Public Methods
    public void Setup()
    {
        foreach(ResearchEncyclopediaArticle article in articlesList)
        {
            article.Setup();
            if (!articles.ContainsKey(article.ID)) articles.Add(article.ID, article);
        }
    }
    // Get the article with the given ID
    public ResearchEncyclopediaArticle GetArticle(ResearchEncyclopediaArticleID id) => Articles[id];
    public ResearchEncyclopediaArticle GetArticle(int index)
    {
        if (index >= 0 && index < articlesList.Count) return articlesList[index];
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEncyclopedia)}: " +
            $"attempting to get article at index {index} on encyclopedia '{name}', " +
            $"but no such article exists");
    }
    #endregion
}
