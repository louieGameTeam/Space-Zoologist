using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimalDominanceItem))]
public class AnimalDominanceItemDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty dominance = property.FindPropertyRelative(nameof(dominance));
        label = GetGUIContent(property);
        EditorGUI.PropertyField(position, dominance, label, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorExtensions.StandardControlHeight;
    }
    #endregion

    #region Public Methods
    public static ItemID GetID(SerializedProperty animalDominanceProperty)
    {
        // Get the sub properties of the id
        SerializedProperty id = animalDominanceProperty.FindPropertyRelative(nameof(id));
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        SerializedProperty index = id.FindPropertyRelative(nameof(index));

        // Get the item Id from the properties
        return new ItemID(
            (ItemRegistry.Category)category.enumValueIndex,
            index.intValue);
    }
    public static void SetID(SerializedProperty animalDominanceProperty, ItemID itemID)
    {
        // Get the sub properties of the dominance property
        SerializedProperty id = animalDominanceProperty.FindPropertyRelative(nameof(id));
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        SerializedProperty index = id.FindPropertyRelative(nameof(index));

        // Set the category and index of the property
        category.enumValueIndex = (int)itemID.Category;
        index.intValue = itemID.Index;
    }
    public static GUIContent GetGUIContent(SerializedProperty property)
    {
        // Get the item Id from the properties
        ItemID itemID = GetID(property);
        string name = itemID.Data.Name.Get(ItemName.Type.English);

        // Return the content with the english name
        return new GUIContent($"{name} Dominance");
    }
    #endregion
}
