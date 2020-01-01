using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility
{
	public class RotatedRectangleF
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
				return RotatePoint(topLeft, topLeft + origin, rotation);
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
				return RotatePoint(topRight, topLeft + origin, rotation);
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
				return RotatePoint(bottomLeft, topLeft + origin, rotation);
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
				return RotatePoint(bottomRight, topLeft + origin, rotation);
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

		public RotatedRectangleF(RectangleF rectangle, float initialRotation, Vector2? origin = null)
		{
			this.baseRectangle = rectangle;
			this.rotation = initialRotation;

			if (origin == null)
				this.origin = new Vector2(rectangle.width / 2, rectangle.height / 2);
			else this.origin = origin.Value;
		}

		public RotatedRectangleF Offset(Vector2 offsetBy)
		{
			return new RotatedRectangleF(baseRectangle.Offset(offsetBy), rotation, origin);
		}

		public bool Intersects(RotatedRectangleF rectangle)
		{
			Vector2[] axes = new Vector2[4];

			axes[0] = TopRight - TopLeft;
			axes[1] = TopRight - BottomRight;
			axes[2] = rectangle.TopLeft - rectangle.BottomLeft;
			axes[3] = rectangle.TopLeft - rectangle.TopRight;

			foreach (Vector2 axis in axes)
			{
				if (!IsAxisColliding(rectangle, axis))
					return false;
			}

			return true;
		}

		private bool IsAxisColliding(RotatedRectangleF rectangle, Vector2 axis)
		{
			float[] thisScalars = new float[4];
			thisScalars[0] = Vector2.Dot(TopLeft, axis);
			thisScalars[1] = Vector2.Dot(TopRight, axis);
			thisScalars[2] = Vector2.Dot(BottomLeft, axis);
			thisScalars[3] = Vector2.Dot(BottomRight, axis);

			float[] otherScalars = new float[4];
			otherScalars[0] = Vector2.Dot(rectangle.TopLeft, axis);
			otherScalars[1] = Vector2.Dot(rectangle.TopRight, axis);
			otherScalars[2] = Vector2.Dot(rectangle.BottomLeft, axis);
			otherScalars[3] = Vector2.Dot(rectangle.BottomRight, axis);

			float thisRectMin = thisScalars.Min();
			float thisRectMax = thisScalars.Max();

			float otherRectMin = otherScalars.Min();
			float otherRectMax = otherScalars.Max();

			if (thisRectMin <= otherRectMax && thisRectMax >= otherRectMax)
				return true;

			if (otherRectMin <= thisRectMax && otherRectMax >= thisRectMax)
				return true;

			return false;
		}

		private Vector2 RotatePoint(Vector2 point, Vector2 origin, float rotation)
		{
			Vector2 rotated = Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
			return rotated;
		}
	}
}
