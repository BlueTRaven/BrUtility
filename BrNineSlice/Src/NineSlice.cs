using BlueRavenUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrNineSlice
{
	public class NineSlice
	{
		public enum DrawMode
		{
			Simple,
			Tile,
			Stretch,
		}

		public static NineSlice Empty { get { return new NineSlice(null, Size.Zero, 0); } }
		
		public Size size;
		
		public float distLeft;
		public float distRight;
		public float distTop;
		public float distBottom;

		public DrawMode drawMode;

		#region Properties
		public RectangleF CornerTopLeft => new RectangleF(0, 0, distLeft, distTop);
		public RectangleF CornerTopRight => new RectangleF(size.Width - distRight, 0, distRight, distTop);
		public RectangleF CornerBottomLeft => new RectangleF(0, size.Height - distBottom, distLeft, distBottom);
		public RectangleF CornerBottomRight => new RectangleF(size.Width - distRight, size.Height - distBottom, distRight, distBottom);

		public RectangleF EdgeLeft => new RectangleF(0, distTop, distLeft, size.Height - (distBottom + distTop));
		public RectangleF EdgeRight => new RectangleF(size.Width - distRight, distTop, distRight, size.Height - (distBottom + distTop));

		public RectangleF EdgeTop => new RectangleF(distLeft, 0, size.Width - (distRight + distLeft), distTop);
		public RectangleF EdgeBottom => new RectangleF(distLeft, size.Height - distBottom, size.Width - (distRight + distLeft), distBottom);

		public RectangleF Center => new RectangleF(distLeft, distTop, size.Width - (distRight + distLeft), size.Height - (distBottom + distTop));

		public float InnerWidth => size.Width - (distRight + distLeft);
		public float InnerHeight => size.Height - (distTop + distBottom);

		public Texture2D TexTopLeft { get; private set; }
		public Texture2D TexTopRight { get; private set; }
		public Texture2D TexBotLeft { get; private set; }
		public Texture2D TexBotRight { get; private set; }

		public Texture2D TexTop { get; private set; }
		public Texture2D TexLeft { get; private set; }
		public Texture2D TexRight { get; private set; }
		public Texture2D TexBottom { get; private set; }

		public Texture2D TexCenter { get; private set; }
		#endregion

		private Texture2D texture;
		private bool hasTextures;

		public NineSlice(Texture2D texture, Size size, float distance, DrawMode drawMode = DrawMode.Tile)
		{
			this.size = size;
			this.distLeft = distance;
			this.distRight = distance;
			this.distTop = distance;
			this.distBottom = distance;

			this.drawMode = drawMode;
			
			if (texture != null)
				GetTextures(texture);
		}

		public NineSlice(Texture2D texture, Size size, float distLeft, float distRight, float distTop, float distBottom, DrawMode drawMode = DrawMode.Tile)
		{
			this.size = size;
			this.distLeft = distLeft;
			this.distRight = distRight;
			this.distTop = distTop;
			this.distBottom = distBottom;
			
			this.drawMode = drawMode;

			if (texture != null)
				GetTextures(texture);
		}

		public void GetTextures(Texture2D texture)
		{
			this.texture = texture;
			if (!hasTextures)
			{
				TexTopLeft = GetTexturePart(texture, CornerTopLeft.ToRectangle(), "topleft");
				TexTopRight = GetTexturePart(texture, CornerTopRight.ToRectangle(), "topright");
				TexBotLeft = GetTexturePart(texture, CornerBottomLeft.ToRectangle(), "botleft");
				TexBotRight = GetTexturePart(texture, CornerBottomRight.ToRectangle(), "botright");

				TexTop = GetTexturePart(texture, EdgeTop.ToRectangle(), "top");
				TexLeft = GetTexturePart(texture, EdgeLeft.ToRectangle(), "left");
				TexRight = GetTexturePart(texture, EdgeRight.ToRectangle(), "right");
				TexBottom = GetTexturePart(texture, EdgeBottom.ToRectangle(), "bottom");

				TexCenter = GetTexturePart(texture, Center.ToRectangle(), "center");
			}

			hasTextures = true;
		}

		private Texture2D GetTexturePart(Texture2D texture, Rectangle sourceRect, string type)
		{
			Texture2D finalTex = new Texture2D(texture.GraphicsDevice, sourceRect.Width, sourceRect.Height);

			Color[] colors = new Color[texture.Width * texture.Height];
			texture.GetData(colors);

			List<Color> finalColors = new List<Color>();
			for (int i = 0; i < colors.Length; i++)
			{
				int size = texture.Width;
				int y = i / size;
				int x = i % size;

				if (x >= sourceRect.X && x < sourceRect.X + sourceRect.Width && y >= sourceRect.Y && y < sourceRect.Y + sourceRect.Height)
				{
					finalColors.Add(colors[i]);
				}
			}

			finalTex.SetData(finalColors.ToArray());
			finalTex.Name = texture.Name.Replace('/', '-') + "-" + type;

			/*if (!Directory.Exists("debug/"))
				Directory.CreateDirectory("debug/");

			using (FileStream fs = File.Create("debug/output_debug_" + (texture.Name.Replace('/', '-')) + "-" + type + ".png"))
			{
				finalTex.SaveAsPng(fs, finalTex.Width, finalTex.Height);
			}/**/

			return finalTex;
		}

		public NineSlice()
		{
			this.drawMode = DrawMode.Simple;
		}

		public void Draw(SpriteBatch batch, Color color, RectangleF rectangle, float scale, float drawDepth)
		{
			//GetTexturePart(texture, CornerTopLeft.ToRectangle());
			
			rectangle.width = (int)(Math.Round(rectangle.width / scale, MidpointRounding.AwayFromZero) * scale);
			rectangle.height = (int)(Math.Round(rectangle.height / scale, MidpointRounding.AwayFromZero) * scale);

			if (texture != null)
			{
				if (drawMode != DrawMode.Simple)
				{
					Vector2 topLeft = rectangle.Position.ToPoint().ToVector2();
					Vector2 topRight = (rectangle.Position + (new Vector2(rectangle.width - distRight * scale, 0))).ToPoint().ToVector2();
					Vector2 bottomLeft = (rectangle.Position + (new Vector2(0, rectangle.height - distBottom * scale))).ToPoint().ToVector2();
					Vector2 bottomRight = (rectangle.Position + new Vector2(rectangle.width - distRight * scale, rectangle.height - distBottom * scale)).ToPoint().ToVector2();

					batch.Draw(TexTopLeft, topLeft, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
					batch.Draw(TexTopRight, topRight, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
					batch.Draw(TexBotLeft, bottomLeft, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
					batch.Draw(TexBotRight, bottomRight, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth + 0.01f);

					RectangleF fTop = new RectangleF(0, 0, ((rectangle.width / scale) - ((distLeft + distRight))), distTop);
					RectangleF fBottom = new RectangleF(0, 0, fTop.width, distBottom);
					RectangleF fLeft = new RectangleF(0, 0, distLeft, (rectangle.height / scale) - (distTop + distBottom));
					RectangleF fRight = new RectangleF(0, 0, distRight, fLeft.height);

					batch.Draw(TexTop, new Vector2(topLeft.X + distLeft * scale, topLeft.Y), fTop.ToRectangle(), color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
					batch.Draw(TexLeft, new Vector2(topLeft.X, topLeft.Y + distTop * scale), fLeft.ToRectangle(), color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
					batch.Draw(TexRight, new Vector2(topRight.X, topRight.Y + distTop * scale), fRight.ToRectangle(), color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
					batch.Draw(TexBottom, new Vector2(bottomLeft.X + distLeft * scale, bottomLeft.Y), fBottom.ToRectangle(), color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);

					RectangleF fCenterSR = new RectangleF(Vector2.Zero, (rectangle.width / scale) - (distLeft + distRight), 
						(rectangle.height / scale) - (distTop + distBottom));
					batch.Draw(TexCenter, new Vector2(rectangle.x + distLeft * scale, rectangle.y + distTop * scale).ToPoint().ToVector2()
						, fCenterSR.ToRectangle(), color, 0, Vector2.Zero, scale, SpriteEffects.None, drawDepth);
				}
				else
				{
					batch.Draw(texture, rectangle.ToRectangle(), null, color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
				}
			}
		}

		private void DrawCenter(SpriteBatch batch, Texture2D texture, Color color, RectangleF rectangle, float drawDepth = 0)
		{	//kinda spaghetti, but it works
			Rectangle drawRect;

			float xnum = (rectangle.width - (distLeft + distRight)) / Center.width;
			float xover = xnum % 1;

			float ynum = (rectangle.height - (distTop + distBottom)) / Center.height; //number of times to tile the texture
			float yover = ynum % 1;

			for (int y = 0; y < (int)ynum; y++)
			{
				float ypos = Center.height * y;

				for (int x = 0; x < (int)xnum; x++)
				{
					float xpos = Center.width * x;
					batch.Draw(texture, new Vector2(rectangle.x + distLeft + xpos, rectangle.y + distTop + ypos).ToPoint().ToVector2(), Center.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
				}

				drawRect = new Rectangle(new Vector2(rectangle.x + distLeft + (Center.width * (int)xnum), rectangle.y + distTop + ypos).ToPoint(), 
					new Vector2(xover * Center.width, Center.height).ToPoint());
				batch.Draw(texture, drawRect, Center.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			}

			RectangleF centerMod = Center;
			centerMod.height = (float)Math.Round(centerMod.height * yover, MidpointRounding.AwayFromZero);
			for (int x = 0; x < (int)xnum; x++)
			{
				float xpos = centerMod.width * x;
				batch.Draw(texture, new Vector2(rectangle.x + distLeft + xpos, rectangle.y + distTop + (Center.height * (int)ynum)), centerMod.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
			}

			drawRect = new RectangleF(rectangle.x + distLeft + (Center.width * (int)xnum), rectangle.y + distTop + (Center.height * (int)ynum), (float)Math.Round(xover * centerMod.width, MidpointRounding.AwayFromZero), centerMod.height).ToRectangle();
			batch.Draw(texture, drawRect, centerMod.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
		}

		private void DrawTop(SpriteBatch batch, Texture2D texture, Color color, RectangleF rectangle, float drawDepth = 0)
		{
			float ypos = rectangle.y;

			if (drawMode == DrawMode.Stretch)
				batch.Draw(texture, new RectangleF(rectangle.x + distLeft, ypos, rectangle.width - (distLeft + distRight), distTop).ToRectangle(), EdgeTop.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			else
			{
				float num = (rectangle.width - (distLeft + distRight)) / EdgeTop.width; //number of times to tile the texture
				float over = num % 1;
				for (int i = 0; i < (int)num; i++)
				{
					float xpos = EdgeTop.width * i;
					batch.Draw(texture, new Vector2(rectangle.x + distLeft + xpos, ypos).ToPoint().ToVector2(), EdgeTop.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
				}

				Rectangle drawRect = new Rectangle(new Vector2(rectangle.x + distLeft + (EdgeTop.width * (int)num), ypos).ToPoint(), 
					new Vector2(over * EdgeTop.width, distTop).ToPoint());
				batch.Draw(texture, drawRect, EdgeTop.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			}
		}

		private void DrawRight(SpriteBatch batch, Texture2D texture, Color color, RectangleF rectangle, float drawDepth = 0)
		{
			float xpos = rectangle.x + rectangle.width - distRight;

			if (drawMode == DrawMode.Stretch)
				batch.Draw(texture, new RectangleF(xpos, rectangle.y + distTop, distRight, rectangle.height - (distTop + distBottom)).ToRectangle(), 
					EdgeRight.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			else
			{
				float num = (rectangle.height - (distTop + distBottom)) / EdgeRight.height; //number of times to tile the texture
				float over = num % 1;
				for (int i = 0; i < (int)num; i++)
				{
					float ypos = EdgeRight.height * i;
					batch.Draw(texture, new Vector2(xpos, rectangle.y + distTop + ypos).ToPoint().ToVector2(), EdgeRight.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
				}

				Rectangle drawRect = new Rectangle(new Vector2(xpos, rectangle.y + distTop + (EdgeRight.height * (int)num)).ToPoint(), 
					new Vector2(distRight, over * EdgeRight.height).ToPoint());
				batch.Draw(texture, drawRect, EdgeRight.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			}
		}

		private void DrawBottom(SpriteBatch batch, Texture2D texture, Color color, RectangleF rectangle, float drawDepth = 0)
		{
			float ypos = rectangle.y + (rectangle.height - distBottom);
			if (drawMode == DrawMode.Stretch)
				batch.Draw(texture, new RectangleF(rectangle.x + distLeft, ypos, (rectangle.width - (distLeft + distRight)), distBottom).ToRectangle(),
					EdgeBottom.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			else
			{
				float num = (rectangle.width - (distLeft + distRight)) / EdgeBottom.width; //number of times to tile the texture
				float over = num % 1;
				for (int i = 0; i < (int)num; i++)
				{
					float xpos = EdgeBottom.width * i;
					batch.Draw(texture, new Vector2(rectangle.x + distLeft + xpos, ypos).ToPoint().ToVector2(), EdgeBottom.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
				}

				Rectangle drawRect = new Rectangle(new Vector2(rectangle.x + distLeft + (EdgeBottom.width * (int)num), ypos).ToPoint(), 
					new Vector2(over * EdgeBottom.width, distBottom).ToPoint());
				batch.Draw(texture, drawRect, EdgeBottom.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			}
		}

		private void DrawLeft(SpriteBatch batch, Texture2D texture, Color color, RectangleF rectangle, float drawDepth = 0)
		{
			float xpos = rectangle.x;

			if (drawMode == DrawMode.Stretch)
				batch.Draw(texture, new RectangleF(xpos, rectangle.y + distTop, distLeft, rectangle.height - (distTop + distBottom)).ToRectangle(), EdgeLeft.ToRectangle(), color,
					0, Vector2.Zero, SpriteEffects.None, drawDepth);
			else
			{
				float num = (rectangle.height - (distTop + distBottom)) / EdgeLeft.height; //number of times to tile the texture
				float over = num % 1;
				for (int i = 0; i < (int)num; i++)
				{
					float ypos = EdgeLeft.height * i;
					batch.Draw(texture, new Vector2(xpos, rectangle.y + distTop + ypos).ToPoint().ToVector2(), EdgeLeft.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
				}

				Rectangle drawRect = new Rectangle(new Vector2(xpos, rectangle.y + distTop + (EdgeLeft.height * (int)num)).ToPoint(), 
					new Vector2(distLeft, over * EdgeLeft.height).ToPoint());
				batch.Draw(texture, drawRect, EdgeLeft.ToRectangle(), color, 0, Vector2.Zero, SpriteEffects.None, drawDepth);
			}
		}
	}
}