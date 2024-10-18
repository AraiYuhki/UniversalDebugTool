using UnityEngine;
using UnityEngine.UI;

namespace Xeon.XTween
{
    public static class TweenExtensions
    {
        private static void SetAlpha(this Graphic self, float alpha)
        {
            var color = self.color;
            color.a = alpha;
            self.color = color;
        }

        private static void SetAlpha(this SpriteRenderer self, float alpha)
        {
            var color = self.color;
            color.a = alpha;
            self.color = color;
        }

        private static void SetPivotX(this RectTransform self, float x)
        {
            var pivot = self.pivot;
            pivot.x = x;
            self.pivot = pivot;
        }
        private static void SetPivotY(this RectTransform self, float y)
        {
            var pivot = self.pivot;
            pivot.y = y;
            self.pivot = pivot;
        }

        private static T GetTween<T>() where T : Tweener => XTween.Get<T>();

        // キャンバスグループ
        public static Tweener TweenFade(this CanvasGroup self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.alpha, value => self.alpha = value).Setup(end, duration, type);

        // Image系
        public static Tweener TweenFade(this Graphic self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.color.a, value => self.SetAlpha(value)).Setup(end, duration, type);
        public static Tweener TweenColor(this Graphic self, Color end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<ColorTween>().SetAccsssor(() => self.color, value => self.color = value).Setup(end, duration, type);

        public static Tweener TweenFade(this SpriteRenderer self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.color.a, value => self.SetAlpha(value)).Setup(end, duration, type);
        public static Tweener TweenColor(this SpriteRenderer self, Color end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<ColorTween>().SetAccsssor(() => self.color, value => self.color = value).Setup(end, duration, type);

        // RectTransform系
        public static Tweener TweenSizeDelta(this RectTransform self, Vector2 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector2Tween>().SetAccsssor(() => self.sizeDelta, value => self.sizeDelta = value).Setup(end, duration, type);
        public static Tweener TweenPivot(this RectTransform self, Vector2 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector2Tween>().SetAccsssor(() => self.pivot, value => self.pivot = value).Setup(end, duration, type);
        public static Tweener TweenPivotX(this RectTransform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.pivot.x, value => self.SetPivotX(value)).Setup(end, duration, type);
        public static Tweener TweenPivotY(this RectTransform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.pivot.y, value => self.SetPivotY(value)).Setup(end, duration, type);

        // サウンド系
        public static Tweener TweenVolume(this AudioSource self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.volume, value => self.volume = value).Setup(end, duration, type);
    }
}
