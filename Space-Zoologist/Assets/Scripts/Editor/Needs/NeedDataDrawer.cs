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
    private static bool DisplayProperty(SerializedProperty parent, SerializedProperty child)
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
            return IsTerrainNeed(parent);
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
            if (IsTerrainNeed(parent))
            {
                SerializedProperty traversibleOnly = parent.FindPropertyRelative(nameof(traversibleOnly));
                return !traversibleOnly.boolValue;
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
            return IsWaterNeed(parent);
        }
        // Any child that does not match a name is displayed by default
        else return true;
    }
    private static bool IsTerrainNeed(SerializedProperty needData)
    {
        // Get the category
        SerializedProperty id = needData.FindPropertyRelative(nameof(id));
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        ItemRegistry.Category categoryData = (ItemRegistry.Category)category.enumValueIndex;

        // Get the index
        SerializedProperty index = id.FindPropertyRelative(nameof(index));
        int indexData = index.intValue;

        // Construct the item id
        ItemID itemID = new ItemID(categoryData, indexData);

        // Determine if the need data is used as terrain
        bool useAsTerrainNeed = needData.FindPropertyRelative(nameof(useAsTerrainNeed)).boolValue;

        // This is a terrain need if the category is a tile and...
        return itemID.Category == ItemRegistry.Category.Tile && 
            // ...either the item is not water or...
            (!itemID.IsWater || 
            // ...if it is the first water, it should be used as a terrain need
            (itemID.WaterIndex == 0 && useAsTerrainNeed));
    }
    private static bool IsWaterNeed(SerializedProperty needData)
    {
        // Get the category
        SerializedProperty id = needData.FindPropertyRelative(nameof(id));
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        ItemRegistry.Category categoryData = (ItemRegistry.Category)category.enumValueIndex;

        // Get the index
        SerializedProperty index = id.FindPropertyRelative(nameof(index));
        int indexData = index.intValue;

        // Construct the item id
        ItemID itemID = new ItemID(categoryData, indexData);

        // Determine if the need data is used as terrain
        bool useAsWaterNeed = needData.FindPropertyRelative(nameof(useAsWaterNeed)).boolValue;

        // This is a water need if the item is water and either
        // it is not the first water or if it is, then the first water
        // is marked for use as a water need
        return itemID.IsWater && (itemID.WaterIndex != 0 || useAsWaterNeed);
    }
    #endregion
}
