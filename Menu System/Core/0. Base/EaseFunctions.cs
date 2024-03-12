// Credits to: https://easings.net/# for easing functions

using System;

namespace MenuManagement.Base
{
    public static class EaseMaster
    {
        private const double Pi = 3.1415926;
        private const double PIByTwo = 1.5707963;
        private const float c1 = 1.70158f;
        private const float c2 = 2.5949095f;
        private const float c3 = 2.70158f;
        private const double c4 = 2.0943950667;
        private const float c5 = 1.3962633778f;
        private const float n1 = 7.5625f;
        private const float d1 = 2.75f;

        public delegate float Function(float timeElapsed, float duration);

        public enum Kind
        {
            // @formatter:off
            Linear,    Custom,                     // 00, 01
            InSine,    OutSine,    InOutSine,      // 02, 03, 04      
            InQuad,    OutQuad,    InOutQuad,      // 05, 06, 07
            InCubic,   OutCubic,   InOutCubic,     // 08, 09, 10
            InQuart,   OutQuart,   InOutQuart,     // 11, 12, 13
            InQuint,   OutQuint,   InOutQuint,     // 14, 15, 16
            InExpo,    OutExpo,    InOutExpo,      // 17, 18, 19
            InCirc,    OutCirc,    InOutCirc,      // 20, 21, 22
            InBack,    OutBack,    InOutBack,      // 23, 24, 25 
            InElastic, OutElastic, InOutElastic,   // 26, 27, 28
            InBounce,  OutBounce,  InOutBounce,    // 29, 30, 31
            // @formatter:on
        }

        public static Function GetFunction(Kind kind)
        {
            return kind switch
            {
                Kind.Linear => Linear,
                Kind.InSine => InSine,
                Kind.OutSine => OutSine,
                Kind.InOutSine => InOutSine,
                Kind.InQuad => InQuad,
                Kind.OutQuad => OutQuad,
                Kind.InOutQuad => InOutQuad,
                Kind.InCubic => InCubic,
                Kind.OutCubic => OutCubic,
                Kind.InOutCubic => InOutCubic,
                Kind.InQuart => InQuart,
                Kind.OutQuart => OutQuart,
                Kind.InOutQuart => InOutQuart,
                Kind.InQuint => InQuint,
                Kind.OutQuint => OutQuint,
                Kind.InOutQuint => InOutQuint,
                Kind.InExpo => InExpo,
                Kind.OutExpo => OutExpo,
                Kind.InOutExpo => InOutExpo,
                Kind.InCirc => InCirc,
                Kind.OutCirc => OutCirc,
                Kind.InOutCirc => InOutCirc,
                Kind.InBack => InBack,
                Kind.OutBack => OutBack,
                Kind.InOutBack => InOutBack,
                Kind.InElastic => InElastic,
                Kind.OutElastic => OutElastic,
                Kind.InOutElastic => InOutElastic,
                Kind.InBounce => InBounce,
                Kind.OutBounce => OutBounce,
                Kind.InOutBounce => InOutBounce,
                _ => Linear
            };
        }

        private static float Linear(float t, float d)
        {
            return t / d;
        }

        private static float InSine(float t, float d)
        {
            return (float)(1 - Math.Cos(t * PIByTwo / d));
        }

        private static float OutSine(float t, float d)
        {
            return (float)Math.Sin(t * PIByTwo / d);
        }

        private static float InOutSine(float t, float d)
        {
            return (float)((1 - Math.Cos(t * Pi / d)) * 0.5);
        }

        private static float InQuad(float t, float d)
        {
            t /= d;
            return t * t;
        }

        private static float OutQuad(float t, float d)
        {
            t = 1f - t / d;
            return 1f - t * t;
        }

        private static float InOutQuad(float t, float d)
        {
            t /= d;
            if (t < 0.5f) return 2f * t * t;
            t = 2f - 2f * t;
            return 1f - t * t * 0.5f;
        }

        private static float InCubic(float t, float d)
        {
            t *= d;
            return t * t * t;
        }

        private static float OutCubic(float t, float d)
        {
            t = 1f - t / d;
            return 1f - t * t * t;
        }

        private static float InOutCubic(float t, float d)
        {
            t /= d;
            if (t < 0.5f) return 4f * t * t * t;
            t = 2f - 2f * t;
            return 1f - t * t * t * 0.5f;
        }

