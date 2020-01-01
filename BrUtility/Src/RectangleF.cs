using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace BlueRavenUtility
{
    public struct RectangleF
    {
        public static RectangleF Empty { get { return new RectangleF { x = 0, y = 0, width = 0, height = 0 }; } set { } }

        public float x, y, width, height;

		[JsonIgnore]
		public Vector2 Position { get { return new Vector2(x, y); } set { x = value.X; y = value.Y; } }
		[JsonIgnore]
		public Size Size { get { return new Size(width, height); } set { width = value.Width; height = value.Height; } }

		[JsonIgnore]
		public Vector2 FarPosition => Position + Size.ToVector2();

		[JsonIgnore]
		public Vector2 Center => Position + (Size.ToVector2() / 2);

		[JsonIgnore]
		public float Left => Position.X;
		[JsonIgnore]
		public float Top => Position.Y;
		[JsonIgnore]
		public float Right => Position.X + Size.Width;
		[JsonIgnore]
		public float Bottom => Position.Y + Size.Height;

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleF(Vector2 xy, Vector2 wh)
        {
            this.x = xy.X;
            this.y = xy.Y;
            this.width = wh.X;
            this.height = wh.Y;
        }

        public RectangleF(float x, float y, Vector2 wh)
        {
            this.x = x;
            this.y = y;
            this.width = wh.X;
            this.height = wh.Y;
        }

		public RectangleF(Vector2 xy, Size wh)
		{
			this.x = xy.X;
			this.y = xy.Y;
			this.width = wh.Width;
			this.height = wh.Height;
		}

		public RectangleF(float x, float y, Size wh)
		{
			this.x = x;
			this.y = y;
			this.width = wh.Width;
			this.height = wh.Height;
		}

        public RectangleF(Vector2 xy, float width, float height)
        {
            this.x = xy.X;
            this.y = xy.Y;
            this.width = width;
            this.height = height;
        }

		public bool Intersects(RectangleF rectangle)
		{	//cheat and just use non-float version. Will be slightly less accurate; may have to update for floating-point precision.
			return rectangle.ToRectangle().Intersects(ToRectangle());
		}

        public bool Contains(Vector2 point)
        {
            return point.X >= x && point.X <= x + width && point.Y >= y && point.Y <= y + height;
        }

        public RectangleF Expand(float byVal)
        {
			RectangleF rect = this;
            rect.x -= byVal;
			rect.y -= byVal;
			rect.width += byVal * 2;
			rect.height += byVal * 2;

            return rect;
        }

        public RectangleF Expand(float byvalX, float byvalY)
        {
			RectangleF rect = this;
			rect.x -= byvalX;
			rect.y -= byvalY;
			rect.width += byvalX * 2;
			rect.height += byvalY * 2;

            return rect;
        }

		public RectangleF Shrink(float byVal)
		{
			RectangleF rect = this;
			rect.x += byVal;
			rect.y += byVal;
			rect.width -= byVal * 2;
			rect.height -= byVal * 2;

			return rect;
		}

		public RectangleF Shrink(float byvalX, float byvalY)
		{
			RectangleF rect = this;
			rect.x += byvalX;
			rect.y += byvalY;
			rect.width -= byvalX * 2;
			rect.height -= byvalY * 2;

			return rect;
		}

		public RectangleF Scale(float scale)
		{
			return new RectangleF(x * scale, y * scale, width * scale, height * scale);
		}

		public RectangleF Scale(float scaleX, float scaleY)
		{
			return new RectangleF(x * scaleX, y * scaleY, width * scaleX, height * scaleY);
		}

		public RectangleF Offset(float x, float y)
		{
			return new RectangleF(this.x + x, this.y + y, width, height);
		}

		public RectangleF Offset(Vector2 vector)
		{
			return Offset(vector.X, vector.Y);
		}

		public Vector2 Clamp(Vector2 vector)
		{
			return new Vector2(MathHelper.Clamp(vector.X, Left, Right), MathHelper.Clamp(vector.Y, Top, Bottom));
		}

		public static RectangleF Clamp(RectangleF inner, RectangleF larger)
		{
			Vector2 pos = inner.Position;

			if (inner.Position.X < larger.Position.X)
				pos.X = larger.Position.X;
			if (inner.Position.Y < larger.Position.Y)
				pos.Y = larger.Position.Y;

			if (inner.Right > larger.Right)
				pos.X = larger.Right - inner.width;
			if (inner.Bottom > larger.Bottom)
				pos.Y = larger.Bottom - inner.height;

			return new RectangleF(pos, inner.Size);
		}

		public static RectangleF FromTwoPoints(Vector2 A, Vector2 B)
		{
			Vector2 dist = B - A;
			return new RectangleF(A, dist);
		}

        public static Rectangle ToRectangle(RectangleF rectf)
        {
            return new Rectangle((int)rectf.x, (int)rectf.y, (int)rectf.width, (int)rectf.height);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        public override string ToString()
        {
            return "X:" + x + " Y:" + y + " W:" + width + " H:" + height;
        }

		public override bool Equals(object obj)
		{
			if (!(obj is RectangleF))
			{
				return false;
			}

			var f = (RectangleF)obj;
			return x == f.x &&
				   y == f.y &&
				   width == f.width &&
				   height == f.height;
		}

		public override int GetHashCode()
		{
			var hashCode = -1222528132;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			hashCode = hashCode * -1521134295 + width.GetHashCode();
			hashCode = hashCode * -1521134295 + height.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(RectangleF rectA, RectangleF rectB)
		{
			return rectA.Equals(rectB);
			//return rectA.x == rectB.x && rectA.y == rectB.y && rectA.width == rectB.width && rectA.height == rectB.height;
		}

		public static bool operator !=(RectangleF rectA, RectangleF rectB)
		{
			return !rectA.Equals(rectB);
			//return rectA.x != rectB.x && rectA.y != rectB.y && rectA.width != rectB.width && rectA.height != rectB.height;
		}
    }
}
