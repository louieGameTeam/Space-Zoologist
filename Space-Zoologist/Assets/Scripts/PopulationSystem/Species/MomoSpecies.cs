using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AnimalSpecies/Momo")]
public class MomoSpecies : AnimalSpecies
{
    [SerializeField] List<SocialNeedConstructData> socialNeed = default;
    [SerializeField] List<TreeTerrainConstructData> treeTerrainNeed = default;
}
