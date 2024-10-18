using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xeon.XTween
{
    public class BezierNode<T>
    {
        public T end;
        public T controlPoint1;
        public T controlPoint2;

        public BezierNode(T end, T controlPoint1, T controlPoint2)
        {
            this.end = end;
            this.controlPoint1 = controlPoint1;
            this.controlPoint2 = controlPoint2;
        }
    }

    public class BezierNode3D : BezierNode<Vector3>
    {
        public BezierNode3D(Vector3 end, Vector3 controlPoint1, Vector3 controlPoint2)
            :base(end, controlPoint1, controlPoint2)
        {
        }
    }

    public class BezierNode2D : BezierNode<Vector2>
    {
        public BezierNode2D(Vector2 end, Vector2 controlPoint1, Vector2 controlPoint2)
            : base(end, controlPoint1, controlPoint2)
        {
        }
    }

    internal class BezierUpdater
    {
        private int nodeCount = 0;
        private float elapsedBetweenNode = 0f;
        private int currentNodeIndex = 0;
        private float durationBetweenNode = 0f;

        public int CurrentIndex => currentNodeIndex;

        public void Reset()
        {
            elapsedBetweenNode = 0f;
            currentNodeIndex = 0;
            durationBetweenNode = 0f;
        }

        public void Setup(int nodeCount, float duration)
        {
            this.nodeCount = nodeCount;
            durationBetweenNode = duration / nodeCount;
        }

        public float UpdateElapsed()
        {
            elapsedBetweenNode += Time.fixedDeltaTime;
            if (currentNodeIndex >= nodeCount)
                elapsedBetweenNode = Mathf.Min(1f, elapsedBetweenNode);

            if (elapsedBetweenNode > durationBetweenNode)
            {
                currentNodeIndex = currentNodeIndex++;
                if (currentNodeIndex >= nodeCount)
                {
                    elapsedBetweenNode = durationBetweenNode;
                    currentNodeIndex = nodeCount - 1;
                }
                else
                    elapsedBetweenNode = elapsedBetweenNode - durationBetweenNode;
            }
            return elapsedBetweenNode / durationBetweenNode;
        }
    }

    public class BezierVector2Tween : Vector2Tween
    {
        private List<BezierNode2D> nodes = new();
        private BezierUpdater updater = new BezierUpdater();

        public override TweenCore<Vector2> Setup(Vector2 end, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            throw new Exception("Not supported this setup");
        }

        public Vector2Tween Setup(float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.duration = duration;
            this.easeType = easeType;
            this.IsLoop = isLoop;
            FullDuration = duration + delay;
            updater.Reset();
            elapsed = 0f;
            return this;
        }

        public Vector2Tween Setup(Vector2 end, Vector2 controlPoint1, Vector2 controlPoint2, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            nodes.Clear();
            nodes.Add(new BezierNode2D(end, controlPoint1, controlPoint2));
            Setup(duration, easeType, isLoop);
            return this;
        }

        public Vector2Tween Setup(List<BezierNode2D> nodes, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.nodes = nodes;
            Setup(duration, easeType, isLoop);
            return this;
        }

        public Vector2Tween SetNodes(params BezierNode2D[] nodes)
        {
            this.nodes = nodes.ToList();
            return this;
        }

        public override void Play()
        {
            base.Play();
            updater.Setup(nodes.Count, duration);
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
            if (value <= 0f)
                return start;
            var time = updater.UpdateElapsed();

            var startPosition = start;
            var nodeIndex = updater.CurrentIndex;
            // Nodeのインデックスが1以上の場合は一つ前のノードの座標を始点にする
            if (nodeIndex > 0)
                startPosition = nodes[nodeIndex - 1].end;
            var endNode = nodes[nodeIndex];
            var reverse = 1.0f - time;
            return
                startPosition * Mathf.Pow(reverse, 3) +
                endNode.controlPoint1 * 3.0f * time * Mathf.Pow(reverse, 2) +
                endNode.controlPoint2 * 3.0f * reverse * Mathf.Pow(time, 2) +
                endNode.end * Mathf.Pow(time, 3);
        }
    }

    public class BezierVector3Tween : Vector3Tween
    {
        private List<BezierNode3D> nodes = new();
        private BezierUpdater updater = new BezierUpdater();

        public override TweenCore<Vector3> Setup(Vector3 end, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            throw new Exception("Not supported this setup");
        }

        public Vector3Tween Setup(float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.duration = duration;
            this.easeType = easeType;
            this.IsLoop = isLoop;
            FullDuration = duration + delay;
            updater.Reset();
            elapsed = 0f;
            return this;
        }

        public Vector3Tween Setup(Vector3 end, Vector3 controlPoint1, Vector3 controlPoint2, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            nodes.Clear();
            nodes.Add(new BezierNode3D(end, controlPoint1, controlPoint2));
            Setup(duration, easeType, isLoop);
            return this;
        }

        public Vector3Tween Setup(List<BezierNode3D> nodes, float duration, EaseType easeType = EaseType.InOutQuad, bool isLoop = false)
        {
            this.nodes = nodes;
            Setup(duration, easeType, isLoop);
            return this;
        }

        public Vector3Tween SetNodes(params BezierNode3D[] nodes)
        {
            this.nodes = nodes.ToList();
            return this;
        }

        public override void Play()
        {
            base.Play();
            updater.Setup(nodes.Count, duration);
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
            if (value <= 0f)
                return start;
            var time = updater.UpdateElapsed();

            var startPosition = start;
            var nodeIndex = updater.CurrentIndex;
            // Nodeのインデックスが1以上の場合は一つ前のノードの座標を始点にする
            if (nodeIndex > 0)
                startPosition = nodes[nodeIndex - 1].end;
            var endNode = nodes[nodeIndex];
            var reverse = 1.0f - time;
            return
                startPosition * Mathf.Pow(reverse, 3) +
                endNode.controlPoint1 * 3.0f * time * Mathf.Pow(reverse, 2) +
                endNode.controlPoint2 * 3.0f * reverse * Mathf.Pow(time, 2) +
                endNode.end * Mathf.Pow(time, 3);
        }
    }
}
