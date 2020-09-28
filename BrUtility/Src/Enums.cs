using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility
{
    public static class Enums
    {
        public enum Alignment
        {
            Center,
            Left,
            TopLeft,
            BottomLeft,
            Right,
            TopRight,
            BottomRight,
            Top,
            Bottom
        }

        public enum DirectionMirror
        {
            Horizontal,
            Vertical
        }

        public enum DirectionBinary
        {
            Left,
            Right
        }

        public enum DirectionClock
        {
            Clockwise,
            CounterClockwise
        }

        public enum DirectionCardinal
        {
            West,
            North,
            East,
            South
        }

		public enum DirectionFrontal
		{
			Left,
			Back,
			Right,
			Front
		}

        public enum DirectionCardinalG
        {
			None,
            Left = 1 << 0,
            Up = 1 << 1,
            Right = 1 << 2,
            Down = 1 << 3,
			All = Left | Up | Right | Down
        }

        public enum DirectionInterCardinal
        {
            West,
            NorthWest,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest
        }

		#region extentions
		/// <summary>
		/// Must be an enum that is one of the following types:
		/// <list type="bullet|table">
		/// <item>
		/// <term>int</term>
		/// </item>
		/// <item>
		/// <term>uint</term>
		/// </item>
		/// <item>
		/// <term>long</term>
		/// </item>
		/// <item>
		/// <term>ulong</term>
		/// </item>
		/// <item>
		/// <term>short</term>
		/// </item>
		/// <item>
		/// <term>ushort</term>
		/// </item>
		/// <item>
		/// <term>byte</term>
		/// </item>
		/// </list>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
				var underlyingType = Enum.GetUnderlyingType(typeof(T));
				
				if (underlyingType == typeof(int))
				{
					int typeV = (int)(object)type;
					int valV = (int)(object)value;
					return ((typeV & valV) == valV);
				}
				else if (underlyingType == typeof(uint))
				{
					uint typeV = (uint)(object)type;
					uint valV = (uint)(object)value;
					return ((typeV & valV) == valV);
				}
				else if (underlyingType == typeof(long))
				{
					long typeV = (long)(object)type;
					long valV = (long)(object)value;
					return ((typeV & valV) == valV);
				}
				else if (underlyingType == typeof(ulong))
				{
					ulong typeV = (ulong)(object)type;
					ulong valV = (ulong)(object)value;
					return ((typeV & valV) == valV);
				}
				else if (underlyingType == typeof(short))
				{
					short typeV = (short)(object)type;
					short valV = (short)(object)value;
					return ((typeV & valV) == valV);
				}
				else if (underlyingType == typeof(ushort))
				{
					ushort typeV = (ushort)(object)type;
					ushort valV = (ushort)(object)value;
					return ((typeV & valV) == valV);
				}
				else if (underlyingType == typeof(byte))
				{
					byte typeV = (byte)(object)type;
					byte valV = (byte)(object)value;
					return ((typeV & valV) == valV);
				}
				else
				{
					throw new Exception("Has<T> does not support enums of type " + underlyingType + ".");
				}
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
				var underlyingType = Enum.GetUnderlyingType(typeof(T));

				if (underlyingType == typeof(int))
					return (int)(object)type == (int)(object)value;
				else if (underlyingType == typeof(uint))
					return (uint)(object)type == (uint)(object)value;
				else if (underlyingType == typeof(long))
					return (long)(object)type == (long)(object)value;
				else if (underlyingType == typeof(ulong))
					return (ulong)(object)type == (ulong)(object)value;
				else if (underlyingType == typeof(short))
					return (short)(object)type == (short)(object)value;
				else if (underlyingType == typeof(ushort))
					return (ushort)(object)type == (ushort)(object)value;
				else if (underlyingType == typeof(byte))
					return (byte)(object)type == (byte)(object)value;
				else
				{
					throw new Exception("Is<T> does not support enums of type " + underlyingType + ".");
				}
            }
            catch
            {
                return false;
            }
        }

        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
				var underlyingType = Enum.GetUnderlyingType(typeof(T));

				if (underlyingType == typeof(int))
					return (T)(object)(((int)(object)type | (int)(object)value));
				else if (underlyingType == typeof(uint))
					return (T)(object)(((uint)(object)type | (uint)(object)value));
				else if (underlyingType == typeof(long))
					return (T)(object)(((long)(object)type | (long)(object)value));
				else if (underlyingType == typeof(ulong))
					return (T)(object)(((ulong)(object)type | (ulong)(object)value));
				else if (underlyingType == typeof(short))
					return (T)(object)(((short)(object)type | (short)(object)value));
				else if (underlyingType == typeof(ushort))
					return (T)(object)(((ushort)(object)type | (ushort)(object)value));
				else if (underlyingType == typeof(byte))
					return (T)(object)(((byte)(object)type | (byte)(object)value));
				else
				{
					throw new Exception("Add<T> does not support enums of type " + underlyingType + ".");
				}
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
				var underlyingType = Enum.GetUnderlyingType(typeof(T));

				if (underlyingType == typeof(int))
					return (T)(object)(((int)(object)type & ~(int)(object)value));
				else if (underlyingType == typeof(uint))
					return (T)(object)(((uint)(object)type & ~(uint)(object)value));
				else if (underlyingType == typeof(long))
					return (T)(object)(((long)(object)type & ~(long)(object)value));
				else if (underlyingType == typeof(ulong))
					return (T)(object)(((ulong)(object)type & ~(ulong)(object)value));
				else if (underlyingType == typeof(short))
					return (T)(object)(((short)(object)type & ~(short)(object)value));
				else if (underlyingType == typeof(ushort))
					return (T)(object)(((ushort)(object)type & ~(ushort)(object)value));
				else if (underlyingType == typeof(byte))
					return (T)(object)(((byte)(object)type & ~(byte)(object)value));
				else
				{
					throw new Exception("Add<T> does not support enums of type " + underlyingType + ".");
				}
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static T Toggle<T>(this System.Enum type, T value)
        {
            try
            {
				if (type.Has(value))
					return type.Add(value);
				else return type.Remove(value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not toggle value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
        #endregion  

		public static Vector2 DirectionToVector(DirectionCardinal cardinal)
		{
			switch (cardinal)
			{
				case DirectionCardinal.West:
					return new Vector2(-1, 0);
				case DirectionCardinal.North:
					return new Vector2(0, -1);
				case DirectionCardinal.East:
					return new Vector2(1, 0);
				case DirectionCardinal.South:
					return new Vector2(0, 1);
				default:
					return new Vector2(0, 0);
			}
		}

		public static Vector2 DirectionToVector(DirectionCardinalG cardinal)
		{
			switch (cardinal)
			{
				case DirectionCardinalG.Left:
					return new Vector2(-1, 0);
				case DirectionCardinalG.Up:
					return new Vector2(0, -1);
				case DirectionCardinalG.Right:
					return new Vector2(1, 0);
				case DirectionCardinalG.Down:
					return new Vector2(0, 1);
				default:
					return new Vector2(0, 0);
			}
		}

		public static DirectionCardinal Reverse(DirectionCardinal direction)
		{
			switch (direction)
			{
				case DirectionCardinal.West:
					return DirectionCardinal.East;
				case DirectionCardinal.North:
					return DirectionCardinal.South;
				case DirectionCardinal.East:
					return DirectionCardinal.West;
				case DirectionCardinal.South:
					return DirectionCardinal.North;
				default: return DirectionCardinal.North;
			}
		}

		public static DirectionCardinalG Reverse(DirectionCardinalG direction)
		{
			switch (direction)
			{
				case DirectionCardinalG.Left:
					return DirectionCardinalG.Right;
				case DirectionCardinalG.Up:
					return DirectionCardinalG.Down;
				case DirectionCardinalG.Right:
					return DirectionCardinalG.Left;
				case DirectionCardinalG.Down:
					return DirectionCardinalG.Up;
				default: return DirectionCardinalG.Up;
			}
		}
	}
}