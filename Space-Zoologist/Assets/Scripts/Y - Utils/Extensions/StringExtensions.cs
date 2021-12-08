using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string Sign(int integer)
    {
        if (integer < 0) return "-";
        else if (integer > 0) return "+";
        else return string.Empty;
    }
}
