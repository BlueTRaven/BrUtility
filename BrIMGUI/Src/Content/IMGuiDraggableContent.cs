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
	public class IMGuiDraggableContent : IMGuiPanelContent
	{
		public RectangleF dragRect; //the bounds in which to check to see if the panel is draggable

		public string title;
		public TextHelper.FontInfo fontInfo;
		public Color fontColor;

		//A size is used to ensure that the draggable rectangle is never outside the bounds of the initial rectangle, barring, of course, a larger rectangle
		public IMGuiDraggableContent(RectangleF rectangle, Size dragRectSize, NineSlice nineSlice, Color backgroundColor, float scale) : 
			base(rectangle, nineSlice, backgroundColor, scale)
		{
			this.dragRect = new RectangleF(rectangle.Position, dragRectSize);
		}

		public IMGuiDraggableContent SetTitle(string title, Color fontColor, TextHelper.FontInfo fontInfo)
		{
			this.title = title;
			this.fontColor = fontColor;
			this.fontInfo = fontInfo;

			return this;
		}

		public IMGuiPanelContent GetBGPanelContent()
		{
			return new IMGuiPanelContent(new RectangleF(rectangle.x, rectangle.y + dragRect.height, rectangle.width, rectangle.height - dragRect.height), 
				backgroundSlice, backgroundColor, scale).SetColors(hotColor, hotColor) as IMGuiPanelContent;
		}

		public IMGuiPanelContent GetDraggablePanelContent()
		{
			return new IMGuiPanelContent(dragRect, backgroundSlice, backgroundColor, scale).SetColors(hotColor, activeColor) as IMGuiPanelContent;
		}
	}
}
