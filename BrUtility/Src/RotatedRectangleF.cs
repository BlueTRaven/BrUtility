using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public struct RotatedRectangleF
	{
		public RectangleF baseRectangle;
		public float rotation;

		public Vector2 origin;

		[JsonIgnore]
		public Vector2 TopLeft
		{
			get
			{
				Vector2 topLeft = new Vector2(baseRectangle.Left, baseRectangle.Top);
				RotatePoint(ref topLeft, topLeft + origin, rotation);
				return topLeft;
			}
		}

		[JsonIgnore]
		public Vector2 TopLeftNoRot
		{
			get
			{
				return new Vector2(baseRectangle.Left, baseRectangle.Top);
			}
		}

		[JsonIgnore]
		public Vector2 TopRight
		{
			get
			{
				Vector2 topLeft = new Vector2(baseRectangle.Left, baseRectangle.Top);
				Vector2 topRight = new Vector2(baseRectangle.Right, baseRectangle.Top);
				RotatePoint(ref topRight, topLeft + origin, rotation);
				return topRight;
			}
		}

		[JsonIgnore]
		public Vector2 TopRightNoRot
		{
			get
			{
				return new Vector2(baseRectangle.Right, baseRectangle.Top);
			}
		}

		[JsonIgnore]
		public Vector2 BottomLeft
		{
			get
			{
				Vector2 topLeft = new Vector2(baseRectangle.Left, baseRectangle.Top);
				Vector2 bottomLeft = new Vector2(baseRectangle.Left, baseRectangle.Bottom);
				RotatePoint(ref bottomLeft, topLeft + origin, rotation);
				return bottomLeft;
			}
		}

		[JsonIgnore]
		public Vector2 BottomLeftNoRot
		{
			get
			{
				return new Vector2(baseRectangle.Left, baseRectangle.Bottom);
			}
		}

		[JsonIgnore]
		public Vector2 BottomRight
		{
			get
			{
				Vector2 topLeft = new Vector2(baseRectangle.Left, baseRectangle.Top);
				Vector2 bottomRight = new Vector2(baseRectangle.Right, baseRectangle.Bottom);
				RotatePoint(ref bottomRight, topLeft + origin, rotation);
				return bottomRight;
			}
		}

		[JsonIgnore]
		public Vector2 BottomRightNoRot
		{
			get
			{
				return new Vector2(baseRectangle.Right, baseRectangle.Bottom);
			}
		}

		[JsonIgnore]
		public Vector2 Center { get { return baseRectangle.Center; } }

		[JsonIgnore]
		public RectangleF BoundaryRectangle
		{
			get
			{
				float minX = MathHelper.Min(MathHelper.Min(TopLeft.X, TopRight.X), MathHelper.Min(BottomLeft.X, BottomRight.X));
				float minY = MathHelper.Min(MathHelper.Min(TopLeft.Y, TopRight.Y), MathHelper.Min(BottomLeft.Y, BottomRight.Y));
				float maxX = MathHelper.Max(MathHelper.Max(TopLeft.X, TopRight.X), MathHelper.Max(BottomLeft.X, BottomRight.X));
				float maxY = MathHelper.Max(MathHelper.Max(TopLeft.Y, TopRight.Y), MathHelper.Max(BottomLeft.Y, BottomRight.Y));

				float width = maxX - minX;
				float height = maxY - minY;
				return new RectangleF(minX, minY, width, height);
			}
		}

		public RotatedRectangleF(RectangleF rectangle, float initialRotation, Vector2? origin = null)
		{
			this.baseRectangle = rectangle;
			this.rotation = initialRotation;

			if (origin == null)
				this.origin = new Vector2(rectangle.width / 2, rectangle.height / 2);
			else this.origin = origin.Value;

			//TODO: can we get rid of this allocation entirely? Array.Min/Max is way too convenient...
			thisScalars = new float[4];
			otherScalars = new float[4];
		}

		public RotatedRectangleF Offset(Vector2 offsetBy)
		{
			return new RotatedRectangleF(baseRectangle.Offset(offsetBy), rotation, origin);
		}

		public bool Intersects(RotatedRectangleF rectangle)
		{
			Vector2 axisTop = TopRight - TopLeft;
			Vector2 axisRight = TopRight - BottomRight;
			Vector2 axisOTop = rectangle.TopRight - rectangle.TopLeft;
			Vector2 axisORight = rectangle.TopRight - rectangle.BottomRight;

			if (!IsAxisColliding(rectangle, axisTop) ||
				!IsAxisColliding(rectangle, axisRight) ||
				!rectangle.IsAxisColliding(this, axisOTop) ||
				!rectangle.IsAxisColliding(this, axisORight))
				return false;
			else return true;
		}

		private float[] thisScalars;
		private float[] otherScalars;
		public bool IsAxisColliding(RotatedRectangleF rectangle, Vector2 axis)
		{
			thisScalars[0] = Vector2.Dot(TopLeft, axis);
			thisScalars[1] = Vector2.Dot(TopRight, axis);
			thisScalars[2] = Vector2.Dot(BottomLeft, axis);
			thisScalars[3] = Vector2.Dot(BottomRight, axis);

			otherScalars[0] = Vector2.Dot(rectangle.TopLeft, axis);
			otherScalars[1] = Vector2.Dot(rectangle.TopRight, axis);
			otherScalars[2] = Vector2.Dot(rectangle.BottomLeft, axis);
			otherScalars[3] = Vector2.Dot(rectangle.BottomRight, axis);

			float thisRectMin = thisScalars.Min();
			float thisRectMax = thisScalars.Max();

			float otherRectMin = otherScalars.Min();
			float otherRectMax = otherScalars.Max();

			if (thisRectMax >= otherRectMax && thisRectMin <= otherRectMax)
				return true;

			if (otherRectMax >= thisRectMax && otherRectMin <= thisRectMax)
				return true;

			return false;
		}

		private void RotatePoint(ref Vector2 point, in Vector2 origin, in float rotation)
		{
			point = Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
		}
	}
}
