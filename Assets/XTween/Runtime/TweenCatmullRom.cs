using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xeon.XTween
{
    internal class CatmullRomUpdater
    {
        private int pointCount = 0;
        private int currentPointIndex = 0;
        private float elapsedBetweenPoint = 0f;
        private float durationBetweenPoint = 0f;

        public int CurrentIndex => currentPointIndex;

        public void Reset()
        {
            currentPointIndex = 0;
            elapsedBetweenPoint = 0f;
            durationBetweenPoint = 0f;
        }

        public void Setup(int pointCount, float duration)
        {
            this.pointCount = pointCount;
            durationBetweenPoint = duration / pointCount;
        }

        public float UpdateElapsed()
        {
            elapsedBetweenPoint += Time.fixedDeltaTime;
            if (currentPointIndex >= pointCount)
                elapsedBetweenPoint = Mathf.Min(1f, elapsedBetweenPoint);

            if (elapsedBetweenPoint > durationBetweenPoint)
            {
                currentPointIndex++;
                if (currentPointIndex >= pointCount - 1)
                {
                    elapsedBetweenPoint = durationBetweenPoint;
                    currentPointIndex = pointCount - 2;
                }
                else
                    elapsedBetweenPoint = elapsedBetweenPoint - durationBetweenPoint;
            }
            return elapsedBetweenPoint / durationBetweenPoint;
        }
    }

    public class CatmullRomTween2D : Vector2Tween
    {
        private List<Vector2> points = new();
        private CatmullRomUpdater updater = new CatmullRomUpdater();

        public override TweenCore<Vector2> Setup(Vector2 end, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            throw new Exception("Not supported this setup function");
        }

        public CatmullRomTween2D Setup(float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.duration = duration;
            this.easeType = easeType;
            this.IsLoop = isLoop;
            FullDuration = duration + delay;
            elapsed = 0f;
            updater.Reset();

            return this;
        }

        public CatmullRomTween2D Setup(List<Vector2> points, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            if (points.Count <= 2)
                throw new Exception("制御点は2つ以上設定してください");
            this.points = points;
            points.Insert(0, Vector2.zero);
            return Setup(duration, easeType, isLoop);
        }

        public override void Play()
        {
            base.Play();
            updater.Setup(points.Count, duration);
            points[0] = start;
        }

        public override void Update()
        {
            if (!IsPlaying || IsCompleted || IsKilled) return;
            elapsed = Mathf.Min(elapsed + Time.fixedDeltaTime, FullDuration);
            time = Mathf.Clamp(elapsed - delay, 0f, 1f) / duration;
            setter?.Invoke(GetValue(TweenFunctions.Evaluate(easeType, time)));

            onUpdate?.Invoke();
            if (Mathf.Abs(elapsed - FullDuration) >= float.Epsilon) return;

            elapsed = 0f;
            updater.Reset();
            if (IsLoop)
            {
                onLoop?.Invoke();
                return;
            }
            onComplete?.Invoke();
            IsPlaying = false;
            IsCompleted = true;
        }

        protected override Vector2 GetValue(float value)
        {
            if (value <= 0f) return start;
            var time = updater.UpdateElapsed();
            var pointIndex = updater.CurrentIndex;
            if (pointIndex == 0)
                return GetValueFirst(time);
            if (pointIndex == points.Count - 2)
                return GetValueLast(time, pointIndex);

            var point0 = points[pointIndex - 1];
            var point1 = points[pointIndex];
            var point2 = points[pointIndex + 1];
            var point3 = points[pointIndex + 2];

            var a = -point0 + 3f * point1 - 3f * point2 + point3;
            var b = 2f * point0 - 5f * point1 + 4f * point2 - point3;
            var c = point2 - point0;
            var d = 2f * point1;

            return 0.5f * (((a * Mathf.Pow(time, 3)) + (b * Mathf.Pow(time, 2)) + c * time) + d);
        }

        private Vector2 GetValueFirst(float time)
        {
            var b = points[0] - 2f * points[1] + points[2];
            var c = -3 * points[0] + 4f * points[1] - points[2];
            var d = 2f * points[0];

            return 0.5f * ((b * Mathf.Pow(time, 2)) + (c * time) + d);
        }

        private Vector2 GetValueLast(float time, int pointIndex)
        {
            var point0 = points[pointIndex - 1];
            var point1 = points[pointIndex];
            var point2 = points[pointIndex + 1];

            var b = point0 - 2f * point1 + point2;
            var c = point2 - point0;
            var d = 2f * point1;
            return 0.5f * ((b * Mathf.Pow(time, 2)) + (c * time) + d);
        }
    }

    public class CatmullRomTween3D : Vector3Tween
    {
        private List<Vector3> points = new();
        private CatmullRomUpdater updater = new CatmullRomUpdater();

        public override TweenCore<Vector3> Setup(Vector3 end, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            throw new Exception("Not supported this setup function");
        }

        public CatmullRomTween3D Setup(float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.duration = duration;
            this.easeType = easeType;
            this.IsLoop = isLoop;
            FullDuration = duration + delay;
            elapsed = 0f;
            updater.Reset();

            return this;
        }

        public CatmullRomTween3D Setup(List<Vector3> points, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            if (points.Count <= 2)
                throw new Exception("制御点は2つ以上設定してください");
            this.points = points;
            points.Insert(0, Vector3.zero);
            return Setup(duration, easeType, isLoop);
        }

        public override void Play()
        {
            base.Play();
            updater.Setup(points.Count, duration);
            points[0] = start;
        }

        public override void Update()
        {
            if (!IsPlaying || IsCompleted || IsKilled) return;
            elapsed = Mathf.Min(elapsed + Time.fixedDeltaTime, FullDuration);
            time = Mathf.Clamp(elapsed - delay, 0f, 1f) / duration;
            setter?.Invoke(GetValue(TweenFunctions.Evaluate(easeType, time)));

            onUpdate?.Invoke();
            if (Mathf.Abs(elapsed - FullDuration) >= float.Epsilon) return;

            elapsed = 0f;
            updater.Reset();
            if (IsLoop)
            {
                onLoop?.Invoke();
                return;
            }
            onComplete?.Invoke();
            IsPlaying = false;
            IsCompleted = true;
        }

        protected override Vector3 GetValue(float value)
        {
            if (value <= 0f) return start;
            var time = updater.UpdateElapsed();
            var pointIndex = updater.CurrentIndex;
            if (pointIndex == 0)
                return GetValueFirst(time);
            if (pointIndex == points.Count - 2)
                return GetValueLast(time, pointIndex);

            var point0 = points[pointIndex - 1];
            var point1 = points[pointIndex];
            var point2 = points[pointIndex + 1];
            var point3 = points[pointIndex + 2];

            var a = -point0 + 3f * point1 - 3f * point2 + point3;
            var b = 2f * point0 - 5f * point1 + 4f * point2 - point3;
            var c = point2 - point0;
            var d = 2f * point1;

            return 0.5f * (((a * Mathf.Pow(time, 3)) + (b * Mathf.Pow(time, 2)) + c * time) + d);
        }

        private Vector3 GetValueFirst(float time)
        {
            var b = points[0] - 2f * points[1] + points[2];
            var c = -3 * points[0] + 4f * points[1] - points[2];
            var d = 2f * points[0];

            return 0.5f * ((b * Mathf.Pow(time, 2)) + (c * time) + d);
        }

        private Vector3 GetValueLast(float time, int pointIndex)
        {
            var point0 = points[pointIndex - 1];
            var point1 = points[pointIndex];
            var point2 = points[pointIndex + 1];

            var b = point0 - 2f * point1 + point2;
            var c = point2 - point0;
            var d = 2f * point1;
            return 0.5f * ((b * Mathf.Pow(time, 2)) + (c * time) + d);
        }
    }
}
