namespace Battleships.Framework.Math
{
    /// <summary>
    /// Mathematical helpers.
    /// </summary>
    internal static class Mathematics
    {
        /// <summary>
        /// Linearly interpolates between a and b, given the time t.
        /// </summary>
        /// <param name="a">The begining value.</param>
        /// <param name="b">The ending value.</param>
        /// <param name="t">The time.</param>
        /// <returns>The linearly interpolated value in time point t.</returns>
        public static float LinearInterpolation(float a, float b, float t)
        {
            return a * (1 - t) + b * t;
        }
    }
}
