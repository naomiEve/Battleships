using System.Numerics;

namespace Battleships.Framework.Math
{
    /// <summary>
    /// Extensions for the vector classes.
    /// </summary>
    internal static class VectorExtensions
    {
        /// <summary>
        /// Linearly interpolates between the first vector and the second vector, given a time t.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="t">The time.</param>
        /// <returns>The linearly interpolated vector at time point t.</returns>
        public static Vector3 LinearInterpolation(this Vector3 start, Vector3 end, float t)
        {
            return new Vector3(
                Mathematics.LinearInterpolation(start.X, end.X, t),
                Mathematics.LinearInterpolation(start.Y, end.Y, t),
                Mathematics.LinearInterpolation(start.Z, end.Z, t)
            );
        }
    }
}
