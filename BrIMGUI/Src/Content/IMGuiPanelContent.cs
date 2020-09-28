using BrUtility;
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
	public class IMGuiPanelContent : IMGuiContent
	{
		public readonly Color backgroundColor;
		public readonly NineSlice backgroundSlice;
		public readonly float scale;

		public Color hotColor = Color.White;
		public NineSlice hotSlice;

		public Color activeColor = Color.White;
		public NineSlice activeSlice;

		public Color disabledColor = Color.Gray;
		public NineSlice disabledSlice;

		public IMGuiPanelContent(RectangleF rectangle, NineSlice backgroundSlice, Color backgroundColor, float scale) : base(rectangle)
		{
			this.backgroundSlice = backgroundSlice;
			this.backgroundColor = backgroundColor;
			this.scale = scale;
		}

		public IMGuiPanelContent SetColors(Color hotColor, Color activeColor, float alpha = 1)
		{
			this.hotColor = hotColor;
			this.activeColor = activeColor;

			this.hotColor = new Color(hotColor, alpha);
			this.activeColor = new Color(activeColor, alpha);

			return this;
		}

		public IMGuiPanelContent SetColors(Color hotColor, NineSlice hotSlice, Color activeColor, NineSlice activeSlice, float alpha)
		{
			this.hotColor = hotColor;
			this.hotSlice = hotSlice;
			this.hotColor = new Color(hotColor, alpha);

			this.activeColor = activeColor;
			this.activeSlice = activeSlice;
			this.activeColor = new Color(activeColor, alpha);

			return this;
		}
	}
}
