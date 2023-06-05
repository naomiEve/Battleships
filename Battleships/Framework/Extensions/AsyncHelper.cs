namespace Battleships.Framework.Extensions;

/// <summary>
/// Helper for various async things.
/// </summary>
internal static class AsyncHelper
{
    /// <summary>
    /// Continuously yields until func returns false.
    /// </summary>
    /// <param name="func">The function.</param>
    public static async Task While(Func<bool> func)
    {
        var task = Task.Run(async () =>
        {
            while (func())
                await Task.Delay(15);
        });

        await Task.WhenAll(task);
    }
}
