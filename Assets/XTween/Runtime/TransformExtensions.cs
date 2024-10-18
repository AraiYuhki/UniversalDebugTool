using UnityEngine;
using System.Collections.Generic;

namespace Xeon.XTween
{
    public static class TransformExtensions
    {
        private static void SetX(this Transform self, float x)
        {
            var position = self.position;
            position.x = x;
            self.position = position;
        }
        private static void SetY(this Transform self, float y)
        {
            var position = self.position;
            position.y = y;
            self.position = position;
        }
        private static void SetZ(this Transform self, float z)
        {
            var position = self.position;
            position.z = z;
            self.position = position;
        }
        private static void SetLocalX(this Transform self, float x)
        {
            var position = self.localPosition;
            position.x = x;
            self.localPosition = position;
        }
        private static void SetLocalY(this Transform self, float y)
        {
            var position = self.localPosition;
            position.y = y;
            self.localPosition = position;
        }
        private static void SetLocalZ(this Transform self, float z)
        {
            var position = self.localPosition;
            position.z = z;
            self.localPosition = position;
        }

        private static void SetLocalScaleX(this Transform self, float x)
        {
            var scale = self.localScale;
            scale.x = x;
            self.localScale = scale;
        }
        private static void SetLocalScaleY(this Transform self, float y)
        {
            var scale = self.localScale;
            scale.y = y;
            self.localScale = scale;
        }
        private static void SetLocalScaleZ(this Transform self, float z)
        {
            var scale = self.localScale;
            scale.z = z;
            self.localScale = scale;
        }

        private static void SetRotationX(this Transform self, float x)
        {
            var rotation = self.rotation.eulerAngles;
            rotation.x = x;
            self.rotation = Quaternion.Euler(rotation);
        }

        private static void SetRotationY(this Transform self, float y)
        {
            var rotation = self.rotation.eulerAngles;
            rotation.y = y;
            self.rotation = Quaternion.Euler(rotation);
        }

        private static void SetRotationZ(this Transform self, float z) 
        {
            var rotation = self.rotation.eulerAngles;
            rotation.z = z;
            self.rotation = Quaternion.Euler(rotation);
        }

        private static void SetLocalRotationX(this Transform self, float x)
        {
            var rotation = self.localRotation.eulerAngles;
            rotation.x = x;
            self.localRotation = Quaternion.Euler(rotation);
        }

        private static void SetLocalRotationY(this Transform self, float y)
        {
            var rotation = self.localRotation.eulerAngles;
            rotation.y = y;
            self.localRotation = Quaternion.Euler(rotation);
        }

        private static void SetLocalRotationZ(this Transform self, float z)
        {
            var rotation = self.localRotation.eulerAngles;
            rotation.z = z;
            self.localRotation = Quaternion.Euler(rotation);
        }

        private static T GetTween<T>() where T : Tweener => XTween.Get<T>();

