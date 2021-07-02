[System.Serializable]
public class JournalNeedResearch
{
    public string NeedName;
    public string NeedDescription;
    public string ResearchStatus;

    public JournalNeedResearch(string needName)
    {
        this.NeedName = needName;
        this.NeedDescription = "";
        this.ResearchStatus = "unresearched";
    }
}
