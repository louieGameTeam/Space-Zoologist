using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionCountdown : MonoBehaviour
{
    public Vector2Int position;
    public int progress;
    public int target;
    private Material material;
    public void Initialize(Vector2Int pos, int time, int progress = -1)
    {
        this.position = pos;
        this.progress = progress == -1 ? time : progress;
        this.target = time;
        this.material = this.gameObject.GetComponent<SpriteRenderer>().material;
        this.material.SetFloat("Target", this.target);
    }
    public void CountDown()
    {
        this.progress += 1;
        this.material.SetFloat("Progress", this.progress);
        // TODO animate material
    }
    public bool IsFinished()
    {
        if (progress >= target)
        {
            return true;
        }
        return false;
    }

}
