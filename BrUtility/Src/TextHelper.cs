using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrUtility
{
	public static class TextHelper
	{
		public enum OverFlowAction
		{
			None,
			DontDrawOverflow,
			DontDrawAny
		}

		public struct FontInfo
		{
			[NonSerialized]
			public SpriteFont font;
			public float size;
			public bool outline;

			public Color outlineColor;

			public float LineSpacing => font.LineSpacing * size;
			public float Spacing => font.Spacing * size;

			public FontInfo(SpriteFont font, float size, bool outline, Color? outlineColor = null)
			{
				this.font = font;
				this.size = size;
				this.outline = outline;

				if (outlineColor.HasValue)
					this.outlineColor = outlineColor.Value;
				else this.outlineColor = Color.Black;
			}

			public Size StringSize(string str)
			{
				return (font.MeasureString(str) * size).ToSize();
			}

			public float StringWidth(string str)
			{
				return StringSize(str).Width;
			}

			public float StringHeight(string str)
			{
				return StringSize(str).Height;
			}
		}

		public static bool debugDrawTextBounds = false;

		private static RasterizerState scissorTestRasterizerState = new RasterizerState() { ScissorTestEnable = true };

		public struct DrawnText
        {
			public Vector2 position;
			public Rectangle bounds;
			public string wrappedText;
        }

		/// <summary>
		/// Draws text indside the given bounds.
		/// </summary>
		/// <param name="text">The text to draw.</param>
		/// <param name="bounds">The bounds to draw within.</param>
		/// <param name="alignment">The alignment within the given bounds.</param>
		/// <param name="wrapWidth">Defines how wide the text can draw before being wrapped. set to -1 to not wrap.</param>
		/// <param name="overflowAction">Defines what happens when the text overflows on the Y axis (i.e. goes out of bounds.)</param>
		public static void DrawText(SpriteBatch batch, FontInfo fontText, string text, Color color, Rectangle bounds, Enums.Alignment alignment, int wrapWidth, float drawDepth = 0, OverFlowAction overflowAction = OverFlowAction.DontDrawOverflow)
		{
			WrappedText wrappedText = new WrappedText(wrapWidth, text);

			//If wrapWidth is -1, don't wrap the text.
			if (wrapWidth != -1)
				wrappedText = GetWrappedText(fontText, text, wrapWidth);

			Vector2 alignmentOffset = GetAlignmentOffset(fontText, text, wrappedText.offset, wrappedText.length, bounds, alignment);

			DrawText(batch, fontText, wrappedText, alignmentOffset, color, bounds, drawDepth, overflowAction);
		}

		public struct WrappedText
        {
			public float wrapWidth;
			public string text;
			public int offset;
			public int length;

			public WrappedText(float wrapWidth, string text, int offset = 0, int length = -1)
			{
				this.wrapWidth = wrapWidth;
				this.text = text;
				this.offset = offset;
				this.length = length == -1 ? text.Length : length;
			}
        }

		public static void DrawText(SpriteBatch batch, FontInfo fontText, WrappedText wrappedText, Vector2 alignmentOffset, Color color, Rectangle bounds, float drawDepth = 0, OverFlowAction overflowAction = OverFlowAction.DontDrawOverflow)
		{
			//If wrapWidth is -1, don't wrap the text.
			Vector2 size = fontText.StringSize(wrappedText.text).ToVector2();
			if (size.Y > bounds.Size.Y && overflowAction == OverFlowAction.DontDrawAny)
				return;

			RecreateDrawString(batch, bounds.Location.ToVector2() + alignmentOffset, fontText, 
				wrappedText.text, wrappedText.offset, wrappedText.length, drawDepth, color);

			if (debugDrawTextBounds)
			{
				batch.DrawHollowRectangle(bounds, 1, Color.Red, 1);
				bounds.Width = (int)wrappedText.wrapWidth;
				batch.DrawHollowRectangle(bounds, 1, Color.Pink, 1);
			}
		}

		public static void FixInvalidTextCharacters(SpriteFont font, ref string text)
		{
			var validChars = font.GetGlyphs();

			for (int i = 0; i < text.Length; i++)
			{
				char c = (char)text[i];
				if (!validChars.ContainsKey(c))
					text = text.Replace(c, '?');
			}
		}

		private static void RecreateDrawString(SpriteBatch batch, Vector2 position, FontInfo font, string text, int textOffset, int textLength, float drawDepth, Color color)
		{
			Vector2 offset = Vector2.Zero;
			bool firstGlyphOfLine = true;

			var glyphs = font.font.GetGlyphs();
			for (int i = textOffset; i < textOffset + textLength; i++)
			{
				char character = text[i];

				if (character == '\r')
					continue;

				if (character == '\n')
				{
					offset.X = 0;
					offset.Y += font.LineSpacing;
					firstGlyphOfLine = true;
					continue;
				}

				//TODO this assumes the default character has a value
				SpriteFont.Glyph currentGlyph = glyphs[font.font.DefaultCharacter.Value];
				if (glyphs.ContainsKey(character))
					currentGlyph = glyphs[character];

				if (firstGlyphOfLine)
				{
					offset.X = Math.Max(currentGlyph.LeftSideBearing, 0);
					firstGlyphOfLine = false;
				}
				else offset.X += font.Spacing + currentGlyph.LeftSideBearing;

				Vector2 finalPos = offset;
				finalPos.X += currentGlyph.Cropping.X * font.size;
				finalPos.Y += currentGlyph.Cropping.Y * font.size;
				finalPos += position;

				RectangleF sourceRect = new RectangleF();
				sourceRect.x = currentGlyph.BoundsInTexture.X;
				sourceRect.y = currentGlyph.BoundsInTexture.Y;
				sourceRect.width = currentGlyph.BoundsInTexture.Width;
				sourceRect.height = currentGlyph.BoundsInTexture.Height;

				DrawCharacter(batch, text, i, finalPos, font, sourceRect, drawDepth, color);

				offset.X += (currentGlyph.Width + currentGlyph.RightSideBearing) * font.size;
			}
		}

		private static void DrawCharacter(SpriteBatch batch, string text, int index, Vector2 position, FontInfo font, RectangleF sourceRect, float drawDepth, Color color)
		{
			if (font.outline)
			{
				float shadowDepth = 0.00025f;
				Color outlineColor = new Color(font.outlineColor, color.A);
				batch.Draw(font.font.Texture, position + new Vector2(-2, 0), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - shadowDepth);
				batch.Draw(font.font.Texture, position + new Vector2(2, 0), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - shadowDepth);
				batch.Draw(font.font.Texture, position + new Vector2(0, -2), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - shadowDepth);
				batch.Draw(font.font.Texture, position + new Vector2(0, 2), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - shadowDepth);
			}

			batch.Draw(font.font.Texture, position, sourceRect.ToRectangle(), color, 0, Vector2.Zero, font.size, SpriteEffects.None, drawDepth);
		}

		public static Vector2 GetLastCharacterPosition(FontInfo fontInfo, string text, Rectangle bounds, Enums.Alignment alignment)
		{
			Vector2 alignmentOffset = GetAlignmentOffset(fontInfo, text, 0, text.Length, bounds, alignment);
			Vector2 drawPosition = bounds.Location.ToVector2() + alignmentOffset;

			string wrapped = WrapText(fontInfo, text, bounds.Width);

			string[] lines = wrapped.Split('\n');

			Vector2 size = fontInfo.StringSize(wrapped).ToVector2();

			float ypos = size.Y - fontInfo.LineSpacing;
			float xpos = fontInfo.StringWidth(lines.Last());

			return drawPosition + new Vector2(xpos, ypos);
		}

		public static Vector2 GetAlignmentOffset(FontInfo font, string text, int offset, int length, Rectangle bounds, Enums.Alignment alignment)
		{
			//TODO: remove text.Substring. This is an allocation that I don't want!
			Vector2 size = Vector2.Zero;
			try { size = font.StringSize(text.Substring(offset, length)).ToVector2(); }
			catch
			{
				Logger.GetOrCreate("BrUtility").Log(Logger.LogLevel.Error, "Text has no size (empty or invalid string.)");
				return Vector2.Zero;
			}

			if (alignment == Enums.Alignment.TopLeft)
				return Vector2.Zero;
			else if (alignment == Enums.Alignment.Left)
				return new Vector2(0, (bounds.Height / 2) - (size.Y / 2));
			else if (alignment == Enums.Alignment.BottomLeft)
				return new Vector2(0, bounds.Height - size.Y);
			else if (alignment == Enums.Alignment.Bottom)
				return new Vector2((bounds.Width / 2) - (size.X / 2), bounds.Height - size.Y);
			else if (alignment == Enums.Alignment.BottomRight)
				return new Vector2(bounds.Width - size.X, bounds.Height - size.Y);
			else if (alignment == Enums.Alignment.Right)
				return new Vector2(bounds.Width - size.X, (bounds.Height / 2) - (size.Y / 2));
			else if (alignment == Enums.Alignment.TopRight)
				return new Vector2(bounds.Width - size.X, 0);
			else if (alignment == Enums.Alignment.Top)
				return new Vector2((bounds.Width / 2) - (size.X / 2), 0);
			else if (alignment == Enums.Alignment.Center)
				return new Vector2((bounds.Width / 2) - (size.X / 2), (bounds.Height / 2) - (size.Y / 2));
			else return Vector2.Zero;
		}

		public static WrappedText GetWrappedText(FontInfo fontInfo, string text, float lineWidth, int offset = 0, int length = -1)
        {
			return new WrappedText(lineWidth, text, offset, length);
        }

		const string space = " ";
		static string[] splitTextBy = new string[] { space };

		public static string WrapText(FontInfo font, string text, float lineWidth)
		{
			string[] words = text.Split(splitTextBy, StringSplitOptions.RemoveEmptyEntries);
			float spaceWidth = font.StringWidth(space);
			float spaceRemaining = lineWidth;
			float wordWidth;

			StringBuilder result = new StringBuilder();

			foreach (string word in words)
			{
				string fword = word;
				
				wordWidth = font.StringWidth(word);

				if (word.Contains("\n"))
				{
					string[] newlineWords = fword.Split('\n');

					for (int i = 0; i < newlineWords.Length; i++)
					{
						string newlineWord = newlineWords[i];
						
						wordWidth = font.StringWidth(newlineWord);

						if (wordWidth + spaceWidth > spaceRemaining)
						{
							result.AppendLine();
							spaceRemaining = lineWidth - wordWidth;
						}
						else
						{
							spaceRemaining -= (wordWidth + spaceWidth);
						}

						if (i != 0)
						{
							result.AppendLine();
							result.Append(newlineWord + space);
							spaceRemaining = lineWidth - wordWidth;  //this is setting to the previous word's length not the new one
						}
						else
						{
							result.Append(newlineWord + space);
						}
					}
					continue;   //we don't want to reach the end because reasons
				}
				else if (wordWidth + spaceWidth > spaceRemaining)
				{
					result.AppendLine();
					spaceRemaining = lineWidth - wordWidth;
				}
				else
				{
					spaceRemaining -= (wordWidth + spaceWidth);
				}
				result.Append(word + space);
			}

			//result.Remove(result.Length - 1, 1);    //remove the last character, which should be a space. This can mess up alignment.

			string fStr = result.ToString();
			while (fStr.EndsWith(" ") || fStr.EndsWith("\n"))
				fStr = fStr.Remove(fStr.Length - 1, 1);

			return fStr;
		}

		//Wraps words, but instead of wrapping them with newlines, wraps them into an array (with each element being a line).
        public static string[] WrapTextAsArray(FontInfo font, string text, float lineWidth)
        {
            string[] words = text.Split(splitTextBy, StringSplitOptions.RemoveEmptyEntries);
            float spaceWidth = font.StringWidth(space);
            float spaceRemaining = lineWidth;
            float wordWidth;

			StringBuilder currentLine = new StringBuilder();
			List<string> lines = new List<string>();

            foreach (string word in words)
            {
                string fword = word;

                wordWidth = font.StringWidth(word);

				//We split by space (and can't split by \n since we still want to force a newline)
				//so if we do happen to encounter a word with a newline in it, we must handle the before and after manually.
                if (word.Contains("\n"))
                {
                    string[] newlineWords = fword.Split('\n');

                    for (int i = 0; i < newlineWords.Length; i++)
                    {
                        string newlineWord = newlineWords[i];

                        wordWidth = font.StringWidth(newlineWord);

                        if (wordWidth + spaceWidth > spaceRemaining)
                        {
							lines.Add(currentLine.ToString());
							currentLine.Clear();
                            //result.AppendLine();
                            spaceRemaining = lineWidth - wordWidth;
                        }
                        else
                        {
                            spaceRemaining -= (wordWidth + spaceWidth);
                        }

                        if (i != 0)
                        {
                            lines.Add(currentLine.ToString());
                            currentLine.Clear();
							currentLine.Append(newlineWord + space);
                            //result.AppendLine();
                            //result.Append(newlineWord + space);
                            spaceRemaining = lineWidth - wordWidth;  //this is setting to the previous word's length not the new one
                        }
						else
						{
							currentLine.Append(newlineWord + space);
							//result.Append(newlineWord + space);
                        }
                    }
                    continue;   //we don't want to reach the end because reasons
                }
                else if (wordWidth + spaceWidth > spaceRemaining)
                {
					lines.Add(currentLine.ToString());
					currentLine.Clear();
                    //result.AppendLine();
                    spaceRemaining = lineWidth - wordWidth;
                }
                else
                {
                    spaceRemaining -= (wordWidth + spaceWidth);
                }

				currentLine.Append(word + space);
                //result.Append(word + space);
            }

			if (currentLine.Length > 0)
				lines.Add(currentLine.ToString());

			return lines.ToArray();
        }

        public static string FirstCharacterToLower(string str)
		{
			if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
				return str;

			return Char.ToLowerInvariant(str[0]) + str.Substring(1);
		}

		public static string defaultReplaceString =
			"abcdefghijklmnopqrstuvwxyz" +
			"ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
			"`0123456789-=" +
			"~!@#$%^&*()_+" +
			"[]\\;',./" +
			"{}|:\"<>?";

		public static char[] DefaultReplaceChars { get { return defaultReplaceString.ToCharArray(); } }

		public static string GetRandom(string str, int chance = 1, params char[] replaceChars)
		{
			if (chance <= 0)
				chance = 1;

			StringBuilder fstr = new StringBuilder(str);
			for (int i = 0; i < str.Length; i++)
			{
				if (Utility.rand.Next(0, chance + 1) == 0)
					fstr[i] = replaceChars[Utility.rand.Next(0, replaceChars.Length)];
			}

			return fstr.ToString();
		}
	}
}

