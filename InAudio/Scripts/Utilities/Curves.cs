using UnityEngine;

namespace InAudioSystem.InMath
{
    //http://www.itl.nist.gov/div898/handbook/eda/section3/eda3667.htm
    public static class Curves
    {
        public static float CumulativeDistribution(float x)
        {
            x = x*5;
            return 1.0f - Mathf.Pow(e,-(x/0.9f));
        }

        public static float CumulativeDistribution(float x, float b)
        {
            x = x * 5;
            return 1.0f - Mathf.Pow(e, -(x / b));
        }

        private const float e = 2.7182818f;
    }
}
