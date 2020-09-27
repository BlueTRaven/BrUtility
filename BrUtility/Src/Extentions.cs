using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrUtility
{
	public static class Extentions
	{
		#region Size
		public static Size ToSize(this Vector2 vector)
		{
			return new Size(vector.X, vector.Y);
		}

		public static Size ToSize(this Point point)
		{
			return new Size(point.X, point.Y);
		}
		#endregion

		public static RectangleF ToRectangleF(this Rectangle rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void CheckAndDelete<T>(this List<T> sequence, Func<T, bool> deletableSelector)
        {
            foreach (T value in sequence.ToList())
            {
                if (deletableSelector(value))
                {
                    sequence.Remove(value);
                }
            }
        }

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Random rand, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = rand.NextFloat() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }

		#region Round
		public static int RoundDown(this int integer, int size)
        {
            return (int)(Math.Floor((double)(integer / size)) * size);
        }

        public static int RoundUp(this int integer, int size)
        {
            return (int)(Math.Ceiling((double)(integer / size)) * size);
        }

        public static float RoundDown(this float integer, int size)
        {
            return (int)(Math.Floor((double)(integer / size)) * size);
        }

        public static float RoundUp(this float integer, int size)
        {
            return (int)(Math.Ceiling((double)(integer / size)) * size);
        }
		#endregion

		public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

		#region Random
		public static int NextSign(this Random rand)
		{
			return rand.NextCoinFlip() ? -1 : 1;
		}

		public static Vector2 NextAngle(this Random rand)
        {
            return Vector2.Transform(new Vector2(-1, 0), Matrix.CreateRotationZ(MathHelper.ToRadians(rand.NextFloat(0, 360))));
        }

        public static Vector2 NextPointInside(this Random rand, Rectangle rectangle)
        {
            float x = rand.NextFloat(rectangle.X, rectangle.X + rectangle.Width);
            float y = rand.NextFloat(rectangle.Y, rectangle.Y + rectangle.Height);

            return new Vector2(x, y);
        }

        public static double NextDouble(this Random rand, double minimum, double maximum)
        {
            return rand.NextDouble() * (maximum - minimum) + minimum;
        }

        public static float NextFloat(this Random rand)
        {
            return (float)rand.NextDouble();
        }

        public static float NextFloat(this Random rand, float minimum, float maximum)
        {
            return (float)rand.NextDouble() * (maximum - minimum) + minimum;
        }

        public static bool NextCoinFlip(this Random rand)
        {   //non inclusive, so either 0 or 1
            return rand.Next(2) == 0;
        }

		public static Vector2 NextInside(this Random rand, Rectangle rectangle)
		{
			return new Vector2(rand.NextFloat(rectangle.X, rectangle.X + rectangle.Width), rand.NextFloat(rectangle.Y, rectangle.Y + rectangle.Height));
		}

		public static Vector2 NextInside(this Random rand, in Rectangle rectangle)
		{
			return new Vector2(rand.NextFloat(rectangle.X, rectangle.X + rectangle.Width), rand.NextFloat(rectangle.Y, rectangle.Y + rectangle.Height));
		}

		public static Vector2 NextOnEdge(this Random rand, Rectangle rectangle)
		{
			int side = rand.Next(4);

			switch (side)
			{
				case 0: //top side
					return new Vector2(rand.Next(rectangle.X, rectangle.X + rectangle.Width), rectangle.Y);
				case 1: //right side
					return new Vector2(rectangle.X + rectangle.Width, rand.Next(rectangle.Y, rectangle.Y + rectangle.Height));
				case 2: //bottom side
					return new Vector2(rand.Next(rectangle.X, rectangle.X + rectangle.Width), rectangle.Y + rectangle.Height);
				case 3: //left side
					return new Vector2(rectangle.X, rand.Next(rectangle.Y, rectangle.Y + rectangle.Height));
			}

			return Vector2.Zero;
		}

		public static Vector2 NextInside(this Random rand, RectangleF rectangle)
		{
			return rand.NextInside(rectangle.ToRectangle());
		}

		public static Vector2 NextInside(this Random rand, in RectangleF rectangle)
		{
			return rand.NextInside(rectangle);
		}

		public static Vector2 NextOnEdge(this Random rand, RectangleF rectangle)
		{
			return rand.NextOnEdge(rectangle.ToRectangle());
		}
		#endregion
	}

    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }

        public T Dequeue()
        {
            lock (syncObject)
            {
                T outObj;
                if (base.TryDequeue(out outObj))
                    return outObj;
                return default(T);
            }
        }

        public void Clear()
        {
            while (Count > 0)   //this is... probably not the best way to do things.
                Dequeue();
        }

        public T Peek()
        {
            T outobj;
            base.TryPeek(out outobj);
            return outobj;
        }
    }
}