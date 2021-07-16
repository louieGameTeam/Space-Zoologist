using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedMapObjects : IEnumerable<KeyValuePair<string, GridItemSet>>
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
    public IEnumerator<KeyValuePair<string, GridItemSet>> GetEnumerator()
    {
        this.names = this.names ?? new string[0];
        for (int i = 0; i < names.Length; i++)
        {
            yield return new KeyValuePair<string, GridItemSet>(names[i], gridItemSets[i]);
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
