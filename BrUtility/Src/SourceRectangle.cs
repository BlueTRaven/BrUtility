using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility
{
	public struct SourceRectangle
	{
		public RectangleF bounds;
		public Vector2 origin;
		
		public SourceRectangle(Rectangle bounds, Vector2 origin)
		{
			this.bounds = bounds.ToRectangleF();
			this.origin = origin;
		}

		public SourceRectangle(RectangleF bounds, Vector2 origin)
		{
			this.bounds = bounds;
			this.origin = origin;
		}

		public SourceRectangle SetOriginByPercent(float x, float y)
		{
			return new SourceRectangle(bounds, new Vector2(x * bounds.width, y * bounds.height));
		}

		public static implicit operator RectangleF(SourceRectangle sr)
		{
			return sr.bounds;
		}

		public static implicit operator Rectangle(SourceRectangle sr)
		{
			return sr.bounds.ToRectangle();
		}
	}
}
