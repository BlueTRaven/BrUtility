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
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                int typeV = (int)(object)type;
                int valV = (int)(object)value;
                return ((typeV & valV) == valV);
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
                return (int)(object)type == (int)(object)value;
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
                return (T)(object)(((int)(object)type | (int)(object)value));
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
                return (T)(object)(((int)(object)type & ~(int)(object)value));
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
                    return (T)(object)(((int)(object)type & ~(int)(object)value));
                else return (T)(object)(((int)(object)type | (int)(object)value));
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
	}
}
