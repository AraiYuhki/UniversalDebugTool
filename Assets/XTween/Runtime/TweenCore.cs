using System;
using UnityEngine;

namespace Xeon.XTween
{
    public abstract class TweenCore<T> : Tweener, ITween
    {
        protected T start, end;
        protected Action<T> setter;
        protected Func<T> getter;

        public TweenCore<T> SetAccsssor(Func<T> getter, Action<T> setter)
        {
            this.setter = setter;
            this.getter = getter;
            return this;
        }

        public virtual TweenCore<T> Setup(T end, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.end = end;
            this.duration = duration;
            this.FullDuration = delay + base.duration;
            base.easeType = easeType;
            IsLoop = isLoop;
            return this;
        }

        public TweenCore<T> SetEnd(T value)
        {
            end = value;
            return this;
        }

        public override void Play()
        {
            if (!IsPaused)
                start = getter();
            base.Play();
        }

        public override void Update()
        {
            if (!IsPlaying || IsCompleted || IsKilled) return;
            if (!useUnscaledTime && Time.timeScale <= 0f) return;

            elapsed += useUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            elapsed = Mathf.Min(elapsed, FullDuration);
            time = Mathf.Clamp(elapsed - delay, 0f, 1f) / duration;
            setter?.Invoke(GetValue(TweenFunctions.Evaluate(easeType, time)));

            onUpdate?.Invoke();
            if (Mathf.Abs(elapsed - FullDuration) >= float.Epsilon) return;

            elapsed = 0f;
            if (IsLoop)
            {
                onLoop?.Invoke();
                return;
            }

            onComplete?.Invoke();
            IsPlaying = false;
            IsCompleted = true;
        }

        protected abstract T GetValue(float value);
        public abstract T Evaluate(float elapsed);
    }

    public class IntTween : TweenCore<int>
    {
        public override int Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override int GetValue(float value) => (int)Mathf.Lerp(start, end, value);
    }

    public class FloatTween : TweenCore<float>
    {
        public override float Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override float GetValue(float value) => Mathf.Lerp(start, end, value);
    }

    public class LongTween : TweenCore<long>
    {
        public override long Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override long GetValue(float value) => (long)Mathf.Lerp(start, end, value);
    }

    public class Vector2Tween : TweenCore<Vector2>
    {
        public override Vector2 Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override Vector2 GetValue(float value) => Vector2.Lerp(start, end, value);
    }

    public class Vector3Tween : TweenCore<Vector3>
    {
        public override Vector3 Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override Vector3 GetValue(float value) => Vector3.Lerp(start, end, value);
    }

    public class Vector4Tween : TweenCore<Vector4>
    {
        public override Vector4 Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override Vector4 GetValue(float value) => Vector4.Lerp(start, end, value);
    }

    public class ColorTween : TweenCore<Color>
    {
        public override Color Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override Color GetValue(float value) => Color.Lerp(start, end, value);
    }

    public class QuaternionTween : TweenCore<Quaternion>
    {
        public override Quaternion Evaluate(float elapsed) => GetValue(CalcValue(elapsed));
        protected override Quaternion GetValue(float value) => Quaternion.Slerp(start, end, value);
    }
}
