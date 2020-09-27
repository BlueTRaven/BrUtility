using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BrUtility
{
    public static class VectorHelper
    {
		public static Vector2 AngleToVector(float angle, bool radians = true)
		{
			if (!radians)
				angle = MathHelper.ToRadians(angle);

			return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
		}

		public static Vector2 RotateBy(this Vector2 vector, float degrees)
        {
            var temp = Vector2.Transform(vector, Matrix.CreateRotationZ(MathHelper.ToRadians(degrees)));
            vector.X = temp.X;
            vector.Y = temp.Y;
            return vector;
        }

        public static Vector2 RoundToAngle(this Vector2 vector, int roundAngle)
        {
            float currentAngle = GetVectorAngle(vector);

            float len = vector.Length();

            return GetAngleNormVector(currentAngle.RoundDown(roundAngle)) * len;
        }
        /// <summary>
        /// Gets the angle of a given vector, relative to 0,0.
        /// </summary>
        /// <returns>The angle in degrees.</returns>
        public static float GetVectorAngle(Vector2 normVec)
        {
			return MathHelper.ToDegrees((float)Math.Atan2(normVec.Y, normVec.X));
        }

        public static Vector2 GetAngleNormVector(float degrees)
        {
            var vec = Vector2.Transform(new Vector2(-1, 0), Matrix.CreateRotationZ(MathHelper.ToRadians(degrees)));
            if (float.IsNaN(vec.X) || float.IsNaN(vec.Y))
                return Vector2.Zero;
            return vec;
        }

        public static int Next(this Random rand, float val1, float val2)
        {
            return rand.Next((int)val1, (int)val2);
        }

        public static byte[] ConvertVectorToBytes(Vector2 vector)
        {
            float[] vecFloats = new float[] { vector.X, vector.Y };
            byte[] fByte = new byte[8];

            Buffer.BlockCopy(vecFloats, 0, fByte, 0, fByte.Length);

            return fByte;
        }

        public static float GetAngleBetweenPoints(Vector2 A, Vector2 B)
        {
            return EngineMathHelper.Mod(MathHelper.ToDegrees((float)Math.Atan2(A.Y - B.Y, A.X - B.X)), 360);
        }

        public static Vector2 GetNormal(Vector2 A, Vector2 B)
        {
            return Vector2.Normalize(GetPerp(B - A));
        }

        public static Vector2 GetPerp(Vector2 vector, bool right = false)
        {
            if (!right)
                return new Vector2(-vector.Y, vector.X);
            else return new Vector2(vector.Y, -vector.X);
        }

        public static Vector2 ProjectToAxis(Vector2 toProject, string axis)
        {
            Vector2 projected = Vector2.Zero;

            if (axis == "horizontal")
                projected = new Vector2(toProject.X, 0);
            else if (axis == "vertical")
                projected = new Vector2(0, toProject.Y);

            return projected;
        }

        public static Vector2 ProjectToVector(Vector2 A, Vector2 B)
        {
            Vector2 final;
            float dot = Vector2.Dot(A, B);
            final.X = (dot / (B.X * B.X + B.Y * B.Y)) * B.X;
            final.Y = (dot / (B.X * B.X + B.Y * B.Y)) * B.Y;

            return final;
            //return projection + Vector2.Dot(toProject - projection, Vector2.Normalize(projection)) * Vector2.Normalize(projection);
        }

        public static float ProjectToVectorSigned(Vector2 A, Vector2 B)
        {
            return Vector2.Dot(A, GetNormal(A, B));
        }

        public static Vector2 FindLargest(Vector2[] vectors)
        {
            Vector2 longest = Vector2.Zero;
            foreach (Vector2 v in vectors)
            {
                if (v.Length() > longest.Length())
                    longest = v;
            }

            return longest;
        }

        public static Vector2 FindSmallest(Vector2[] vectors)
        {
            Vector2 smallest = vectors[0];
            foreach (Vector2 v in vectors)
            {
                if (v.Length() < smallest.Length())
                    smallest = v;
            }

            return smallest;
        }

        public static Vector2 GetMidPoint(Vector2 A, Vector2 B)
        {
            return (A + B) / 2f;
        }

        public static Vector2 WorldToScreenCoords(this Vector2 worldCoords, Camera camera)
        {
            return worldCoords - camera.Position;
        }

        public static Vector2 ScreenToWorldCoords(this Vector2 screenCoords, Camera camera)
        {
            return screenCoords + camera.Position;
            //return Vector2.Transform(screenCoords, Camera.camera.GetInverseViewMatrix());
        }


		public static Vector2 RoundToTileSpace(Vector2 vector, float tileSize)
		{
			float x = (float)Math.Round(vector.X / tileSize) * tileSize;
			float y = (float)Math.Round(vector.Y / tileSize) * tileSize;

			return new Vector2(x, y);
		}

        public static bool LineIntersectsRect(Point p1, Point p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

		public static Vector2 ClampToRect(RectangleF rect, Vector2 position)
		{
			return Vector2.Clamp(position, rect.Position, rect.Position + rect.Size.ToVector2());
		}
	}
}
