using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
/// <summary>
/// The data of a single bookmark. Holds the name of the game object
/// that has a BookmarkTarget component attached, and holds the data
/// that should be set on that bookmark target. 
/// NOTE: this must be  a class to use reflection properly
/// </summary>
public class BookmarkData
{
    #region Public Fields
    // Every possible type must be stored as a separate variable
    // in order to be serializable. Other solutions considered:
    // - Convert these structs to classes and make them inherit from a serializable base class
    //      This was rejected because these types are structs. They were made structs so that
    //      they could not be null so that their == operators worked correctly. Alternatively,
    //      they could be made classes again and the == operators can check for null. But
    //      it's also problematic to make them all inherit from a base class related to bookmarking.
    //      It's preferrable if the other types don't "know" that they are bookmarkable so they can
    //      inherit from any other base class that they might need to inherit from

    // IMPORTANT: do not name ANY OTHER public fields with the word "data" in them,
    // unless you want that data to be able to be set on a NotebookUI component.
    // The class uses reflection to find fields with the word "data" in them

    public NotebookTab data0;
    public ItemID data1;
    public LevelID data2;
    public ResearchEncyclopediaArticleID data3;
    #endregion

    #region Private Editor Fields
    // The name of the game object that has a bookmark target component
    // NOTE: we can't just store the bookmark target itself, it is destroyed on scene load
    [SerializeField]
    private string targetGameObjectName;
    // Index of the data that is used by the bookmark data
    [SerializeField]
    private int index;    
    #endregion

    #region Constructors
    public BookmarkData(string targetGameObjectName, object targetComponentData)
    {
        this.targetGameObjectName = targetGameObjectName;

        // Set the data to default values
        index = -1;
        data0 = 0;
        data1 = ItemID.Invalid;
        data2 = LevelID.Invalid;
        data3 = ResearchEncyclopediaArticleID.Empty;

        // Set the bookmark data
        SetData(targetComponentData);
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Create a bookmark data that holds the information
    /// of a specific bookmark target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static BookmarkData Create(BookmarkTarget target)
    {
        return new BookmarkData(target.name, target.GetTargetComponentData());
    }
    #endregion

    #region Public Methods
    public void SetTargetData(Dictionary<string, BookmarkTarget> nameTargetMap)
    {
        if (nameTargetMap.ContainsKey(targetGameObjectName))
        {
            // Set the data on the bookmark target
            object data = GetData();
            BookmarkTarget target = nameTargetMap[targetGameObjectName];
            target.SetTargetComponentData(data);
        }
        else Debug.LogWarning($"{nameof(BookmarkData)}: " +
            $"cannot set the data on any bookmark target because " +
            $"no bookmark target could be found with the name {targetGameObjectName}. " +
            $"Bookmark targets given: {string.Join(", ", nameTargetMap.Values)}");
    }
    // List all fields with the word "data" in them
    public FieldInfo[] GetDataFields()
    {
        Type myType = GetType();
        return myType
            .GetFields()
            .Where(field => field.Name.Contains("data"))
            .ToArray();
    }
    public Type[] SupportedDataTypes() => GetDataFields()
        .Select(field => field.FieldType)
        .ToArray();
    public int CountDataFields() => GetDataFields().Length;
    public bool HasData()
    {
        int dataCount = CountDataFields();

        // Check if local index identifies a data field
        return index >= 0 && index < dataCount;
    }
    public object GetData()
    {
        int dataCount = CountDataFields();

        // Check if index is valid
        if (HasData())
        {
            // Get the value of the reflected field
            Type myType = GetType();
            string fieldName = $"data{index}";
            FieldInfo field = myType.GetField(fieldName);

            // If we got a field with that name then return its value
            if (field != null) return field.GetValue(this);
            else throw new MissingFieldException($"{nameof(BookmarkData)}: " +
                $"expected a field named '{fieldName}' in this class " +
                $"but no such field could be found. A field with this name " +
                $"should be added to the source code, or the index should not " +
                $"be set to the number {index}");
        }
        else throw new IndexOutOfRangeException($"{nameof(BookmarkData)}: " +
            $"no data exists at index {index}. Total data fields: {dataCount}");
    }
    public void SetData(object data)
    {
        if (data != null)
        {
            // List the data fields
            FieldInfo[] dataFields = GetDataFields();
            Type dataType = data.GetType();

            // Find a field with the same type as the object we are trying to set to
            int index = Array.FindIndex(dataFields, field => field.FieldType == dataType);

            // If a field was found, then set its value
            if (index >= 0)
            {
                dataFields[index].SetValue(this, data);
                this.index = index;
            }
            else Debug.LogWarning($"{nameof(BookmarkData)}: " +
                $"SetData failed because no data field exists " +
                $"with type {dataType}. Please create a data field " +
                $"with this type in the source code for {nameof(BookmarkData)} " +
                $"or pass in an object with a supported type. " +
                $"Supported types: {string.Join<Type>(", ", SupportedDataTypes())}");
        }
        else Debug.LogWarning($"{nameof(BookmarkData)}: " +
            $"cannot set the bookmark data to null");
    }
    #endregion

    #region Object Overrides
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        else if (obj.GetType() == GetType()) return this == (BookmarkData)obj;
        else return false;
    }
    public override int GetHashCode()
    {
        int dataHash = 0;

        // If this has data then get its hash
        if (HasData()) dataHash = GetData().GetHashCode();

        // Combine name and data hash
        return targetGameObjectName.GetHashCode() + dataHash;
    }
    #endregion

    #region Operators
    public static bool operator==(BookmarkData a, BookmarkData b)
    {
        // Check if a and b are not null
        if (!ReferenceEquals(a, null) && !ReferenceEquals(b, null))
        {
            if (a.HasData() && b.HasData())
            {
                // They are equal if they target the same game object
                // with the same data
                return a.targetGameObjectName == b.targetGameObjectName &&
                    a.GetData().Equals(b.GetData());
            }
            // If one has data and the other does not, then they cannot be equal
            else if (a.HasData() != b.HasData()) return false;
            // If neither has data then only check the game object name
            else return a.targetGameObjectName == b.targetGameObjectName;
        }
        // If both are null they are considered equal
        else if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        // If only one is null they cannot be equal
        else return false;
    }
    public static bool operator!=(BookmarkData a, BookmarkData b)
    {
        return !(a == b);
    }
    #endregion
}
