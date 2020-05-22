using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JournalData", menuName = "Journal/JournalData")]
public class JournalData : ScriptableObject
{
    [Expandable] public List<JournalEntry> Entries = default;
}
