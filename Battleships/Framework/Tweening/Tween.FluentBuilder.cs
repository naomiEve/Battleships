namespace Battleships.Framework.Tweening
{
    internal partial class Tween<TValue>
    {
        /// <summary>
        /// Sets the beginning value of the tween.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        public Tween<TValue> WithBeginningValue(TValue value)
        {
            BeginningValue = value;
            return this;
        }

        /// <summary>
        /// Sets the ending value of the tween.
        /// </summary>
        /// <param name="value">The ending value of the tween.</param>
        public Tween<TValue> WithEndingValue(TValue value)
        {
            EndValue = value;
            return this;
        }

        /// <summary>
        /// Sets the easing of the tween.
        /// </summary>
        /// <param name="easing">The easing of the tween.</param>
        public Tween<TValue> WithEasing(TimeEasing easing)
        {
            Easing = easing;
            return this;
        }

        /// <summary>
        /// Sets the time taken by the tween.
        /// </summary>
        /// <param name="time">The time taken by the tween.</param>
        public Tween<TValue> WithTime(float time)
        {
            Time = time;
            return this;
        }

        /// <summary>
        /// Sets the update callback of the tween.
        /// </summary>
        /// <param name="callback">The update callback of the tween.</param>
        public Tween<TValue> WithUpdateCallback(Action<TValue> callback)
        {
            OnUpdate = callback;
            return this;
        }

        /// <summary>
        /// Sets the finished callback of the tween.
        /// </summary>
        /// <param name="callback">The finished callback of the tween.</param>
        public Tween<TValue> WithFinishedCallback(Action<TValue> callback)
        {
            OnFinished = callback;
            return this;
        }

        /// <summary>
        /// Sets the incrementer of the tween.
        /// </summary>
        /// <param name="callback">The incrementer.</param>
        public Tween<TValue> WithIncrementer(Func<TValue, TValue, float, TValue> callback)
        {
            Incrementer = callback;
            return this;
        }

        /// <summary>
        /// Binds this tween to an engine.
        /// </summary>
        /// <param name="engine">The engine to bind to.</param>
        public Tween<TValue> BindTo(TweenEngine engine)
        {
            engine.AddTween(this);
            return this;
        }
    }
}
