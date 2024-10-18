using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Xeon.XTween
{
    public interface ITween
    {
        void Play();
        void Reset();
        void Update();
        void Kill();
        void SetPlayOnAwake(bool flag);
        bool PlayOnAwake { get; }
        bool IsCompleted { get; }
        bool IsSequenced { get; }
        bool IsKilled { get; }
    }

    public class XTween
    {
        private static XTween instance;
        public static XTween Instance
        {
            get
            {
                if (instance == null)
                    instance = new XTween();
                return instance;
            }
        }

        public bool PlayOnAwake { get; set; } = true;
        private int defaultPool = 30;

        private Dictionary<Type, LinkedPool<ITween>> pools = new();
        private List<ITween> activeTweens = new();

        private XTween()
        {
            pools[typeof(IntTween)] = new LinkedPool<ITween>(() => OnCreate<IntTween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(FloatTween)] = new LinkedPool<ITween>(() => OnCreate<FloatTween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(LongTween)] = new LinkedPool<ITween>(() => OnCreate<LongTween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(Vector2Tween)] = new LinkedPool<ITween>(() => OnCreate<Vector2Tween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(Vector3Tween)] = new LinkedPool<ITween>(() => OnCreate<Vector3Tween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(Vector4Tween)] = new LinkedPool<ITween>(() => OnCreate<Vector4Tween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(ColorTween)] = new LinkedPool<ITween>(() => OnCreate<ColorTween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(QuaternionTween)] = new LinkedPool<ITween>(() => OnCreate<QuaternionTween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(BezierVector3Tween)] = new LinkedPool<ITween>(() => OnCreate<BezierVector3Tween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(BezierVector2Tween)] = new LinkedPool<ITween>(() => OnCreate<BezierVector2Tween>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(CatmullRomTween3D)] = new LinkedPool<ITween>(() => OnCreate<CatmullRomTween3D>(), OnGet, OnRelease, OnDestroy, true, defaultPool);
            pools[typeof(Sequence)] = new LinkedPool<ITween>(() => OnCreate<Sequence>(), OnGet, OnRelease, OnDestroy, true, defaultPool);

            Application.onBeforeRender -= Update;
            Application.onBeforeRender += Update;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChanged;
#else
            Application.wantsToQuit -= WantsToQuit;
            Application.wantsToQuit += WantsToQuit;
#endif
            Application.quitting -= KillAll;
            Application.quitting += KillAll;
        }

        private bool WantsToQuit()
        {
            KillAll();
            return false;
        }

#if UNITY_EDITOR
        private void PlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode) return;
            KillAll();
        }
#endif

        private T OnCreate<T>() where T : Tweener, new() => new T();
        private void OnGet<T>(T target) where T : ITween
        {
            activeTweens.Add(target);
            target.Reset();
            target.SetPlayOnAwake(PlayOnAwake);
        }

        private void OnRelease<T>(T target) where T : ITween => activeTweens.Remove(target);
        private void OnDestroy<T>(T target) { }

        private void Update()
        {
            foreach (var tween in activeTweens.ToList())
            {
                if (tween.PlayOnAwake)
                    tween.Play();

                if (tween.IsSequenced)
                {
                    tween.Update();
                    continue;
                }

                if (tween.IsKilled)
                {
                    Kill(tween);
                    continue;
                }

                tween.Update();

                if (!tween.IsCompleted) continue;
                Kill(tween);
            }
        }

        public void Kill(ITween tween)
        {
            pools[tween.GetType()].Release(tween);
            activeTweens.Remove(tween);
            if (tween is not Sequence sequence) return;
            Kill(sequence.GetTweens());
        }

        public void Kill(List<ITween> tweens)
        {
            foreach (var tween in tweens)
                Kill(tween);
        }

        public void KillAll()
        {
            foreach (var tween in activeTweens)
                tween.Kill();
            foreach (var (type, pool) in pools)
                pool.Clear();
            activeTweens.Clear();
        }

        public static T Get<T>() where T : Tweener
            => Instance.pools[typeof(T)].Get() as T;

        public static Sequence Sequence()
            => Instance.pools[typeof(Sequence)].Get() as Sequence;

        private static TTweener To<TTweener, TValue>(Func<TValue> getter, Action<TValue> setter, TValue end, float duration, EaseType ease = EaseType.InOutQuad)
            where TTweener : TweenCore<TValue>
            => Get<TTweener>().SetAccsssor(getter, setter).Setup(end, duration, ease) as TTweener;

        public static IntTween To(Func<int> getter, Action<int> setter, int end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<IntTween, int>(getter, setter, end, duration, ease);
        public static FloatTween To(Func<float> getter, Action<float> setter, float end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<FloatTween, float>(getter, setter, end, duration, ease);
        public static LongTween To(Func<long> getter, Action<long> setter, long end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<LongTween, long>(getter, setter, end, duration, ease);
        public static Vector2Tween To(Func<Vector2> getter, Action<Vector2> setter, Vector2 end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<Vector2Tween, Vector2>(getter, setter, end, duration, ease);
        public static Vector3Tween To(Func<Vector3> getter, Action<Vector3> setter, Vector3 end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<Vector3Tween, Vector3>(getter, setter, end, duration, ease);
        public static Vector4Tween To(Func<Vector4> getter, Action<Vector4> setter, Vector4 end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<Vector4Tween, Vector4>(getter, setter, end, duration, ease);
        public static QuaternionTween To(Func<Quaternion> getter, Action<Quaternion> setter, Quaternion end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<QuaternionTween, Quaternion>(getter, setter, end, duration, ease);
        public static ColorTween To(Func<Color> getter, Action<Color> setter, Color end, float duration, EaseType ease = EaseType.InOutQuad)
            => To<ColorTween, Color>(getter, setter, end, duration, ease);

        public static BezierVector2Tween Bezier(Func<Vector2> getter, Action<Vector2> setter, List<BezierNode2D> nodes, float duration, EaseType ease = EaseType.InOutQuad)
        {
            var tweener = Get<BezierVector2Tween>();
            tweener.SetAccsssor(getter, setter);
            tweener.Setup(nodes, duration, ease, false);
            return tweener;
        }

        public static BezierVector3Tween Bezier(Func<Vector3> getter, Action<Vector3> setter, List<BezierNode3D> nodes, float duration, EaseType ease = EaseType.InOutQuad)
        {
            var tween = Get<BezierVector3Tween>();
            tween.SetAccsssor(getter, setter);
            tween.Setup(nodes, duration, ease, false);
            return tween;
        }

        public static CatmullRomTween3D CatmullRom(Func<Vector3> getter, Action<Vector3> setter, List<Vector3> points, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = Get<CatmullRomTween3D>();
            tween.SetAccsssor(getter, setter);
            tween.Setup(points, duration, type, false);
            return tween;
        }
    }
}
