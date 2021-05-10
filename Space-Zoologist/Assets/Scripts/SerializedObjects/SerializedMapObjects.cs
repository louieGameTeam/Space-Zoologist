using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedMapObjects
{
    public string[] names;
    private List<string> typeList = new List<string>();
    public GridItemSet[] gridItemSets;
    private List<GridItemSet> gridItemSetList = new List<GridItemSet>();
    public void AddType(string name, GridItemSet gridItemSet)
    {
        this.typeList.Add(name);
        this.names = this.typeList.ToArray();
        this.gridItemSetList.Add(gridItemSet);
        this.gridItemSets = this.gridItemSetList.ToArray();
    }
    public Dictionary<string , GridItemSet> ToDictionary()
    {
        Dictionary<string, GridItemSet> dict = new Dictionary<string, GridItemSet>();
        if (this.names != null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                dict.Add(names[i], gridItemSets[i]);
            }
        }

        return dict;
    }
}
