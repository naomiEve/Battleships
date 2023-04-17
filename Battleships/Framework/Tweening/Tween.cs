namespace Battleships.Framework.Tweening
{
    /// <summary>
    /// A generic tween.
    /// </summary>
    /// <typeparam name="TValue">The tweened value.</typeparam>
    internal partial class Tween<TValue> : ITween
    {
        /// <summary>
        /// The current value.
        /// </summary>
        public TValue? Value { get; set; }

        /// <summary>
        /// The beginning value.
        /// </summary>
        public TValue? BeginningValue { get; private set; }

        /// <summary>
        /// The ending value.
        /// </summary>
        public TValue? EndValue { get; private set; }

        /// <summary>
        /// The time this tween should take.
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// The used easing.
        /// </summary>
        public TimeEasing Easing { get; private set; } = TimeEasing.Linear;

        /// <summary>
        /// The function responsible for incrementing the tween.
        /// </summary>
        public Func<TValue, TValue, float, TValue>? Incrementer { get; private set; }

        /// <summary>
        /// User-definable. Ran whenever the tween updates.
        /// </summary>
        public Action<TValue>? OnUpdate { get; private set; }

        /// <summary>
        /// Called when this tween finishes.
        /// </summary>
        public Action<TValue>? OnFinished { get; private set; }

        /// <summary>
        /// The elapsed time.
        /// </summary>
        private float _elapsed;

        /// <inheritdoc/>
        public bool Finished { get; private set; } = false;

        /// <summary>
        /// Constructs an empty tween.
        /// </summary>
        public Tween()
        {

        }

        /// <summary>
        /// Constructs a new tween.
        /// </summary>
        /// <param name="beginningValue">The beginning value.</param>
        /// <param name="endValue">The ending value.</param>
        /// <param name="time">The time this tween will take.</param>
        /// <param name="easing">The given easing.</param>
        /// <param name="incrementer">The incrementer for the value.</param>
        /// <param name="onUpdate">The function to be invoked when the tween updates.</param>
        public Tween(
            TValue? beginningValue, 
            TValue? endValue, 
            float time, 
            TimeEasing easing, 
            Func<TValue, TValue, float, TValue>? incrementer, 
            Action<TValue>? onUpdate = null, 
            Action<TValue>? onFinished = null)
        {
            Value = beginningValue;
            BeginningValue = beginningValue;
            EndValue = endValue;
            Time = time;
            Easing = easing;
            Incrementer = incrementer;
            OnUpdate = onUpdate;
            OnFinished = onFinished;
        }

        /// <summary>
        /// Kills this tween.
        /// </summary>
        public void Kill()
        {
            Value = EndValue;
            Finished = true;
            OnFinished?.Invoke(Value);
        }

        /// <inheritdoc/>
        public void UpdateTween(float dt)
        {
            if (Finished)
                return;

            _elapsed += dt;
            var interpolationFactor = TimeInterpolator.Evaluate(Easing, _elapsed, Time);

            Value = Incrementer!(BeginningValue!, EndValue!, interpolationFactor);
            OnUpdate?.Invoke(Value);

            Finished = (interpolationFactor >= 1);
            if (Finished)
                OnFinished?.Invoke(Value);
        }
    }
}
