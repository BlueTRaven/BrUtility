using BlueRavenUtility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public struct Angle
	{
		private float radians;

		public float Radians => radians;
		public float Degrees { get { return MathHelper.ToDegrees(radians); } set { radians = EngineMathHelper.Mod(MathHelper.ToRadians(value), MathHelper.Pi * 2); } }

		public Enums.DirectionCardinal Direction => GetDirection(Degrees);

		public static Angle Zero => FromRadians(0);

		private Angle(float radians)
		{
			this.radians = EngineMathHelper.Mod(radians, MathHelper.Pi * 2);
		}

		public Angle Reverse()
		{
			return FromRadians(radians * -1);
		}

		public static Angle Lerp(Angle fromAngle, Angle toAngle, float delta)
		{
			float degrees = EngineMathHelper.AngleLerp(fromAngle.Degrees, toAngle.Degrees, delta);
			return FromDegrees(degrees);
		}

		public static Angle FromRadians(float radians)
		{
			return new Angle(radians);
		}

		public static Angle FromDegrees(float degrees)
		{
			Angle angle = new Angle();
			angle.Degrees = degrees;
			return angle;
		}

		public static Angle FromVector2(Vector2 vector)
		{
			return FromRadians((float)Math.Atan2(vector.Y, vector.X));
		}

		public override string ToString()
		{
			return "Angle - Degrees: " + Degrees.ToString() + ", Radians: " + Radians;
		}

		public static Enums.DirectionCardinal GetDirection(float degrees)
		{
			if ((degrees >= 315 && degrees < 360) || (degrees >= 0 && degrees < 45))
				return Enums.DirectionCardinal.East;
			else if (degrees >= 45 && degrees < 135)
				return Enums.DirectionCardinal.North;
			else if (degrees >= 135 && degrees < 225)
				return Enums.DirectionCardinal.West;
			else if (degrees >= 225 && degrees < 315)
				return Enums.DirectionCardinal.South;

			return Enums.DirectionCardinal.South; 
		}

		#region Operators
		/// <summary>
		/// Angle + Angle is radians addition
		/// </summary>
		/// <param name="thisAngle"></param>
		/// <param name="otherAngle"></param>
		/// <returns></returns>
		public static Angle operator +(Angle thisAngle, Angle otherAngle)
		{
			return FromRadians(thisAngle.radians + otherAngle.radians);
		}

		public static Angle operator -(Angle thisAngle, Angle otherAngle)
		{
			return FromRadians(thisAngle.radians - otherAngle.radians);
		}

		/// <summary>
		/// Angle + int is degrees addition, because integers are more likely to be degrees (you can't define radians with integers, at least, not very well.)
		/// </summary>
		/// <param name="thisAngle"></param>
		/// <param name="otherAngleDegrees"></param>
		/// <returns></returns>
		public static Angle operator +(Angle thisAngle, int otherAngleDegrees)
		{
			return FromDegrees(thisAngle.Degrees + otherAngleDegrees);
		}

		public static Angle operator -(Angle thisAngle, int otherAngleDegrees)
		{
			return FromDegrees(thisAngle.Degrees - otherAngleDegrees);
		}

		/// <summary>
		/// Angle + float is radians addition, because floats are more likely to be radians.
		/// </summary>
		/// <param name="thisAngle"></param>
		/// <param name="otherAngleRadians"></param>
		/// <returns></returns>
		public static Angle operator + (Angle thisAngle, float otherAngleRadians)
		{
			return FromRadians(thisAngle.radians + otherAngleRadians);
		}

		public static Angle operator -(Angle thisAngle, float otherAngleRadians)
		{
			return FromRadians(thisAngle.Radians - otherAngleRadians);
		}

		public static Angle operator +(Angle thisAngle, double otherAngleRadians)
		{
			return thisAngle + (float)otherAngleRadians;	//float addition since we already use floats
		}

		public static Angle operator -(Angle thisAngle, double otherAngleRadians)
		{
			return thisAngle - (float)otherAngleRadians;
		}
		#endregion
	}
}
