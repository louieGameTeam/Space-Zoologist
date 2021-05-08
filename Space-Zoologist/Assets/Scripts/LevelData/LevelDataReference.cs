using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataReference : MonoBehaviour
{
    public int MapWidth => LevelData.MapWidth;
    public int MapHeight => LevelData.MapHeight;
    [Expandable] public LevelData LevelData = default;
}
