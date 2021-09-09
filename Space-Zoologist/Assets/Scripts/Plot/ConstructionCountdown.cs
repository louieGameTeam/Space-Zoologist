using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionCountdown
{
    public int progress;
    public int target;
    public void Initialize(Vector2Int pos, int time, int progress = -1)
    {
        this.progress = progress == -1 ? 0 : progress;
        this.target = time;
    }
    public void CountDown()
    {
        this.progress += 1;
    }
    public bool IsFinished()
    {
        return progress >= target;
    }
}
