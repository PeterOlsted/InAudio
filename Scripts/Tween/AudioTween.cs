using System;
using InAudioLeanTween;
using UnityEngine;

//InAudio 2015
//Extracted core functions for easy of access

// Copyright (c) 2013 Russell Savage - Dented Pixel
// 
// LeanTween version 2.14 - http://dentedpixel.com/developer-diary/
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/*
TERMS OF USE - EASING EQUATIONS#
Open source under the BSD License.
Copyright (c)2001 Robert Penner
All rights reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace InAudioSystem.Runtime
{
    public class AudioTween
    {

        public static float DirectTween(LeanTweenType tweenType, float fromValue, float toValue, float ratioPassed)
        {
            float returnVal = 0f;
            float diff = toValue - fromValue;
            switch (tweenType)
            {
                case LeanTweenType.linear:
                    returnVal = fromValue + diff * ratioPassed;
                    break;
                case LeanTweenType.easeOutQuad:
                    returnVal = easeOutQuadOpt(fromValue, diff, ratioPassed);
                    break;
                case LeanTweenType.easeInQuad:
                    returnVal = easeInQuadOpt(fromValue, diff, ratioPassed);
                    break;
                case LeanTweenType.easeInOutQuad:
                    returnVal = easeInOutQuadOpt(fromValue, diff, ratioPassed);
                    break;
                case LeanTweenType.easeInCubic:
                    returnVal = easeInCubic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutCubic:
                    returnVal = easeOutCubic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutCubic:
                    returnVal = easeInOutCubic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInQuart:
                    returnVal = easeInQuart(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutQuart:
                    returnVal = easeOutQuart(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutQuart:
                    returnVal = easeInOutQuart(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInQuint:
                    returnVal = easeInQuint(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutQuint:
                    returnVal = easeOutQuint(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutQuint:
                    returnVal = easeInOutQuint(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInSine:
                    returnVal = easeInSine(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutSine:
                    returnVal = easeOutSine(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutSine:
                    returnVal = easeInOutSine(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInExpo:
                    returnVal = easeInExpo(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutExpo:
                    returnVal = easeOutExpo(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutExpo:
                    returnVal = easeInOutExpo(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInCirc:
                    returnVal = easeInCirc(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutCirc:
                    returnVal = easeOutCirc(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutCirc:
                    returnVal = easeInOutCirc(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInBounce:
                    returnVal = easeInBounce(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutBounce:
                    returnVal = easeOutBounce(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutBounce:
                    returnVal = easeInOutBounce(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInBack:
                    returnVal = easeInBack(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutBack:
                    returnVal = easeOutBack(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutBack:
                    returnVal = easeInOutElastic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInElastic:
                    returnVal = easeInElastic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeOutElastic:
                    returnVal = easeOutElastic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.easeInOutElastic:
                    returnVal = easeInOutElastic(fromValue, toValue, ratioPassed);
                    break;
                case LeanTweenType.punch:
                case LeanTweenType.easeShake:
                    throw new ArgumentException("InAudio: This type of fading is not supported");
                //if (tween.tweenType == LeanTweenType.punch)
                //{
                //    tween.animationCurve = LeanTween.punch;
                //}
                //else if (tween.tweenType == LeanTweenType.easeShake)
                //{
                //    tween.animationCurve = LeanTween.shake;
                //}
                //toValue = fromValue + toValue;
                //diff = toValue - fromValue;
                //returnVal = tweenOnCurve(tween, ratioPassed);
                //break;
                case LeanTweenType.easeSpring:
                    returnVal = spring(fromValue, toValue, ratioPassed);
                    break;
                default:
                    {
                        returnVal = fromValue + diff * ratioPassed;
                        break;
                    }
            }
            return returnVal;
        }

        private static float tweenOnCurve(LTDescr tweenDescr, float ratioPassed)
        {
            // Debug.Log("single ratio:"+ratioPassed+" tweenDescr.animationCurve.Evaluate(ratioPassed):"+tweenDescr.animationCurve.Evaluate(ratioPassed));
            return tweenDescr.from.x + (tweenDescr.diff.x) * tweenDescr.animationCurve.Evaluate(ratioPassed);
        }

        private static Vector3 tweenOnCurveVector(LTDescr tweenDescr, float ratioPassed)
        {
            return new Vector3(tweenDescr.from.x + (tweenDescr.diff.x) * tweenDescr.animationCurve.Evaluate(ratioPassed),
                tweenDescr.from.y + (tweenDescr.diff.y) * tweenDescr.animationCurve.Evaluate(ratioPassed),
                tweenDescr.from.z + (tweenDescr.diff.z) * tweenDescr.animationCurve.Evaluate(ratioPassed));
        }

        private static float easeOutQuadOpt(float start, float diff, float ratioPassed)
        {
            return -diff * ratioPassed * (ratioPassed - 2) + start;
        }

        private static float easeInQuadOpt(float start, float diff, float ratioPassed)
        {
            return diff * ratioPassed * ratioPassed + start;
        }

        private static float easeInOutQuadOpt(float start, float diff, float ratioPassed)
        {
            ratioPassed /= .5f;
            if (ratioPassed < 1) return diff / 2 * ratioPassed * ratioPassed + start;
            ratioPassed--;
            return -diff / 2 * (ratioPassed * (ratioPassed - 2) - 1) + start;
        }

        private static float linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }

        private static float clerp(float start, float end, float val)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * val;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * val;
                retval = start + diff;
            }
            else retval = start + (end - start) * val;
            return retval;
        }

        private static float spring(float start, float end, float val)
        {
            val = Mathf.Clamp01(val);
            val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) *
                  (1f + (1.2f * (1f - val)));
            return start + (end - start) * val;
        }

        private static float easeInQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }

        private static float easeOutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }

        private static float easeInOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val + start;
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }

        private static float easeInCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }

        private static float easeOutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }

        private static float easeInOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }

        private static float easeInQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }

        private static float easeOutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }

        private static float easeInOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val + start;
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }

        private static float easeInQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }

        private static float easeOutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }

        private static float easeInOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }

        private static float easeInSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }

        private static float easeOutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }

        private static float easeInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }

        private static float easeInExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }

        private static float easeOutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }

        private static float easeInOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }

        private static float easeInCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }

        private static float easeOutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }

        private static float easeInOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }

        /* GFX47 MOD START */

        private static float easeInBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - easeOutBounce(0, end, d - val) + start;
        }

        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //public static function bounce(float start, float end, float val){
        private static float easeOutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            else if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            else if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            else
            {
                val -= (2.625f / 2.75f);
                return end * (7.5625f * (val) * val + .984375f) + start;
            }
        }

        /* GFX47 MOD END */

        /* GFX47 MOD START */

        private static float easeInOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2) return easeInBounce(0, end, val * 2) * 0.5f + start;
            else return easeOutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }

        /* GFX47 MOD END */

        private static float easeInBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }

        private static float easeOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val = (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }

        private static float easeInOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }
            val -= 2;
            s *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }

        /* GFX47 MOD START */

        private static float easeInElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;
            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }
            val = val - 1;
            return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //public static function elastic(float start, float end, float val){
        private static float easeOutElastic(float start, float end, float val)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        /* GFX47 MOD START */

        private static float easeInOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / (d / 2);
            if (val == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1)
            {
                val = val - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
            }
            val = val - 1;
            return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
    }

}