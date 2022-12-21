using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (menuName = "NPC Data")]
public class NPCData : ScriptableObject
{
    public string CharacterName;
    public List<NPCExpression> Expressions = new List<NPCExpression>();

    [MenuItem("Tools/Dialogue/Create NPC From Icons")]
    public static void GenerateNPCFromSelection()
    {
        if (Selection.activeObject.GetType () != typeof (Sprite))
        {
            Debug.LogError ($"Failed to generate NPC: All selected items must be of type Sprite, found type {Selection.activeObject.GetType ()}.\n" +
                            $"Yes, that means you must select the sprite contained within a Texture2D. Sorry :/");
            return;
        }

        string outputPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath (Selection.activeObject)) + "/";
        //Debug.Log (outputPath);

        NPCData outputData = new NPCData ();
        int charNameEndIndex = Selection.activeObject.name.IndexOf ('_');

        outputData.CharacterName = char.ToUpper(Selection.activeObject.name[0]) + Selection.activeObject.name.Substring(1, charNameEndIndex - 1);
        //Debug.Log (outputPath + outputData.CharacterName);

        foreach (Object obj in Selection.objects)
        {
            if (obj.GetType() != typeof(Sprite))
            {
                Debug.LogError ($"Failed to generate NPC: All selected items must be of type Sprite, found type {obj.GetType ()}.\n" +
                                $"Yes, that means you must select the sprite contained within a Texture2D. Sorry :/");
                return;
            }
            if (outputData.CharacterName != char.ToUpper (obj.name [0]) + obj.name.Substring (1, charNameEndIndex - 1))
            {
                Debug.LogError ("Failed to generate NPC: All selected items must begin with the same name prefix and match format \"character_expression\"");
                return;
            }

            NPCExpression exp = new NPCExpression ();
            exp.DisplayName = char.ToUpper (obj.name [charNameEndIndex + 1]) + obj.name.Substring(charNameEndIndex + 2);
            exp.Icon = (Sprite) obj;

            outputData.Expressions.Add (exp);

            //Debug.Log (exp.DisplayName);
        }

        AssetDatabase.CreateAsset(outputData, outputPath + outputData.CharacterName + ".asset");
    }
}

[System.Serializable]
public class NPCExpression
{
    public string DisplayName;
    public Sprite Icon;
}