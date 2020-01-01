using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility
{
	public struct Size
	{
		public float Width { get; set; }
		public float Height { get; set; }

		public static Size Zero => new Size(0);

		public Size(float width, float height)
		{
			this.Width = width;
			this.Height = height;
		}

		public Size(float size)
		{
			this.Width = size;
			this.Height = size;
		}

		public Vector2 ToVector2()
		{
			return new Vector2(Width, Height);
		}

		public Point ToPoint()
		{
			return new Point((int)Width, (int)Height);
		}

		public override string ToString()
		{
			return string.Format("Width: {0}, Height: {1}", Width, Height);
		}

		public static Size operator +(Size size, Size other)
		{
			return new Size(size.Width + other.Width, size.Height + other.Height);
		}
	}
}
