using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtensions
{
    #region Public Extension Methods
    public static Texture2D SetAllPixels(this Texture2D tex, Color32[] c)
    {
        tex.SetPixels32(c);
        return tex;
    }
    public static Texture2D FillCircle(this Texture2D texture, int x, int y, int r, Color c)
    {
        // Cache r squared since it will be used often
        float rSquared = r * r;

        // Loop through all x-coordinates
        for (int u = x - r; u < x + r + 1; u++)
            // Loop through all y coordinates in this x coordinate
            for (int v = y - r; v < y + r + 1; v++)
                // If the current x-y is in range, then set the pixel
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    texture.SetPixel(u, v, c);

        return texture;
    }
    public static Texture2D StrokeLine(this Texture2D texture, int x1, int y1, int x2, int y2, Color c)
    {
        // Different in x values
        int xDiff = Mathf.Abs(x2 - x1);
        // Direction that x1 shifts as it moves towards x2
        int xShift = x1 < x2 ? 1 : -1;

        // Difference in y values. It must be negative
        int yDiff = -Mathf.Abs(y2 - y1);
        // Direction that y1 shifts as it moves towards y2
        int yShift = y1 < y2 ? 1 : -1;

        // Error value. Negative if yDiff is bigger and positive if xDiff is bigger
        int error = xDiff + yDiff;
        // Error * 2, cached for efficiency
        int error2;

        while(x1 != x2 || y1 != y2)
        {
            // Set pixel of the texture
            texture.SetPixel(x1, y1, c);

            // Compute double the error
            error2 = error << 1;

            // Do a thing
            if(error2 >= yDiff)
            {
                error += yDiff;
                x1 += xShift;
            }
            // Do a different thing
            if(error2 <= xDiff)
            {
                error += xDiff;
                y1 += yShift;
            }
        }

        return texture;
    }
    public static Texture2D StrokeThickLine(this Texture2D texture, int x1, int y1, int x2, int y2, int t, Color c)
    {
        // Different in x values
        int xDiff = Mathf.Abs(x2 - x1);
        // Difference in y values. It must be negative
        int yDiff = Mathf.Abs(y2 - y1);

        // If x difference is bigger, thick line is drawn with vertical cap
        if(xDiff > yDiff)
        {
            for(int dy = -(t / 2); dy <= (t / 2); dy++)
            {
                texture.StrokeLine(x1, y1 + dy, x2, y2 + dy, c);
            }
        }
        // If y difference is bigger, thick line is drawn with horizontal cap
        else
        {
            for (int dx = -(t / 2); dx <= (t / 2); dx++)
            {
                texture.StrokeLine(x1 + dx, y1, x2 + dx, y2, c);
            }
        }

        return texture;
    }
    #endregion

    #region Private Methods
    //private static void StrokeLineOverlap(Texture2D texture) 
    #endregion
}
