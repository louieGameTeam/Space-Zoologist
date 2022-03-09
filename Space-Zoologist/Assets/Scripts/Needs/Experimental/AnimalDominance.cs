using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AnimalDominance))]
public class AnimalDominance : ScriptableObjectSingleton<AnimalDominance>
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Measures how much each animal dominates other animals " +
        "when competing for the same terrain space")]
    private TerrainDominance terrainDominance;
    [SerializeField]
    [Tooltip("List of dominance amounts for each animal " +
        "competing for shared food sources")]
    private AnimalDominanceItem[] foodDominance;
    #endregion
}
