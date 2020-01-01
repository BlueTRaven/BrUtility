using BlueRavenUtility;
using BrNineSlice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrIMGUI.Content
{
	public class IMGuiTextPanelContent : IMGuiPanelContent
	{
		public readonly Vector2 padding;
		public readonly Enums.Alignment alignment;
		public readonly Color fontColor;
		public readonly bool wrap;

		/// <summary>
		/// A text and panel combination content.
		/// </summary>
		/// <param name="rectangle">Drawing bounds</param>
		/// <param name="padding">The text padding, i.e. how much space the text should be bordered by.</param>
		/// <param name="alignment">The location at which the text is anchored to.</param>
		public IMGuiTextPanelContent(RectangleF rectangle, NineSlice backgroundSlice, Color backgroundColor, Vector2 padding, 
			Enums.Alignment alignment, Color fontColor, bool wrap, float scale) : base(rectangle, backgroundSlice, backgroundColor, scale)
		{
			this.padding = padding;
			this.alignment = alignment;
			this.fontColor = fontColor;
			this.wrap = wrap;
		}
	}
}
