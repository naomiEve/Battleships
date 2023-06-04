namespace Battleships.Framework.Assets;

/// <summary>
/// Implemented by all assets which can return a ref to the underlying Raylib object.
/// </summary>
internal interface IAssetCanBeRef<T>
{
    /// <summary>
    /// Returns a reference to the underlying Raylib object.
    /// </summary>
    /// <returns>The reference.</returns>
    ref T AsRef();
}
