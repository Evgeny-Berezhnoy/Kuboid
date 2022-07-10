using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    #region Methods

    public static T Random<T>(this IEnumerable<T> collection)
    {
        var length = collection.Count();

        if (length == 0) return default(T);

        var random  = new Random(Randomizer.Seed);
        var index   = random.Next(0, length);

        return collection.ElementAt(index);
    }

    #endregion
}