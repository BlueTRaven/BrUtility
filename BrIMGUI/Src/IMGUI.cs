using A1r.Input;
using BrUtility;
using BrIMGUI.Content;
using BrNineSlice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrIMGUI
{
	public static class IMGUI
	{
		public enum TextboxType
		{
			None = 0,
			/// <summary>
			/// All alphabetical characters.
			/// a-z.
			/// </summary>
			Alphabetical = 1 << 0,
			/// <summary>
			/// All numbers.
			/// 0-9.
			/// </summary>
			Numerical = 1 << 1,
			/// <summary>
			/// All special characters. 
			/// !@#$%^&*()_+-=[]{}\|;':",.<>/?`~
			/// </summary>
			Special = 1 << 2,
			/// <summary>
			/// Characters that integers use. Just '-' at the moment.
			/// </summary>
			Integer = Numerical | 1 << 3,
			/// <summary>
			/// Floating point precision characters. '.' and '-'.
			/// Overridden by special. Used for floats and doubles where we need decimal spaces and negatives.
			/// </summary>
			FloatingPrecision = Numerical | 1 << 4,
			/// <summary>
			/// Backspace can be pressed to delete characters.
			/// </summary>
			AllowsBackspace = 1 << 5,
			/// <summary>
			/// Enter can be pressed.
			/// </summary>
			AllowsEnter = 1 << 6,
			/// <summary>
			/// Enter exits the textbox.
			/// </summary>
			EnterExits = 1 << 7,
			/// <summary>
			/// Tab can be pressed.
			/// </summary>
			AllowsTab = 1 << 8,
			/// <summary>
			/// Allows all alphabetical and numerical characters.
			/// Does not add backspace, enter, and tab.
			/// </summary>
			AlphaNumerical = Alphabetical | Numerical,
			/// <summary>
			/// Allows all alphabetical, numerical, and special characters.
			/// Does not add backspace, enter, and tab.
			/// </summary>
			AllCharacters = Alphabetical | Numerical | Special,
		}

		public enum TextboxFlags
		{
			None = 0,				//nothing is happening with the textbox. It is not being selected nor is it currently selected.
			Finished = 1 << 0,		//The text box is 'finished.' It will stop recieving input. Called when enter is pressed if AllowsEnter is false.
			ToActive = 1 << 1,		//The text box was just clicked.
			Inactive = 1 << 2,		//same as None. Initially wanted this to be set when we change selection, but the issue is we don't know if we *were* active last frame...
			Active = 1 << 3,		//the text box is currently active
			Canceled = 1 << 4,		//Escape is pressed.
		}

		public static float foldoutIndent = 32;

		public static int timer;
		
		public static IMGUIId hot = IMGUIId.Inactive, clicked = IMGUIId.Inactive, active = IMGUIId.Inactive;

		internal static IMGUIId lastCreated = IMGUIId.Inactive;
		internal static IMGUIId parent = IMGUIId.Inactive;

		public static bool activeSetLast;
		public static bool hotSetThisFrame, activeSetThisFrame;

		public static float DrawDepth { get; set; }
		internal const float depthIncrease = 0.001f;

		private static Game game;
		private static SpriteBatch batch;
		private static InputManager inputManager;
		public static bool useOverrideCursorPosition;
		public static Vector2 overrideCursorPosition;
		public static bool inTextbox;

		private static List<IMGUIId> ids;
		private static List<IMGuiContent> contents;

		public static float scale = 1;

		private static bool started;

		private static bool canAny = true;
		private static bool canHot = true;
		public static bool CanHot { get { if (canAny) return canHot; else return false; } set => canHot = value; }
		private static bool canActive = true;
		public static bool CanActive { get { if (canAny) return canActive; else return false; } set => canActive = value; }
		private static bool canClick = true;
		public static bool CanClick { get { if (canAny) return canClick; else return false; } set => canClick = value; }

		public static event Action<IMGUIId> SetHotEvent, SetActiveEvent;

		public static void Start(Game _game, SpriteBatch _batch, InputManager _inputManager, float drawDepth = 0)
		{
			started = true;

			timer++;

			game = _game;
			batch = _batch;
			inputManager = _inputManager;

			activeSetLast = activeSetThisFrame;
			hotSetThisFrame = false;
			activeSetThisFrame = false;

			IMGUI.DrawDepth = drawDepth;

			lastCreated = IMGUIId.Inactive;
			parent = IMGUIId.Inactive;

			ids = new List<IMGUIId>();
			contents = new List<IMGuiContent>();

			clicked = IMGUIId.Inactive;
		}

		public static void New()
		{
			inTextbox = false;
			hot = IMGUIId.Inactive;
			clicked = IMGUIId.Inactive;
			active = IMGUIId.Inactive;
		}

		public static IMGUIId GetGuID(bool enabled = true, bool visible = true)
		{
			IMGUIId guid = new IMGUIId(ids.Count, parent.id);
			guid.enabled = enabled;
			guid.visible = visible;

			ids.Add(guid);
			contents.Add(null); //insert a null reference. this way, everything in both ids and contents aligns properly if, for whatever reason, a IMGUI function call doesn't add a content.

			lastCreated = guid;

			return guid;
		}

		public static void Label(IMGuiTextContent content, TextHelper.FontInfo fontTextInfo, string label)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			RectangleF drawRect = finalRect.Shrink(content.padding.X, content.padding.Y);

			drawRect = drawRect.Scale(scale);
			finalRect = finalRect.Scale(scale);

			//Labels can't really be hot or active; why would you want to click them, or perform special logic on them?
			//Don't really even need any special logic for enable/disable.

			TextHelper.DrawText(batch, fontTextInfo, label, content.fontColor, drawRect.ToRectangle(), content.alignment, content.wrap ? (int)drawRect.width : -1, DrawDepth += depthIncrease, TextHelper.OverFlowAction.None);
		}

		public static void Panel(IMGuiPanelContent content, bool canHot = false)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return;

			Color drawColor = content.backgroundColor;
			NineSlice drawSlice = content.backgroundSlice;
			//Texture2D drawTexture = content.backgroundTexture;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			finalRect = finalRect.Scale(scale);

			if (canHot)
			{
				bool hovered = IsHot(id, finalRect);
				if (hovered)
				{
					drawColor = content.hotColor;
					if (content.hotSlice != null)
						drawSlice = content.hotSlice;
				}
			}

			drawSlice.Draw(batch, drawColor, finalRect, content.scale, DrawDepth += depthIncrease);
		}

		public static bool TextButton(IMGuiTextPanelContent content, TextHelper.FontInfo fontTextInfo, string label, Vector2? hotTextOffset = null, Vector2? activeTextOffset = null)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return false;

			Color drawColor = content.backgroundColor;
			NineSlice drawSlice = content.backgroundSlice;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			RectangleF drawRect = finalRect.Shrink(content.padding.X, content.padding.Y);

			drawRect = drawRect.Scale(scale);
			finalRect = finalRect.Scale(scale);

			bool contains = false, clicked = false, justClicked = false;

			if (CheckGUIDEnabled(id))
			{
				contains = CheckSetHot(id, finalRect);
				justClicked = JustClicked(id, finalRect);
				clicked = CheckSetClicked(id, finalRect);

				if (contains)
				{
					if (hotTextOffset.HasValue)
						drawRect = drawRect.Offset(hotTextOffset.Value);

					drawColor = content.hotColor;
					if (content.hotSlice != null)
						drawSlice = content.hotSlice;
				}

				if (contains && clicked)
				{
					if (activeTextOffset.HasValue)
						drawRect = drawRect.Offset(activeTextOffset.Value);

					drawColor = content.activeColor;
					if (content.activeSlice != null)
						drawSlice = content.activeSlice;
				}
			}
			else drawColor = content.disabledColor;

			//content.backgroundSlice.Draw(batch, content.backgroundTexture, drawColor, finalRect, drawDepth += depthIncrease);
			drawSlice.Draw(batch, drawColor, finalRect, content.scale, DrawDepth += depthIncrease);

			TextHelper.DrawText(batch, fontTextInfo, label, content.fontColor, drawRect.ToRectangle(), content.alignment, content.wrap ? (int)drawRect.width : -1, DrawDepth += depthIncrease, TextHelper.OverFlowAction.None);

			return contains && justClicked;
		}

		/// <summary>
		/// Draw a button without anything in it.
		/// </summary>
		public static bool Button(IMGuiPanelContent content)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return false;

			Color drawColor = content.backgroundColor;
			NineSlice drawSlice = content.backgroundSlice;
			//Texture2D drawTexture = content.backgroundTexture;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			finalRect = finalRect.Scale(scale);

			bool contains = false, clicked = false, justClicked = false;

			if (CheckGUIDEnabled(id))
			{
				contains = CheckSetHot(id, finalRect);
				justClicked = JustClicked(id, finalRect);
				clicked = CheckSetClicked(id, finalRect);

				if (contains)
				{
					drawColor = content.hotColor;
					if (content.hotSlice != null)
						drawSlice = content.hotSlice;
				}

				if (contains && clicked)
				{
					drawColor = content.activeColor;
					if (content.activeSlice != null)
						drawSlice = content.activeSlice;
				}
			}
			else drawColor = content.disabledColor;

			drawSlice.Draw(batch, drawColor, finalRect, content.scale, DrawDepth += depthIncrease);
			
			return contains && justClicked;
		}

		public static void Texture(IMGuiContent content, Texture2D texture, float texScale)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			finalRect = finalRect.Scale(scale);

			batch.Draw(texture, finalRect.Center, null, Color.White, 0, texture.Bounds.Center.ToVector2() * texScale, texScale, SpriteEffects.None, DrawDepth += depthIncrease);
		}

		public static void Checkbox(IMGuiContent content, Texture2D uncheckedTexture, Texture2D checkedTexture, ref bool check)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return;

			Color drawColor = Color.White;
			if (content is IMGuiPanelContent panel)
				drawColor = panel.backgroundColor;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			if (CheckGUIDEnabled(id))
			{
				bool contains = CheckSetHot(id, finalRect);
				bool justClicked = JustClicked(id, finalRect);
				bool clicked = CheckSetClicked(id, finalRect);

				if (content is IMGuiPanelContent)
				{
					if (contains)
						drawColor = (content as IMGuiPanelContent).hotColor;

					if (contains && clicked)
					{
						if (justClicked)
							check = !check;

						drawColor = (content as IMGuiPanelContent).activeColor;
					}
				}
				else
				{
					if (contains)
						SetHot(id);

					if (contains && clicked)
					{
						SetClicked(id);

						if (justClicked)
							check = !check;
					}
				}
			}

			if (content is IMGuiPanelContent panelContent)
				panelContent.backgroundSlice.Draw(batch, drawColor, finalRect, panelContent.scale, DrawDepth += depthIncrease);

			if (check)
				batch.Draw(checkedTexture, finalRect.Center, null, Color.White, 0, checkedTexture.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, DrawDepth += depthIncrease);
			else
			{
				if (uncheckedTexture != null)
					batch.Draw(uncheckedTexture, finalRect.Center, null, Color.White, 0, uncheckedTexture.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, DrawDepth += depthIncrease);
			}
		}

		public static TextboxFlags Textbox(IMGuiTextPanelContent content, TextHelper.FontInfo fontTextInfo, TextboxType type, ref string text, bool canFocus = true, bool forceFocus = false)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return TextboxFlags.None;

			TextboxFlags flags = TextboxFlags.None;

			Color drawColor = content.backgroundColor;
			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			RectangleF drawRect = finalRect.Shrink(content.padding.X, content.padding.Y);

			drawRect = drawRect.Scale(scale);
			finalRect = finalRect.Scale(scale);

			bool justClicked = JustClicked(id, content.rectangle);

			if (justClicked)
			{
				flags |= TextboxFlags.ToActive;
				inTextbox = true;
			}

			if (CheckGUIDEnabled(id))
			{
				bool contains = CheckSetHot(id, finalRect);
				bool clicked = CheckSetClicked(id, finalRect);

				if (active.id != id.id)
				{
					flags |= TextboxFlags.Inactive;
					inTextbox = false;
				}
				else
				{
					flags |= TextboxFlags.Active;
					inTextbox = true;
				}

				if (contains || active.id == id.id)
					drawColor = content.hotColor;

				if ((contains && clicked && canFocus) || forceFocus)
				{
					timer = 0;

					drawColor = content.activeColor;

					flags |= TextboxFlags.ToActive;
					inTextbox = true;
				}

				if (!canFocus)
					New();	//stop focusing anything
			}
			else drawColor = content.disabledColor;

			content.backgroundSlice.Draw(batch, drawColor, finalRect, content.scale, DrawDepth += depthIncrease);

			TextHelper.DrawText(batch, fontTextInfo, text, content.fontColor, drawRect.ToRectangle(), content.alignment, content.wrap ? (int)finalRect.width : -1, DrawDepth += depthIncrease);
			
			if (active.id == id.id)
			{
				bool processKeys = true;

				if ((inputManager.IsHeld(Keys.LeftControl) && inputManager.JustPressed(Keys.V)) || (inputManager.JustPressed(Keys.LeftControl) && inputManager.IsHeld(Keys.V)))
				{
					//handle pasting
					//TODO fix pasting - import System.Windows.Forms.Clipboard and use GetText()
					string clipboardText = "";//System.Windows.Forms.Clipboard.GetText();

					if (IsTextAllowedInTextbox(type, clipboardText))
						text += clipboardText;

					//don't process keys, otherwise we'd get a v in the textbox.
					processKeys = false;
				}

				if (processKeys)
				{
					Keys[] keys = inputManager.currentKeyboardState.GetPressedKeys();

					for (int i = 0; i < keys.Length; i++)
					{
						Keys key = keys[i];

						if (inputManager.JustPressed(key))
						{
							bool final = InterpretPressedKey(type, key, ref text);
							if (final && key == Keys.Enter)
								flags |= TextboxFlags.Finished;
							else if (final && key == Keys.Escape)
								flags |= TextboxFlags.Canceled;

							if (final)
							{
								New();
								break;
							}
						}

						if (inputManager.IsRepeated(key))
							InterpretPressedKey(type, key, ref text);
					}
				}

				if (timer % 60 < 30)
				{
					Vector2 position = TextHelper.GetLastCharacterPosition(fontTextInfo, text, finalRect.ToRectangle(), content.alignment) + new Vector2(8, 0);
					batch.DrawRectangle(new RectangleF(position, 16, fontTextInfo.font.LineSpacing), content.fontColor, DrawDepth += depthIncrease);
				}
			}

			return flags;
		}

		public static void Separator(IMGuiContent content, float indent)
		{
			IMGUIId id = GetGuID();

			CheckFunctionValid(id, content);

			if (!CheckGUIDVisible(id))
				return;

			RectangleF finalRect = content.rectangle;

			if (id.parent != -1)
				finalRect = finalRect.Offset(GetRectOffset(id, content));

			batch.DrawRectangle(finalRect.Shrink(indent, 0), Color.White, DrawDepth += depthIncrease);
		}

		public static RectangleF GetAnchorByAlignmentToParent(RectangleF rect, Enums.Alignment alignment)
		{
			if (parent.id != -1)
			{
				RectangleF bounds = contents[parent.id].rectangle;
				Vector2 size = rect.Size.ToVector2();

				if (alignment == Enums.Alignment.TopLeft)
					return rect;
				else if (alignment == Enums.Alignment.Left)
					return new RectangleF(new Vector2(0, (bounds.height / 2) - (size.Y / 2)), rect.Size);
				else if (alignment == Enums.Alignment.BottomLeft)
					return new RectangleF(new Vector2(0, bounds.height - size.Y), rect.Size);
				else if (alignment == Enums.Alignment.Bottom)
					return new RectangleF(new Vector2((bounds.width / 2) - (size.X / 2), bounds.height - size.Y), rect.Size);
				else if (alignment == Enums.Alignment.BottomRight)
					return new RectangleF(new Vector2(bounds.width - size.X, bounds.height - size.Y), rect.Size);
				else if (alignment == Enums.Alignment.Right)
					return new RectangleF(new Vector2(bounds.width - size.X, (bounds.height / 2) - (size.Y / 2)), rect.Size);
				else if (alignment == Enums.Alignment.TopRight)
					return new RectangleF(new Vector2(bounds.width - size.X, 0), rect.Size);
				else if (alignment == Enums.Alignment.Top)
					return new RectangleF(new Vector2((bounds.width / 2) - (size.X / 2), 0), rect.Size);
				else if (alignment == Enums.Alignment.Center)
					return new RectangleF(new Vector2((bounds.width / 2) - (size.X / 2), (bounds.height / 2) - (size.Y / 2)), rect.Size);
				else return rect;
			}
			else return rect;
		}

		//TODO broken
		public static float GetNextVertical()
		{
			if (lastCreated.id == -1)
				return 0;

			if (contents[lastCreated.id] != null)
				return contents[lastCreated.id].rectangle.height;
			else return 0;
		}

		/// <summary>
		/// Recursively get the total offset of the given id - where the position of the rectangle should be accounting for all the parent's positions
		/// </summary>
		internal static Vector2 GetRectOffset(IMGUIId id, IMGuiContent startContent)
		{
			int parentID = id.parent;

			Vector2 finalOffset = Vector2.Zero;

			while(parentID != -1)
			{
				finalOffset += contents[parentID].rectangle.Position;
				parentID = ids[parentID].parent;
			}

			return finalOffset;
		}

		internal static bool InterpretPressedKey(TextboxType type, Keys pressedKey, ref string text)
		{
			bool dontProcessNumber = false;
			bool caps = inputManager.IsPressed(Keys.LeftShift) || inputManager.IsPressed(Keys.RightShift) || inputManager.currentKeyboardState.CapsLock;

			if ((type & TextboxType.Special) == TextboxType.Special)
			{
				switch (pressedKey)
				{
					case Keys.OemPipe:
						if (caps)
							text += '|';
						else text += '\\';
						break;
					case Keys.OemPlus:
						if (caps)
							text += '+';
						else text += '=';
						break;
					case Keys.OemMinus:
						if (caps)
							text += '_';
						else text += '-';
						break;
					case Keys.OemQuestion:
						if (caps)
							text += '?';
						else text += '/';
						break;
					case Keys.OemQuotes:
						if (caps)
							text += '"';
						else text += '\'';
						break;
					case Keys.OemSemicolon:
						if (caps)
							text += ':';
						else text += ';';
						break;
					case Keys.OemTilde:
						if (caps)
							text += '~';
						else text += '`';
						break;
					case Keys.D0:
						if (caps)
							text += ')';
						break;
					case Keys.D1:
						if (caps)
						{ text += '!'; dontProcessNumber = true; }
						break;
					case Keys.D2:
						if (caps)
						{ text += '@'; dontProcessNumber = true; }
						break;
					case Keys.D3:
						if (caps)
						{ text += '#'; dontProcessNumber = true; }
						break;
					case Keys.D4:
						if (caps)
						{ text += '$'; dontProcessNumber = true; }
						break;
					case Keys.D5:
						if (caps)
						{ text += '%'; dontProcessNumber = true; }
						break;
					case Keys.D6:
						if (caps)
						{ text += '^'; dontProcessNumber = true; }
						break;
					case Keys.D7:
						if (caps)
						{ text += '&'; dontProcessNumber = true; }
						break;
					case Keys.D8:
						if (caps)
						{ text += '*'; dontProcessNumber = true; }
					break;
					case Keys.D9:
						if (caps)
						{ text += '('; dontProcessNumber = true; }
						break;
				}
			}

			if (!dontProcessNumber && (type & TextboxType.Numerical) == TextboxType.Numerical && (int)pressedKey >= (int)Keys.D0 && (int)pressedKey <= (int)Keys.D9)
			{
				char character = ((int)pressedKey - (int)Keys.D0).ToString()[0];

				text += character;
			}

			if ((type & TextboxType.Alphabetical) == TextboxType.Alphabetical && (int)pressedKey >= 65 && (int)pressedKey <= 90)
			{   //a-z keys
				char character = pressedKey.ToString()[0];

				if (!caps)
					character = Char.ToLower(character);
				text += character;
			}

			if (((type & TextboxType.FloatingPrecision) == TextboxType.FloatingPrecision || (type & TextboxType.Integer) == TextboxType.Integer || (type & TextboxType.Special) == TextboxType.Special) &&
				inputManager.IsPressed(Keys.OemPeriod))
				text += '.';

			if (((type & TextboxType.FloatingPrecision) == TextboxType.FloatingPrecision || (type & TextboxType.Integer) == TextboxType.Integer) &&
				inputManager.IsPressed(Keys.OemMinus))
				text += '-';

			if (pressedKey == Keys.Space)
				text += ' ';
			
			if ((type & TextboxType.AllowsTab) == TextboxType.AllowsTab)
			{	//TODO fix tab (font.MeasureString doesn't like it, throws exception, stops processing the *entire* string)
				 //text += '\t';
			}

			if ((type & TextboxType.AllowsEnter) == TextboxType.AllowsEnter && pressedKey == Keys.Enter)
				text += Environment.NewLine;
			else if (pressedKey == Keys.Enter)
				return true;

			if ((type & TextboxType.AllowsBackspace) == TextboxType.AllowsBackspace && pressedKey == Keys.Back)
			{
				if (text.Length > 0)
				{
					if (inputManager.IsHeld(Keys.LeftControl) || inputManager.IsHeld(Keys.RightControl))
					{
						text = text.Substring(0, text.Length - 1);
						char c = text[text.Length - 1];

						while (text.Length > 0 && c != ' ')
						{
							text = text.Substring(0, text.Length - 1);

							if (text.Length <= 0)
								break;
							else c = text[text.Length - 1];
						}
					}
					else text = text.Substring(0, text.Length - 1);
				}
			}

			if (pressedKey == Keys.Escape)
			{
				active = IMGUIId.Inactive;
				return true;
			}

			return false;
		}

		internal static bool IsTextAllowedInTextbox(TextboxType type, string text)
		{
			if ((type & TextboxType.FloatingPrecision) == TextboxType.FloatingPrecision)
			{
				foreach (char c in text)
				{
					if (!char.IsNumber(c) || c != '.' || c != '-')
						return false;
				}
			}

			if ((type & TextboxType.Integer) == TextboxType.Integer)
			{
				foreach (char c in text)
				{
					if (!char.IsNumber(c) || c != '-')
						return false;
				}
			}

			return true;
		}

		internal static void CheckFunctionValid(IMGUIId guid, IMGuiContent content)
		{
			if (!started)
				throw new InvalidOperationException("IMGUI.Start has not been called!");

			if (content != null)
				contents[guid.id] = content;
			else throw new NullReferenceException("Must pass valid IMGuiContent object.");
		}

		internal static bool CheckGUIDEnabled(IMGUIId id, bool isParent = false)
		{
			if (isParent)
				return id.enabled;

			if (!parent.valid)
				return id.enabled;
			else return id.enabled && CheckGUIDEnabled(parent, true);
		}

		internal static bool CheckGUIDVisible(IMGUIId id, bool isParent = false)
		{
			if (isParent)
				return id.visible;

			if (!parent.valid)
				return id.visible;
			else return id.visible && CheckGUIDVisible(parent, true);
		}

		#region Parenters
		public static Vector2 StartDraggable(IMGuiDraggableContent content, bool canDrag)
		{
			DrawDepth += depthIncrease;

			parent = GetGuID();

			CheckFunctionValid(parent, content);

			IMGuiPanelContent bgPanelContent = content.GetBGPanelContent();
			IMGuiPanelContent dragPanelContent = content.GetDraggablePanelContent();

			int panelID = ids.Count;
			Panel(bgPanelContent, false);

			Vector2 finalPosition = content.rectangle.Position;

			int buttonID = ids.Count;
			if ((Button(dragPanelContent) || (clicked.id == buttonID && inputManager.IsPressed(MouseInput.LeftButton))) && canDrag)
			{   //we are dragging
				float amt = 1f / scale;
				finalPosition += inputManager.GetMouseDelta() / 2; //divide by two for some reason???
				//TODO really need to figure out why this is. There's definitely some inconsistency between coordinate systems for some reason.
			}

			if (content.title != null && content.title != "")
				Label(new IMGuiTextContent(dragPanelContent.rectangle, Vector2.Zero, Enums.Alignment.Center, content.fontColor, true), content.fontInfo, content.title);

			parent = ids[panelID];  //everything else will be parented to the inner pannel so that we aren't using the title bar as workable space
			parent.skipParent = true;   //skip this inner panel so when we exit the parenting we don't go back to the panel's parent, which is the draggable (thus, throwing things off)

			ids[panelID] = parent;

			return finalPosition;
		}

		//TODO broken
		public static void StartVertical(Vector2 position)
		{
			DrawDepth += depthIncrease;

			parent = GetGuID();

			lastCreated = IMGUIId.Inactive;

			CheckFunctionValid(parent, new IMGuiContent(new RectangleF(position, Size.Zero)));
		}

		public static void StartFoldout(IMGuiContent content, bool opened)
		{
			DrawDepth += depthIncrease;

			parent = GetGuID();

			parent.enabled = opened;
			parent.visible = opened;

			CheckFunctionValid(parent, new IMGuiContent(new RectangleF(content.rectangle.x + foldoutIndent, content.rectangle.y + 32, content.rectangle.width, content.rectangle.height)));
		}

		public static void StartDisable(IMGuiContent content)
		{
			DrawDepth += depthIncrease;

			parent = GetGuID();
			parent.enabled = false;

			CheckFunctionValid(parent, content);
		}

		public static void EndParent(bool recursive)
		{
			if (parent.id != -1)
			{
				DrawDepth -= depthIncrease;

				bool skipParent = parent.skipParent;

				if (parent.parent != -1)
					parent = ids[parent.parent];
				else parent = IMGUIId.Inactive;

				if (recursive)
				{
					while (skipParent)
					{
						skipParent = parent.skipParent;

						if (parent.parent != -1)
							parent = ids[parent.parent];
						else parent = IMGUIId.Inactive;
					}
				}
			}
		}
		#endregion

		#region Get/Set hot/active guids
		internal static bool CheckSetHot(IMGUIId id, RectangleF rect)
		{
			bool isHot = IsHot(id, rect);

			if (isHot)
				SetHot(id);
			else if (hot.id == id.id)
				hot = IMGUIId.Inactive;

			return isHot;
		}

		internal static bool CheckSetClicked(IMGUIId id, RectangleF rect)
		{
			bool isActive = IsActive(id, rect);
			bool justActive = JustClicked(id, rect);

			if (justActive)
				SetActiveEvent?.Invoke(id);

			if (isActive)
				SetClicked(id);
			else if (clicked.id == id.id)
				clicked = IMGUIId.Inactive;

			return isActive;
		}

		internal static void SetHot(IMGUIId id)
		{
			SetHotEvent?.Invoke(id);

			hot = id;
			hotSetThisFrame = true;
		}

		internal static void SetClicked(IMGUIId id)
		{
			clicked = id;
			active =  id;
			activeSetThisFrame = true;
		}

		internal static bool IsHot(IMGUIId id, RectangleF rect)
		{
			return game.IsActive && CanHot && rect.Contains(GetCursorPosition()) && !hotSetThisFrame;
		}

		internal static bool IsActive(IMGUIId id, RectangleF rect)
		{
			if (game.IsActive && CanActive && rect.Contains(GetCursorPosition()) && !activeSetThisFrame)
				return CheckClickInput();

			return false; 
		}

		internal static bool JustClicked(IMGUIId id, RectangleF rect)
		{
			return game.IsActive && rect.Contains(GetCursorPosition()) && CheckClickInput() && !activeSetThisFrame && !activeSetLast;
		}
		#endregion

		internal static bool CheckClickInput()
		{
			return game.IsActive && CanClick && (inputManager.IsPressed(MouseInput.LeftButton) || inputManager.IsPressed(Buttons.A, 0));
		}
		internal static Vector2 GetCursorPosition()
		{
			if (!useOverrideCursorPosition)
				return inputManager.GetMousePosition().ToVector2() * scale;
			else return overrideCursorPosition * scale;
		}

		public static bool AnyHotOrActive()
		{
			return !(hot == IMGUIId.Inactive && active == IMGUIId.Inactive);
		}

		public static bool AnyClicked()
		{
			return !(clicked == IMGUIId.Inactive);
		}
	}
}

