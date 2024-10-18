using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xeon.XTween
{
    public enum EaseType
    {
        Linear,

        InSine,
        OutSine,
        InOutSine,

        InQuad,
        OutQuad,
        InOutQuad,

        InCubic,
        OutCubic,
        InOutCubic,

        InQuart,
        OutQuart,
        InOutQuart,

        InQuint,
        OutQuint,
        InOutQuint,

        InExpo,
        OutExpo,
        InOutExpo,

        InCirc,
        OutCirc,
        InOutCirc,

        InBack,
        OutBack,
        InOutBack,

        InElastic,
        OutElastic,
        InOutElastic,

        InBounce,
        OutBounce,
        InOutBounce
    }

    public static class TweenFunctions
    {
        private const float value1 = 1.70158f;
        private const float value2 = value1 * 1.525f;
        private const float value3 = value1 + 1f;
        private const float value4 = 2 * Mathf.PI / 3;
        private const float value5 = 2 * Mathf.PI / 4.5f;

        private const float n = 7.5625f;
        private const float d = 2.75f;

        private static readonly Dictionary<EaseType, Func<float, float>> functions = new()
        {
            { EaseType.Linear, Linear },
            { EaseType.InSine, InSine },
            { EaseType.OutSine, OutSine },
            { EaseType.InOutSine, InOutSine },
            { EaseType.InQuad, InQuad },
            { EaseType.OutQuad, OutQuad },
            { EaseType.InOutQuad, InOutQuad },
            { EaseType.InCubic, InCubic },
            { EaseType.OutCubic, OutCubic },
            { EaseType.InOutCubic, InOutCubic },
            { EaseType.InQuart, InQuart },
            { EaseType.OutQuart, OutQuart },
            { EaseType.InOutQuart, InOutQuart },
            { EaseType.InQuint, InQuint },
            { EaseType.OutQuint, OutQuint },
            { EaseType.InOutQuint, InOutQuint },
            { EaseType.InExpo, InExpo },
            { EaseType.OutExpo, OutExpo },
            { EaseType.InOutExpo, InOutExpo },
            { EaseType.InCirc, InCirc },
            { EaseType.OutCirc, OutCirc },
            { EaseType.InOutCirc, InOutCirc },
            { EaseType.InBack, InBack },
            { EaseType.OutBack, OutBack },
            { EaseType.InOutBack, InOutBack },
            { EaseType.InElastic, InElastic },
            { EaseType.OutElastic, OutElastic },
            { EaseType.InOutElastic, InOutElastic },
            { EaseType.InBounce, InBounce },
            { EaseType.OutBounce, OutBounce },
            { EaseType.InOutBounce, InOutBounce}
        };

        public static float Evaluate(EaseType type, float time)
            => functions[type](time);
        public static float Linear(float time) => time;

        public static float InSine(float time) => 1 - Mathf.Cos((time * Mathf.PI) / 2);
        public static float OutSine(float time) => Mathf.Sin((time * Mathf.PI) / 2);
        public static float InOutSine(float time) => -(Mathf.Cos(Mathf.PI * time) - 1) / 2;
        public static float InQuad(float time) => time * time;
        public static float OutQuad(float time) => 1 - (1 - time) * (1 - time);
        public static float InOutQuad(float time) => time < 0.5f ? 2 * time * time : 1 - Mathf.Pow(-2 * time + 2, 2) / 2;
        public static float InCubic(float time) => Mathf.Pow(time, 3);
        public static float OutCubic(float time) => 1 - Mathf.Pow(1 - time, 3);
        public static float InOutCubic(float time) => time < 0.5 ? 4 * Mathf.Pow(time, 3) : 1 - Mathf.Pow(-2 * time + 2, 3) / 2;
        public static float InQuart(float time) => Mathf.Pow(time, 4);
        public static float OutQuart(float time) => 1 - Mathf.Pow(1 - time, 4);
        public static float InOutQuart(float time) => time < 0.5f ? 8 * Mathf.Pow(time, 4) : 1 - Mathf.Pow(-2 * time + 2, 4) / 2;
        public static float InQuint(float time) => Mathf.Pow(time, 5);
        public static float OutQuint(float time) => 1 - Mathf.Pow(1 - time, 5);
        public static float InOutQuint(float time) => time < 0.5f ? 16 * Mathf.Pow(time, 5) : 1 - Mathf.Pow(-2 * time + 2, 5) / 2;
        public static float InExpo(float time) => time == 0 ? 0 : Mathf.Pow(2, 10 * time - 10);
        public static float OutExpo(float time) => time == 1f ? 1f : 1 - Mathf.Pow(2, -10 * time);
        public static float InOutExpo(float time)
        {
            if (time == 0) return 0;
            if (time == 1f) return 1f;
            if (time < 0.5f)
                return Mathf.Pow(2, 20 * time - 10) / 2;
            return (2 - Mathf.Pow(2, -20 * time + 10)) / 2;
        }
        public static float InCirc(float time) => 1 - Mathf.Sqrt(1 - Mathf.Pow(time, 2));
        public static float OutCirc(float time) => Mathf.Sqrt(1 - Mathf.Pow(time - 1, 2));
        public static float InOutCirc(float time) => time < 0.5f ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * time, 2))) / 2 : (Mathf.Sqrt(1 - Mathf.Pow(-2 * time + 2, 2)) + 1) / 2;

        public static float InBack(float time) => value3 * Mathf.Pow(time, 3) - value1 * time * time;
        public static float OutBack(float time) => 1 + value3 * Mathf.Pow(time - 1, 3) + value1 * Mathf.Pow(time - 1, 2);
        public static float InOutBack(float time)
        {
            if (time < 0.5f)
                return Mathf.Pow(2 * time, 2) * ((value2 + 1) * 2 * time - value2) / 2;
            return (Mathf.Pow(2 * time - 2, 2) * ((value2 + 1) * (time * 2 - 2) + value2) + 2) / 2;
        }
        public static float InElastic(float time)
        {
            if (time == 0f) return 0f;
            if (time == 1f) return 1f;
            return -Mathf.Pow(2, 10 * time - 10) * Mathf.Sin((time * 10 - 10.75f) * value4);
        }
        public static float OutElastic(float time)
        {
            if (time == 0f) return 0f;
            if (time == 1f) return 1f;
            return Mathf.Pow(2, -10 * time) * Mathf.Sin((time * 10 - 0.75f) * value4) + 1;
        }
        public static float InOutElastic(float time)
        {
            if (time == 0f) return 0f;
            if (time == 1f) return 1f;
            if (time < 0.5f)
                return -(Mathf.Pow(2, 20 * time - 10) * Mathf.Sin((20 * time - 11.125f) * value5)) / 2;
            return Mathf.Pow(2, -20 * time + 10) * Mathf.Sin((20 * time - 11.125f) * value5) / 2 + 1;
        }

        public static float InBounce(float time) => 1 - OutBounce(1 - time);

        public static float OutBounce(float time)
        {
            if (time < 1 / d)
                return n * time * time;
            if (time < 2 / d)
                return n * (time -= 1.5f / d) * time + 0.75f;
            if (time < 2.5f / d)
                return n * (time -= 2.25f / d) * time + 0.9375f;
            return n * (time -= 2.625f / d) * time + 0.984375f;
        }

        public static float InOutBounce(float time) => time < 0.5f ? (1 - OutBounce(1 - 2 * time)) / 2 : (1 + OutBounce(2 * time - 1)) / 2;
    }
}
