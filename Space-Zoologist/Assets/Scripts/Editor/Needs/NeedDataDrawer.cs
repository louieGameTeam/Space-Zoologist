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
        // Get ID and parent
        SerializedProperty parent = property.Copy();
        SerializedProperty id = property.FindPropertyRelative("id");

        // Go to the first boolean property
        property.Next(true);
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
                if (DisplayProperty(parent, child))
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
        SerializedProperty parent = property.Copy();
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
                .Where(child => DisplayProperty(parent, child))
                .Sum(child => EditorGUI.GetPropertyHeight(child, true));
        }

        return height;
    }
    #endregion

    #region Private Methods
    private bool DisplayProperty(SerializedProperty parent, SerializedProperty child)
    {
        // Get the category
        SerializedProperty id = parent.FindPropertyRelative(nameof(id));
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        ItemRegistry.Category categoryData = (ItemRegistry.Category)category.enumValueIndex;

        // Get the index
        SerializedProperty index = id.FindPropertyRelative(nameof(index));
        int indexData = index.intValue;

        // Construct the item id
        ItemID itemID = new ItemID(categoryData, indexData);
        // Check if the item is water
        bool isWater = itemID.IsWater;

        // Traversible only applies to terrain
        if (child.name == "traversibleOnly")
        {
            return categoryData == ItemRegistry.Category.Tile && !isWater;
        }
        // Only the first water ID can be used as terrain need
        if (child.name == "useAsTerrainNeed")
        {
            return itemID.IsWater && itemID.WaterIndex == 0;
        }
        // Can only prefer a food or tile that is not water
        if (child.name == "preferred")
        {
            // Only display tile preference rating if it is not traversible only
            if (categoryData == ItemRegistry.Category.Tile)
            {
                // Display preference if we are using this water as terrain
                if (isWater && itemID.WaterIndex == 0)
                {
                    SerializedProperty useAsTerrainNeed = parent.FindPropertyRelative(nameof(useAsTerrainNeed));
                    return useAsTerrainNeed.boolValue;
                }
                else if (!isWater)
                {
                    SerializedProperty traversibleOnly = parent.FindPropertyRelative(nameof(traversibleOnly));
                    return !traversibleOnly.boolValue;
                }
                // Water other than index 0 should not display preference
                else return false;
            }
            // Always display food preference
            return categoryData == ItemRegistry.Category.Food;
        }
        // Species type only applies for species
        else if (child.name == "speciesNeedType")
        {
            return categoryData == ItemRegistry.Category.Species;
        }
        // Display use as water need for the first water
        else if (child.name == "useAsWaterNeed")
        {
            return isWater && itemID.WaterIndex == 0;
        }
        // Min max only applies for water
        else if (child.name == "minimum" || child.name == "maximum")
        {
            // If this is the first water, check if we are using it as a water need
            if (isWater && itemID.WaterIndex == 0)
            {
                SerializedProperty useAsWaterNeed = parent.FindPropertyRelative(nameof(useAsWaterNeed));
                return useAsWaterNeed.boolValue;
            }
            // Else only display if this is a water
            else return isWater;
        }
        else return true;
    }
    #endregion
}