        private static float InQuart(float t, float d)
        {
            t /= d;
            t *= t;
            return t * t;
        }

        private static float OutQuart(float t, float d)
        {
            t = 1f - t / d;
            t *= t;
            return 1f - t * t;
        }

        private static float InOutQuart(float t, float d)
        {
            t /= d;
            if (t < 0.5f)
            {
                t *= t;
                return 8f * t * t;
            }

            t = 2f - 2f * t;
            t *= t;
            return 1f - t * t * 0.5f;
        }

        private static float InQuint(float t, float d)
        {
            t /= d;
            d = t * t;
            return d * d * t;
        }

        private static float OutQuint(float t, float d)
        {
            t = 1f - t / d;
            d = t * t;
            return 1f - d * d * t;
        }

        private static float InOutQuint(float t, float d)
        {
            t /= d;
            if (t < 0.5f)
            {
                d = t * t;
                return 16f * d * d * t;
            }

            t = 2 - 2 * t;
            d = t * t;
            return 1f - d * d * t * 0.5f;
        }

        private static float InExpo(float t, float d)
        {
            t /= d;
            return t == 0f ? 0f : (float)Math.Pow(2, 10 * t - 10);
        }

        private static float OutExpo(float t, float d)
        {
            t /= d;
            if (t >= 1) return 1;
            return (float)(1f - Math.Pow(2, -10 * t));
        }

        private static float InOutExpo(float t, float d)
        {
            t /= d;
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            if (t < 0.5f) return (float)(Math.Pow(2, 20 * t - 10) * 0.5);
            return (float)((2 - Math.Pow(2, -20 * t + 10)) * 0.5);
        }

        private static float InCirc(float t, float d)
        {
            t /= d;
            return (float)(1f - Math.Sqrt(1f - t * t));
        }

        private static float OutCirc(float t, float d)
        {
            t = 1 - t / d;
            return (float)Math.Sqrt(1 - t * t);
        }

        private static float InOutCirc(float t, float d)
        {
            t /= d;
            if (t < 0.5) return (float)((1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) * 0.5);
            return (float)((Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) * 0.5);
        }

        private static float InBack(float t, float d)
        {
            t /= d;
            return (c3 * t - c1) * t * t;
        }

        private static float OutBack(float t, float d)
        {
            t = t / d - 1f;
            return 1f + (c3 * t + c1) * t * t;
        }

        private static float InOutBack(float t, float d)
        {
            t /= d;
            if (t < 0.5f) return (4f * t * t * ((c2 + 1f) * 2f * t - c2)) * 0.5f;

            float sq = (float)Math.Pow(2f * t - 2f, 2f);
            return (sq * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) * 0.5f;
        }

        private static float InElastic(float t, float d)
        {
            t /= d;
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return (float)(-Math.Pow(1.5, 10 * t - 10) * Math.Sin((t * 10 - 10.75) * c4));
        }

        private static float OutElastic(float t, float d)
        {
            t /= d;
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return (float)(Math.Pow(1.5, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1);
        }

        private static float InOutElastic(float t, float d)
        {
            t /= d;
            if (t <= 0) return 0;
            if (t >= 1) return 1;
            if (t < 0.5) return (float)(-(Math.Pow(1.5, 20 * t - 10) * Math.Sin((20 * t - 11.125) * c5)) * 0.5);
            return (float)((Math.Pow(1.5, -20 * t + 10) * Math.Sin((20 * t - 11.125) * c5)) * 0.5 + 1);
        }

        private static float Bounce(float x)
        {
            if (x < 1 / d1) return n1 * x * x;
            if (x < 2 / d1) return n1 * (x -= 1.5f / d1) * x + 0.75f;
            if (x < 2.5 / d1) return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }

        private static float InBounce(float t, float d)
        {
            return 1 - Bounce(1 - t / d);
        }

        private static float OutBounce(float t, float d)
        {
            return Bounce(t / d);
        }

        private static float InOutBounce(float t, float d)
        {
            t /= d;
            if (t < 0.5) return (1 - Bounce(1 - 2 * t)) / 2;
            return (1 + Bounce(2 * t - 1)) / 2;
        }
    }
}