        // 移動系
        public static Tweener TweenMoveX(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.position.x, value => self.SetX(value)).Setup(end, duration, type);
        public static Tweener TweenMoveY(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.position.y, value => self.SetY(value)).Setup(end, duration, type);
        public static Tweener TweenMoveZ(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.position.z, value => self.SetZ(value)).Setup(end, duration, type);
        public static Tweener TweenMove(this Transform self, Vector3 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector3Tween>().SetAccsssor(() => self.position, value => self.position = value).Setup(end, duration, type);
        
        public static Tweener TweenBezier(this Transform self, Vector3 end, Vector3 controlPoint1, Vector3 controlPoint2, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = GetTween<BezierVector3Tween>().SetAccsssor(() => self.position, value => self.position = value) as BezierVector3Tween;
            tween.Setup(end, controlPoint1, controlPoint2, duration, type);
            return tween;
        }
        public static Tweener TweenBezier(this Transform self, List<BezierNode3D> nodes, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = GetTween<BezierVector3Tween>().SetAccsssor(() => self.position, value => self.position = value) as BezierVector3Tween;
            tween.Setup(nodes, duration, type);
            return tween;
        }

        public static Tweener TweenCatmulRom(this Transform self, List<Vector3> points, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = GetTween<CatmullRomTween3D>().SetAccsssor(() => self.position, value => self.position = value) as CatmullRomTween3D;
            tween.Setup(points, duration, type);
            return tween;
        }

        // 移動系(ローカル座標)
        public static Tweener TweenLocalMoveX(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localPosition.x, value => self.SetLocalX(value)).Setup(end, duration, type);
        public static Tweener TweenLocalMoveY(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localPosition.y, value => self.SetLocalY(value)).Setup(end, duration, type);
        public static Tweener TweenLocalMoveZ(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localPosition.z, value => self.SetLocalZ(value)).Setup(end, duration, type);
        public static Tweener TweenLocalMove(this Transform self, Vector3 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector3Tween>().SetAccsssor(() => self.localPosition, value => self.localPosition = value).Setup(end, duration, type);

        public static Tweener TweenLocalBezier(this Transform self, Vector3 end, Vector3 controlPoint1, Vector3 controlPoint2, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = GetTween<BezierVector3Tween>().SetAccsssor(() => self.localPosition, value => self.localPosition = value) as BezierVector3Tween;
            tween.Setup(end, controlPoint1, controlPoint2, duration, type);
            return tween;
        }

        public static Tweener TweenLocalBezier(this Transform self, List<BezierNode3D> nodes, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = GetTween<BezierVector3Tween>().SetAccsssor(() => self.localPosition, value => self.localPosition = value) as BezierVector3Tween;
            tween.Setup(nodes, duration, type);
            return tween;
        }

        public static Tweener TweenLocalCatmulRom(this Transform self, List<Vector3> points, float duration, EaseType type = EaseType.InOutQuad)
        {
            var tween = GetTween<CatmullRomTween3D>().SetAccsssor(() => self.localPosition, value => self.localPosition = value) as CatmullRomTween3D;
            tween.Setup(points, duration, type);
            return tween;
        }

        // 回転系
        public static Tweener TweenRotateX(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.rotation.x, value => self.SetRotationX(value)).Setup(end, duration, type);
        public static Tweener TweenRotateY(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.rotation.y, value => self.SetRotationY(value)).Setup(end, duration, type);
        public static Tweener TweenRotateZ(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.rotation.z, value => self.SetRotationZ(value)).Setup(end, duration, type);
        public static Tweener TweenRotate(this Transform self, Quaternion end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<QuaternionTween>().SetAccsssor(() => self.rotation, value => self.rotation = value).Setup(end, duration, type);
        public static Tweener TweenRotate(this Transform self, Vector3 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector3Tween>().SetAccsssor(() => self.rotation.eulerAngles, value => self.rotation = Quaternion.Euler(value)).Setup(end, duration, type);

        // 回転系(ローカル)
        public static Tweener TweenLocalRotateX(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localRotation.x, value => self.SetLocalRotationX(value)).Setup(end, duration, type);
        public static Tweener TweenLocalRotateY(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localRotation.y, value => self.SetLocalRotationY(value)).Setup(end, duration, type);
        public static Tweener TweenLocalRotateZ(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localRotation.z, value => self.SetLocalRotationZ(value)).Setup(end, duration, type);
        public static Tweener TweenLocalRotate(this Transform self, Quaternion end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<QuaternionTween>().SetAccsssor(() => self.localRotation, value => self.localRotation = value).Setup(end, duration, type);
        public static Tweener TweenLocalRotate(this Transform self, Vector3 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector3Tween>().SetAccsssor(() => self.localRotation.eulerAngles, value => self.localRotation = Quaternion.Euler(value)).Setup(end, duration, type);

        // スケール
        public static Tweener TweenLocalScaleX(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localScale.x, value => self.SetLocalScaleX(value)).Setup(end, duration, type);
        public static Tweener TweenLocalScaleY(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(() => self.localScale.y, value => self.SetLocalScaleY(value)).Setup(end, duration, type);
        public static Tweener TweenLocalScaleZ(this Transform self, float end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<FloatTween>().SetAccsssor(()=>self.localScale.z, value => self.SetLocalScaleZ(value)).Setup(end, duration, type);
        public static Tweener TweenLocalScale(this Transform self, Vector3 end, float duration, EaseType type = EaseType.InOutQuad)
            => GetTween<Vector3Tween>().SetAccsssor(() => self.localScale, value => self.localScale = value).Setup(end, duration, type);
    }
}
