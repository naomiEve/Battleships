namespace Battleships.Framework.Tweening
{
    /// <summary>
    /// Helper class for interpolating time for tweens.
    /// </summary>
    internal static class TimeInterpolator
    {
        const float C4 = (2 * MathF.PI) / 3;

        /// <summary>
        /// Returns a value between [0-1] given the elapsed time and the duration.
        /// </summary>
        /// <param name="easing">The easing to use.</param>
        /// <param name="elapsed">The already elapsed time.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>A value between [0-1] used for linear interpolation.</returns>
        public static float Evaluate(TimeEasing easing, float elapsed, float duration)
        {
            var t = elapsed / duration;

            return easing switch
            {
                TimeEasing.Linear => t,
                TimeEasing.OutCubic => 1 - MathF.Pow(duration - elapsed, 3),
                TimeEasing.OutElastic => t == 0 ? 0 : (t == 1 ? 1 : MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * C4) + 1),
                _ => 1
            };
        }
    }
}
