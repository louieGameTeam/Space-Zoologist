using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Modify in the editor or via code to change an animal's behavior.
/// </summary>
/// TODO add more unique fields as more behaviors are created. Consider creating specific data scripts if this becomes too unorganized.
public enum Movement { idle = 0, walking = 1, running = 2, eating = 3 }
public enum Direction { up = 0, down = 1, left = 2, right = 3, upRight = 4, upLeft = 5, downRight = 6, downLeft = 7 }
[System.Serializable]
public class MovementData
{
    public Movement MovementStatus = Movement.idle;
    public Direction CurrentDirection = Direction.down;
    [SerializeField] private Dictionary<string, float> TagsToMultiplicationSpeedModifiers = new Dictionary<string, float>();
    [SerializeField] private Dictionary<string, float> TagsToAdditionSpeedModifiers = new Dictionary<string, float>();
    [SerializeField] public float BaseSpeed = 3f; // The base speed before applying all the modifiers, usually does not change in runtime
    [SerializeField] public float RunThreshold = 2f; // If the speed after applying all the modifiers exceeds this value, then animation will be shown as running
    [SerializeField] public float Speed = 0f; // The actual speed the animal is moving
    public bool IsModifierChanged = false;
    /*Used for optimization. If modifier is unchanged, MovementController will use the buffered speed stored in its self without doing calculation every update.
    This works based on the assumption that only the MovementController needs this information and Calling CalculateModifiedSpeed() in the MovementController will reset this value to false once done a new calculation.
    If a new system need this information, try get it from the MovementController instead or other work around.*/
    /// <summary>
    /// Adds a new multiplication modifier
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="multiplier"></param>
    public void AddMultiplicationSpeedModifiers(string tag, float multiplier)
    {
        IsModifierChanged = true;
        if (TagsToMultiplicationSpeedModifiers.ContainsKey(tag))
        {
            TagsToMultiplicationSpeedModifiers[tag] = multiplier;
        }
        else
        {
            TagsToMultiplicationSpeedModifiers.Add(tag, multiplier);
        }
    }
    /// <summary>
    /// Adds a new Addition modifier. To subtract, just use a negative value
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="multiplier"></param>
    public void AddAdditionSpeedModifiers(string tag, float adder)
    {
        IsModifierChanged = true;
        if (TagsToAdditionSpeedModifiers.ContainsKey(tag))
        {
            TagsToAdditionSpeedModifiers[tag] = adder;
        }
        else
        {
            TagsToAdditionSpeedModifiers.Add(tag, adder);
        }
    }
    public void RemoveSpeedModifier(string tag)
    {
        IsModifierChanged = true;
        if (TagsToAdditionSpeedModifiers.ContainsKey(tag))
        {
            TagsToAdditionSpeedModifiers.Remove(tag);
            return;
        }
        if (TagsToMultiplicationSpeedModifiers.ContainsKey(tag))
        {
            TagsToMultiplicationSpeedModifiers.Remove(tag);
            return;
        }
        Debug.LogError("Modifier not found");

    }
    /// <summary>
    /// Returns the moving speed taking consideration all the modifiers. Additions first, then multiplications
    /// </summary>
    /// <returns></returns>
    public float CalculateModifiedSpeed()
    {
        IsModifierChanged = false;
        float multiplier = 1;
        foreach (float value in TagsToMultiplicationSpeedModifiers.Values)
        {
            multiplier *= value;
        }
        float adder = 0;
        foreach (float value in TagsToAdditionSpeedModifiers.Values)
        {
            adder += value;
        }
        return (BaseSpeed + adder) * multiplier; //Addition before multiplication
    }
}
