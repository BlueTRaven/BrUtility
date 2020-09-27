using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
    public static class EngineMathHelper
    {
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public static float Mod(float x, float m)
        {
            float r = x % m;
            return r < 0 ? r + m : r;
        }

        public static double CosineInterpolate( double y1, double y2, double mu)
        {
            double mu2;

            mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
            return (y1 * (1 - mu2) + y2 * mu2);
        }

        public static double CubicInterpolate(double y0, double y1, double y2, double y3, double mu)
        {
            double a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = y3 - y2 - y0 + y1;
            a1 = y0 - y1 - a0;
            a2 = y2 - y0;
            a3 = y1;

            return (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }

        public static double CubicInterpCatmullRom(double y0, double y1, double y2, double y3, double mu)
        {
            double a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = -0.5 * y0 + 1.5 * y1 - 1.5 * y2 + 0.5 * y3;
            a1 = y0 - 2.5 * y1 + 2 * y2 - 0.5 * y3;
            a2 = -0.5 * y0 + 0.5 * y2;
            a3 = y1;

            return (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }

        public static double HermiteInterpolate(double y0, double y1, double y2, double y3, double mu, double tension, double bias)
        {
            double m0, m1, mu2, mu3;
            double a0, a1, a2, a3;

            mu2 = mu * mu;
            mu3 = mu2 * mu;
            m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
            m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
            m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
            m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;
            a0 = 2 * mu3 - 3 * mu2 + 1;
            a1 = mu3 - 2 * mu2 + mu;
            a2 = mu3 - mu2;
            a3 = -2 * mu3 + 3 * mu2;

            return (a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2);
        }
		
		public static float AngleLerp(float start, float end, float percent)
		{
			float max = 360;
			float da = Mod((end - start), max);
			float f = ((2 * da) % max) - da;

			return start + (f * percent);

			/*float lerpSafeAngle = end;
			float theta = end - start;
			if (theta > 180)
				start += 360;
			if (theta < -180)
				start -= 360;
			start += (lerpSafeAngle - start) * percent;

			return start;*/
		}
    }
}
