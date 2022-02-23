using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NeedData))]
public class NeedDataDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get end property before iterating
        SerializedProperty id = property.FindPropertyRelative("id");

        // Go to the first boolean property
        property.Next(true);

        // Layout the property
        property.boolValue = EditorGUIAuto.ToggleLeft(ref position, label, property.boolValue);

        if (property.boolValue)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Go to the next property
            property.Next(false);

            // Get enumerable of all the children
            IEnumerable<SerializedProperty> children = EditorGUIAuto.ToEnd(property, id, false, false);

            // Iterate over all children
            foreach (SerializedProperty child in children)
            {
                // Only display the property if we are supposed to
                if (DisplayProperty(child, id))
                {
                    EditorGUIAuto.PropertyField(ref position, child, true);
                }
            }

            // Decrease indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Height starts at standard control
        float height = EditorExtensions.StandardControlHeight;

        // Get end property before iterating
        SerializedProperty id = property.FindPropertyRelative("id");

        // Go to first property
        property.Next(true);

        // If the need is needed then add the height for the children
        if (property.boolValue)
        {
            property.Next(false);

            // Get an iterator over all children
            IEnumerable<SerializedProperty> children = EditorGUIAuto.ToEnd(property, id, false, false);

            height += children
                .Where(child => DisplayProperty(child, id))
                .Sum(child => EditorGUI.GetPropertyHeight(child, true));
        }

        return height;
    }
    #endregion

    #region Private Methods
    private bool DisplayProperty(SerializedProperty property, SerializedProperty id)
    {
        // Get the category
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        ItemRegistry.Category categoryData = (ItemRegistry.Category)category.enumValueIndex;

        // Get the index
        SerializedProperty index = id.FindPropertyRelative(nameof(index));
        int indexData = index.intValue;

        // Construct the item id
        ItemID itemID = new ItemID(categoryData, indexData);
        // Get the item data
        ItemData itemData = itemID.Data;
        // Check if the item is water
        bool isWater = itemData.Name.AnyNameContains("Water");

        // Can only prefer a food or tile that is not water
        if (property.name == "preferenceRating")
        {
            return categoryData == ItemRegistry.Category.Food ||
                (categoryData == ItemRegistry.Category.Tile && !isWater);
        }
        // Species type only applies for species
        else if (property.name == "speciesNeedType")
        {
            return categoryData == ItemRegistry.Category.Species;
        }
        // Min max only applies for water
        else if (property.name == "minimum" || property.name == "maximum")
        {
            return isWater;
        }
        else return true;
    }
    #endregion
}
