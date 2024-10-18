using System;
using System.Diagnostics;

namespace Xeon.XTween
{
    public class Tweener
    {
        public float FullDuration { get; protected set; } = 0.2f;

        public bool IsSequenced { get; protected set; } = false;
        public float StartTime { get; protected set; } = 0f;
        public bool IsCompleted { get; protected set; } = false;
        public bool IsPlaying { get; protected set; } = false;
        public bool IsPaused { get; protected set; } = false;
        public bool IsKilled { get; protected set; } = false;
        public bool IsLoop { get; protected set; } = false;

        public bool PlayOnAwake => !IsPlaying && !IsPaused && !IsKilled && !IsCompleted && playOnAwake;

        protected float time = 0f;
        protected float delay = 0f;
        protected float elapsed = 0f;
        protected float duration = 0.2f;
        protected bool playOnAwake = false;
        protected bool useUnscaledTime = true;
        protected EaseType easeType = EaseType.InOutQuad;

        protected Action onUpdate;
        protected Action onComplete;
        protected Action onLoop;
        protected Action onKill;

        public Tweener SetEase(EaseType type)
        {
            easeType = type;
            return this;
        }

        public Tweener SetDuration(float duration)
        {
            this.duration = duration;
            FullDuration = delay + this.duration;
            return this;
        }

        public Tweener SetLoop(bool flag)
        {
            IsLoop = flag;
            return this;
        }

        public Tweener SetDelay(float delay)
        {
            this.delay = delay;
            FullDuration = this.delay + duration;
            return this;
        }

        public void SetStartTime(float time) => StartTime = time;
        public void SetIsSequenced() => IsSequenced = true;
        public virtual void SetPlayOnAwake(bool flag) => playOnAwake = flag;
        public void SetUseUnscaledTime(bool flag) => useUnscaledTime = flag;

        public Tweener OnComplete(Action onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }

        public Tweener OnLoop(Action onLoop)
        {
            this.onLoop = onLoop;
            return this;
        }

        public Tweener OnUpdate(Action onUpdate)
        {
            this.onUpdate = onUpdate;
            return this;
        }

        public Tweener OnKill(Action onKill)
        {
            this.onKill = onKill;
            return this;
        }

        public Action GetOnComplete() => onComplete;

        public virtual void Reset()
        {
            IsSequenced = IsPlaying = IsCompleted = IsPaused = IsKilled = false;
            StartTime = time = elapsed = delay = duration = 0f;
        }

        public virtual void Clear()
        {
            Reset();
            onComplete = onKill = onLoop = onUpdate = null;
        }

        public virtual void Play()
        {
            if (IsKilled)
                throw new Exception("This tween has killed");
            IsPlaying = true;
            IsCompleted = IsPaused = IsKilled = false;
        }

        public virtual void Rewind()
        {
            IsCompleted = false;
            time = elapsed = 0f;

            if (!IsSequenced) return;
            IsPlaying = false;
        }

        public virtual void Kill()
        {
            IsPlaying = IsPaused = IsCompleted = false;
            IsKilled = true;
            elapsed = 0f;
            time = 0f;
            onKill?.Invoke();
        }

        public virtual void Pause()
        {
            IsPlaying = false;
            IsPaused = true;
        }

        protected virtual float CalcValue(float elapsed)
        {
            this.elapsed = elapsed;
            time = elapsed / duration;
            return TweenFunctions.Evaluate(easeType, time);
        }
        public virtual void Update() { }
    }
}
