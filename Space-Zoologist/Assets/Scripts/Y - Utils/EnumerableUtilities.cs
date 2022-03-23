using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumerableUtilities
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, index) => (item, index));
    }
}
