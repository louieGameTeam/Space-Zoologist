
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public static class SpriteSheetTrimmer
{
    [MenuItem("SpriteTools/TrimAllSlices")]
    static void TrimAll()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D)
            {
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
                dataProvider.InitSpriteEditorDataProvider();
                Trim(dataProvider, (Texture2D)obj);
                dataProvider.Apply();
            }
        }
    }

    private static void Trim(ISpriteEditorDataProvider dataProvider, Texture2D tex)
    {
        var spriteRects = dataProvider.GetSpriteRects();
        foreach (var rect in spriteRects)
        {
            float left = float.MaxValue, right = 0, bottom = float.MaxValue, top = 0;
            int width = (int)rect.rect.width;
            int height = (int)rect.rect.height;
            int xPos = (int)rect.rect.x;
            int yPos = (int)rect.rect.y;
            for (int i = xPos; i < xPos + width; i++)
            {
                for (int j = yPos; j < yPos + height; j++)
                {
                    if (tex.GetPixel(i, j).a > 0)
                    {
                        if (i < left)
                            left = i;
                        else if (i > right)
                            right = i;
                        if (j < bottom)
                            bottom = j;
                        else if (j > top)
                            top = j;
                    }
                }
            }
            rect.rect = new Rect(left, bottom, right - left, top - bottom);
        }
        dataProvider.SetSpriteRects(spriteRects);
    }
}