using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ForceExitCallback(GameObject gameObject);
public enum BehaviorType { None, Movement, ColorChange, LayerOverlay, Animation, Mixed };
[System.Serializable]
public class BehaviorData
{
    public string behaviorName;
    public ForceExitCallback ForceExitCallback;
}
