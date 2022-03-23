using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple wrapper around the float[] content of a liquidbody
/// </summary>
/// <remarks>
/// This makes it easy to compare liquid bodies by their contents. 
/// The Equals() method checks to make sure the float[] elements
/// are all equal
/// </remarks>
[System.Serializable]
public class LiquidBodyContent
{
    #region Public Properties
    public float[] Contents => contents;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("The content in the liquid body")]
    private float[] contents;
    #endregion

    #region Constructors
    public LiquidBodyContent(float[] contents)
    {
        this.contents = contents;
    }
    #endregion

    #region Object Overrides
    public override bool Equals(object other)
    {
        // If the other is null then it is not equal to this object
        if (ReferenceEquals(other, null)) return false;
        // If the types are equal then check if the contents are equal
        else if (GetType() == other.GetType())
        {
            LiquidBodyContent otherContent = other as LiquidBodyContent;
            return contents.SequenceEqual(otherContent.contents);
        }
        // If the types are unequal then the objects are unequal
        else return false;
    }
    public override int GetHashCode()
    {
        int hash = 0;

        // Use XOR so that we do not get arithmetic overflows
        foreach (float f in contents)
        {
            hash ^= f.GetHashCode();
        }

        return hash;
    }
    #endregion
}
