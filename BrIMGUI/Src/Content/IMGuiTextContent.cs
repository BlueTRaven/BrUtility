using BrUtility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrIMGUI.Content
{
	public class IMGuiTextContent : IMGuiContent
	{
		public Vector2 padding;
		public Enums.Alignment alignment;
		public Color fontColor;
		public bool wrap;

		public IMGuiTextContent(RectangleF rectangle, Vector2 padding, Enums.Alignment alignment, Color fontColor, bool wrap) : base(rectangle)
		{
			this.padding = padding;
			this.alignment = alignment;
			this.fontColor = fontColor;
			this.wrap = wrap;
		}
	}
}
