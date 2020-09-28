using BrUtility;
using BrIMGUI.Content;
using BrNineSlice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrIMGUI
{
	public static class IMGUIGeneric
	{
		public enum ButtonSwapType
		{
			SwapNineSlice,
			SwapColor
		}

		public static Color? LabelColor { get; set; }

		public static Enums.Alignment LabelTextAlignment { get; set; }
		public static Enums.Alignment ButtonTextAlignment { get; set; }

		public static ButtonSwapType _ButtonSwapType { get; private set; }

		public static NineSlice ButtonNineslice { get; private set; }
		public static NineSlice ButtonHotNineSlice { get; private set; }
		public static NineSlice ButtonActiveNineSlice { get; private set; }

		public static Color ButtonColor { get; private set; }
		public static Color ButtonHotColor { get; private set; }
		public static Color ButtonActiveColor { get; private set; }

		public static float ButtonScale { get; set; }

		public static NineSlice BackgroundNineslice { get; set; }

		public static float BackgroundScale { get; set; }

		public static TextHelper.FontInfo LabelFont { get; set; }
		public static TextHelper.FontInfo ButtonFont { get; set; }
		public static TextHelper.FontInfo TextboxFont { get; set; }

		public static float TextureScale { get; set; }

		static IMGUIGeneric()
		{
			ButtonScale = 1;
			BackgroundScale = 1;
			LabelColor = Color.Black;

			LabelTextAlignment = Enums.Alignment.Left;
			ButtonTextAlignment = Enums.Alignment.Center;

			TextureScale = 1;
		}

		public static void RegisterButtonNineslice(NineSlice nineslice, Color color, ButtonSwapType swapType = ButtonSwapType.SwapColor,
			NineSlice hotSlice = null, NineSlice activeSlice = null, Color? hotColor = null, Color? activeColor = null)
		{
			ButtonNineslice = nineslice;
			ButtonColor = color;

			_ButtonSwapType = swapType;

			ButtonHotColor = hotColor.GetValueOrDefault();
			ButtonActiveColor = activeColor.GetValueOrDefault();

			ButtonHotNineSlice = hotSlice;
			ButtonActiveNineSlice = activeSlice;
		}

		public static void RegisterBackgroundNineSlice(NineSlice nineslice)
		{
			BackgroundNineslice = nineslice;
		}

		public static void RegisterLabelFont(SpriteFont font)
		{
			LabelFont = new TextHelper.FontInfo(font, 1, false);
		}

		public static void RegisterButtonFont(SpriteFont font)
		{
			ButtonFont = new TextHelper.FontInfo(font, 1, false);
		}

		public static void RegisterTextboxFont(SpriteFont font)
		{
			TextboxFont = new TextHelper.FontInfo(font, 1, false);
		}

		public static Vector2 DrawGenericDraggable(RectangleF rect, string title)
		{
			if (BackgroundNineslice == null)
			{
				NoRegisterError("BackgroundNineSlice");
				return Vector2.Zero;
			}

			if (!LabelColor.HasValue)
			{
				NoRegisterError("LabelColor");
				return Vector2.Zero;
			}

			Vector2 dragPos = IMGUI.StartDraggable(new IMGuiDraggableContent(rect, new Size(rect.width, 24), BackgroundNineslice, Color.White, 1)
				.SetTitle(title, LabelColor.GetValueOrDefault(), LabelFont), true);

			return dragPos;
		}

		public static Vector2 DrawGenericDraggableWithClose(RectangleF rect, string title, ref bool status)
		{
			if (BackgroundNineslice == null)
			{
				NoRegisterError("BackgroundNineSlice");
				return Vector2.Zero;
			}

			if (!LabelColor.HasValue)
			{
				NoRegisterError("LabelColor");
				return Vector2.Zero;
			}

			Vector2 dragPos = IMGUI.StartDraggable(new IMGuiDraggableContent(rect, new Size(rect.width, 24), BackgroundNineslice, Color.White, 1)
				.SetTitle(title, LabelColor.GetValueOrDefault(), LabelFont), true);

			if (DrawGenericLabeledButton(new RectangleF(rect.width, -24, 24, 24), "X"))
				status = !status;

			return dragPos;
		}

		public static void DrawGenericLabel(RectangleF bounds, string text)
		{
			if (LabelFont.font == null)
			{
				NoRegisterError("LabelFont");
				return;
			}

			if (!LabelColor.HasValue)
			{
				NoRegisterError("LabelColor");
				return;
			}

			IMGUI.Label(new IMGuiTextContent(bounds, Vector2.Zero, LabelTextAlignment, LabelColor.GetValueOrDefault(), true), LabelFont, text);
		}

		public static void DrawGenericEnumButton(RectangleF bounds, List<string> text, ref int index)
		{
			if (DrawGenericLabeledButton(bounds, text[index]))
				index++;
			index %= text.Count;
		}

		public static void DrawGenericLabelEnumButtonPair(RectangleF bounds, float labelWidth, string labelText, List<string> names, ref int index)
		{
			DrawGenericLabel(new RectangleF(bounds.Position, labelWidth, bounds.height), labelText);
			DrawGenericEnumButton(new RectangleF(bounds.Position.X + labelWidth, bounds.Position.Y, bounds.width - labelWidth, bounds.height), names, ref index);
		}

		public static bool DrawGenericTextureButton(RectangleF bounds, Texture2D texture, Vector2 origin)
		{
			if (ButtonNineslice == null)
			{
				NoRegisterError("ButtonNineSlice");
				return false;
			}

			if (!LabelColor.HasValue)
			{
				NoRegisterError("LabelColor");
				return false;
			}

			bool ret = IMGUI.Button(GetPanelContent(bounds, ButtonScale));
			IMGUI.Texture(new IMGuiContent(bounds.Offset(-origin)), texture, TextureScale);
			return ret;
		}

		public static bool DrawGenericLabeledButton(RectangleF bounds, string text)
		{
			if (ButtonNineslice == null)
			{
				NoRegisterError("ButtonNineSlice");
				return false;
			}

			if (ButtonFont.font == null)
			{
				NoRegisterError("ButtonFont");
				return false;
			}

			if (!LabelColor.HasValue)
			{
				NoRegisterError("LabelColor");
				return false;
			}

			return IMGUI.TextButton(GetTextPanelContent(bounds, ButtonScale), ButtonFont, text);
		}

		public static bool DrawGenericLabelButtonPair(RectangleF bounds, float labelWidth, string labelText, string buttonText)
		{
			DrawGenericLabel(new RectangleF(bounds.Position, labelWidth, bounds.height), labelText);
			return DrawGenericLabeledButton(new RectangleF(bounds.Position.X + labelWidth, bounds.Position.Y, bounds.width - labelWidth, bounds.height), buttonText);
		}

		public static IMGUI.TextboxFlags DrawGenericTextbox(RectangleF bounds, ref string text, IMGUI.TextboxType type)
		{
			if (ButtonNineslice == null)
			{
				NoRegisterError("ButtonNineSlice");
				return IMGUI.TextboxFlags.None;
			}

			if (TextboxFont.font == null)
			{
				NoRegisterError("TextboxFont");
				return IMGUI.TextboxFlags.None;
			}

			if (!LabelColor.HasValue)
			{
				NoRegisterError("LabelColor");
				return IMGUI.TextboxFlags.None;
			}

			return IMGUI.Textbox(GetTextPanelContent(bounds, ButtonScale), TextboxFont, type, ref text);
		}

		public static IMGUI.TextboxFlags DrawGenericLabelTextboxPair(RectangleF bounds, float labelWidth, string labelText, ref string textboxText, IMGUI.TextboxType type)
		{
			DrawGenericLabel(new RectangleF(bounds.Position, labelWidth, bounds.height), labelText);
			return DrawGenericTextbox(new RectangleF(bounds.Position.X + labelWidth, bounds.Position.Y, bounds.width - labelWidth, bounds.height), ref textboxText, type);
		}

		private static void NoRegisterError(string registerName)
		{
			string str = "Tried to run IMGUIGeneric function without " + registerName + " registered.";
			Logger.GetOrCreate("BrUtility").Log(Logger.LogLevel.Caution, str);

#if DEBUG
			throw new Exception(str);
#endif
		}

		private static IMGuiPanelContent GetPanelContent(RectangleF bounds, float scale)
		{
			//Original colors: hot - new Color(239, 239, 239), active - new Color(191, 191, 191)
			IMGuiPanelContent panelContent = new IMGuiPanelContent(bounds, ButtonNineslice, ButtonColor, scale);
			if (_ButtonSwapType.Has(ButtonSwapType.SwapColor))
				panelContent.SetColors(ButtonHotColor, ButtonActiveColor);
			if (_ButtonSwapType.Has(ButtonSwapType.SwapNineSlice))
				panelContent.SetColors(ButtonHotColor, ButtonHotNineSlice, ButtonActiveColor, ButtonActiveNineSlice, 1);

			return panelContent;
		}

		private static IMGuiTextPanelContent GetTextPanelContent(RectangleF bounds, float scale)
		{
			//Original colors: hot - new Color(239, 239, 239), active - new Color(191, 191, 191)
			IMGuiTextPanelContent textPanelContent = new IMGuiTextPanelContent(bounds, ButtonNineslice, ButtonColor, Vector2.Zero, ButtonTextAlignment, LabelColor.GetValueOrDefault(), false, scale);
			if (_ButtonSwapType.Has(ButtonSwapType.SwapColor))
				textPanelContent.SetColors(ButtonHotColor, ButtonActiveColor);
			if (_ButtonSwapType.Has(ButtonSwapType.SwapNineSlice))
				textPanelContent.SetColors(ButtonHotColor, ButtonHotNineSlice, ButtonActiveColor, ButtonActiveNineSlice, 1);

			return textPanelContent;
		}
	}
}
