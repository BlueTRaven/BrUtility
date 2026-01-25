using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
    public static class EngineMathHelper
    {
        public static Vector3 QuaternionToYawPitchRoll(Quaternion quat)
        {
            // WARNING: ChatGPT generated this!
            // Pitch (X axis)
            double sinp = 2.0 * (quat.W * quat.X - quat.Y * quat.Z);
            double pitch;
            if (Math.Abs(sinp) >= 1)
            {
                var sign = Math.Sign(sinp);
                pitch = Math.PI / 2 * sign; // gimbal lock
            }
            else
                pitch = Math.Asin(sinp);

            // Yaw (Y axis)
            double siny = 2.0 * (quat.W * quat.Y + quat.Z * quat.X);
            double cosy = 1.0 - 2.0 * (quat.X * quat.X + quat.Y * quat.Y);
            double yaw = Math.Atan2(siny, cosy);

            // Roll (Z axis)
            double sinr = 2.0 * (quat.W * quat.Z + quat.X * quat.Y);
            double cosr = 1.0 - 2.0 * (quat.X * quat.X + quat.Z * quat.Z);
            double roll = Math.Atan2(sinr, cosr);

            return new Vector3((float)yaw, (float)pitch, (float)roll);

            //float roll = (float)Math.Atan2(2 * (quat.W * quat.X + quat.Y * quat.Z), 1 - 2 * (quat.X * quat.X + quat.Y * quat.Y));

            //float pitch = (float)Math.Asin(2 * (quat.W * quat.Y - quat.Z * quat.X));

            //float yaw = (float)Math.Atan2(2 * (quat.W * quat.Z + quat.X * quat.Y), 1 - 2 * (quat.Y * quat.Y + quat.Z * quat.Z));

            //return new Vector3(yaw, pitch, roll);
        }

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

        // WARNING: chatgpt generated
        public static Quaternion LookRotation(Vector3 direction, Vector3 up)
        {
            if (direction == Vector3.Zero)
                return Quaternion.Identity;

            direction.Normalize();

            // Create an orthonormal basis
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, direction));
            Vector3 correctedUp = Vector3.Cross(direction, right);

            Matrix rotationMatrix = new Matrix(
                right.X, right.Y, right.Z, 0,
                correctedUp.X, correctedUp.Y, correctedUp.Z, 0,
                direction.X, direction.Y, direction.Z, 0,
                0, 0, 0, 1
            );

            return Quaternion.CreateFromRotationMatrix(rotationMatrix);
        }
    }
}
