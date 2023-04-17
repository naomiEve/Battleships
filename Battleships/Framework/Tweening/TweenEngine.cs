using Battleships.Framework.Objects;

namespace Battleships.Framework.Tweening
{
    /// <summary>
    /// The object responsible for tweening.
    /// </summary>
    internal class TweenEngine : GameObject,
        IIndestructibleObject,
        ISingletonObject
    {
        /// <summary>
        /// The tweens.
        /// </summary>
        private readonly List<ITween> _tweens;

        /// <summary>
        /// The tweens to be disposed.
        /// </summary>
        private readonly List<ITween> _disposeList;

        /// <summary>
        /// Construct a new tween engine.
        /// </summary>
        public TweenEngine()
        {
            _tweens = new List<ITween>();
            _disposeList = new List<ITween>();
        }

        /// <summary>
        /// Adds a tween to the list of tweens.
        /// </summary>
        /// <param name="tween">The tween.</param>
        public void AddTween(ITween tween)
        {
            _tweens.Add(tween);
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            _disposeList.Clear();
            foreach (var tween in _tweens)
            {
                tween.UpdateTween(dt);

                if (tween.Finished)
                    _disposeList.Add(tween);
            }

            foreach (var tween in _disposeList)
                _tweens.Remove(tween);
        }
    }
}
