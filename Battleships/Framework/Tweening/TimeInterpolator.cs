namespace Battleships.Framework.Tweening
{
    /// <summary>
    /// Helper class for interpolating time for tweens.
    /// </summary>
    internal static class TimeInterpolator
    {
        /// <summary>
        /// Returns a value between [0-1] given the elapsed time and the duration.
        /// </summary>
        /// <param name="easing">The easing to use.</param>
        /// <param name="elapsed">The already elapsed time.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>A value between [0-1] used for linear interpolation.</returns>
        public static float Evaluate(TimeEasing easing, float elapsed, float duration)
        {
            return easing switch
            {
                TimeEasing.Linear => elapsed / duration,
                _ => 1
            };
        }
    }
}
