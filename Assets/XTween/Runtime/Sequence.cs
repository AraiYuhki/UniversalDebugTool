using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xeon.XTween
{
    public struct SequenceCallback
    {
        public float executeTime;
        public Action action;
        public bool isExecuted { get; private set; }
        public SequenceCallback(float executeTime, Action action)
        {
            this.executeTime = executeTime;
            this.action = action;
            isExecuted = false;
        }

        public void Invoke()
        {
            action?.Invoke();
            isExecuted = true;
        }

        public void Reset()
        {
            isExecuted = false;
        }
    }

    public class Sequence : Tweener, ITween
    {
        protected List<Tweener> tweens = new();
        protected List<SequenceCallback> callbacks = new();
        protected float lastTweenInsertTime = 0f;
        protected float lastTweenFullDuration = 0f;

        public List<ITween> GetTweens() => tweens.Select(tween => (ITween)tween).ToList();

        public override void Reset()
        {
            base.Reset();
            lastTweenInsertTime = 0f;
            lastTweenFullDuration = 0f;
            foreach (var tween in tweens) tween.Reset();
            foreach (var callback in callbacks) callback.Reset();
        }

        public override void Clear()
        {
            base.Clear();
            lastTweenInsertTime = 0f;
            lastTweenFullDuration = 0f;
            tweens.Clear();
            callbacks.Clear();
        }

        public override void Update()
        {
            if (!IsPlaying) return;
            elapsed += UnityEngine.Time.deltaTime;
            foreach (var tween in tweens)
            {
                if (tween.IsPlaying || tween.IsCompleted) continue;
                if (elapsed >= tween.StartTime) tween.Play();
            }
            foreach (var callback in callbacks)
            {
                if (callback.isExecuted) continue;
                if (elapsed >= callback.executeTime) callback.Invoke();
            }
            onUpdate?.Invoke();
            if (tweens.Any(t => !t.IsCompleted)) return;
            if (callbacks.Any(c => !c.isExecuted)) return;
            onComplete?.Invoke();
            IsPlaying = false;
            IsCompleted = true;
        }

        public override void Pause()
        {
            base.Pause();
            foreach (var tween in tweens)
                tween.Pause();
        }

        public override void Rewind()
        {
            foreach (var tween in tweens)
                tween.Rewind();
            base.Rewind();
        }

        public override void Kill()
        {
            base.Kill();
            foreach (var tween in tweens)
                tween.Kill();
        }

        public Sequence Append(Tweener tween)
        {
            tween.SetPlayOnAwake(false);
            tween.SetStartTime(lastTweenInsertTime);
            tween.SetIsSequenced();
            lastTweenInsertTime += tween.FullDuration;
            lastTweenFullDuration = tween.FullDuration;
            tweens.Add(tween);
            return this;
        }

        public Sequence Join(Tweener tween)
        {
            tween.SetPlayOnAwake(false);
            var lastTween = tweens.LastOrDefault();
            tween.SetStartTime(lastTween == null ? 0f : lastTween.StartTime);
            tween.SetIsSequenced();
            // 一緒に再生するTweenの内、最も長いTweenになるか確認する
            if (lastTweenFullDuration < tween.FullDuration)
            {
                // 差分の分だけ最後の時間を進める
                lastTweenInsertTime += tween.FullDuration - lastTweenFullDuration;
                lastTweenFullDuration = tween.FullDuration;
            }
            tweens.Add(tween);
            return this;
        }

        public Sequence Insert(float atPosition, Tweener tween)
        {
            tween.SetPlayOnAwake(false);
            tween.SetStartTime(atPosition);
            tween.SetIsSequenced();
            lastTweenInsertTime = Mathf.Max(lastTweenInsertTime, atPosition + tween.FullDuration);
            tweens.Add(tween);
            return this;
        }

        public Sequence AppendCallback(Action callback)
        {
            callbacks.Add(new SequenceCallback(lastTweenInsertTime, callback));
            return this;
        }

        public Sequence InsertCallback(float atPosition, Action callback)
        {
            callbacks.Add(new SequenceCallback(atPosition, callback));
            return this;
        }
    }
}
