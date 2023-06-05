namespace Battleships.Framework.Extensions;

/// <summary>
/// Extensions for arrays.
/// </summary>
internal static class ArrayExtensions
{
    /// <summary>
    /// Flattens a 2D array into an enumerator.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="map">The 2d array.</param>
    /// <returns>The enumerator.</returns>
    public static IEnumerable<T> Flatten<T>(this T[,] map)
    {
        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
                yield return map[x, y];
        }
    }

    /// <summary>
    /// Select a random element from the IEnumerable.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="enumerable">The enumerable</param>
    /// <returns>The random element.</returns>
    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        var index = Random.Shared.Next(0, enumerable.Count());
        return enumerable.ElementAt(index);
    }
}
