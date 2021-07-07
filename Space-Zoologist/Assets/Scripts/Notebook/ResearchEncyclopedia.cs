using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/Encyclopedia")]
public class ResearchEncyclopedia : ScriptableObject
{
    [SerializeField]
    private List<ResearchEncyclopediaArticle> articles;
}
