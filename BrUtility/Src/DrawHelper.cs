using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrUtility
{
    public static class DrawHelper
    {
		public static BlendState AlphaSubtract;

		public static BlendState GreyscaleToAlpha;	//draw greyscale to alpha where black = alpha 0 and white = alpha 1

		static DrawHelper()
		{
			AlphaSubtract = new BlendState()
			{
				ColorSourceBlend = Blend.SourceAlpha,
				AlphaSourceBlend = Blend.SourceAlpha,
				ColorDestinationBlend = Blend.One,
				AlphaDestinationBlend = Blend.One,
				ColorBlendFunction = BlendFunction.ReverseSubtract,
				AlphaBlendFunction = BlendFunction.ReverseSubtract,
			};

			GreyscaleToAlpha = CopyBlendState(BlendState.AlphaBlend);
			GreyscaleToAlpha.AlphaSourceBlend = Blend.InverseSourceColor;
			GreyscaleToAlpha.ColorSourceBlend = Blend.One;
			GreyscaleToAlpha.AlphaBlendFunction = BlendFunction.Add;
		}

		public static void LoadContent(GraphicsDevice graphics)
		{
			whitePixel = new Texture2D(graphics, 1, 1);
			whitePixel.SetData(new Color[] { Color.White });
            whitePixel.Name = "Whitepixel";

            blackPixel = new Texture2D(graphics, 1, 1);
            blackPixel.SetData(new Color[] { Color.Black });
            blackPixel.Name = "Blackpixel";

            transparentPixel = new Texture2D(graphics, 1, 1);
            transparentPixel.SetData(new Color[] { new Color(0, 0, 0, 0) });
            transparentPixel.Name = "TransparentPixel";

            normalPixel = new Texture2D(graphics, 1, 1);
            normalPixel.SetData(new Color[] { new Color(127, 127, 255) });
            normalPixel.Name = "NormalPixel";
        }

        internal static Texture2D whitePixel;
        public static Texture2D WhitePixel
		{
			get
			{
				if (whitePixel != null)
					return whitePixel;
				else throw new Exception("DrawHelper has not been initialized. Call DrawHelper.Initialize in your LoadContent override.");
			}
			set { whitePixel = value; }
		}

		internal static Texture2D blackPixel;
		public static Texture2D BlackPixel
        {
            get
            {
                if (blackPixel != null)
                    return blackPixel;
                else throw new Exception("DrawHelper has not been initialized. Call DrawHelper.Initialize in your LoadContent override.");
            }
            set { blackPixel = value; }
        }

        internal static Texture2D transparentPixel;
        public static Texture2D TransparentPixel
        {
            get
            {
                if (transparentPixel != null)
                    return transparentPixel;
                else throw new Exception("DrawHelper has not been initialized. Call DrawHelper.Initialize in your LoadContent override.");
            }
            set { transparentPixel = value; }
        }

        //For use in normal maps.
        internal static Texture2D normalPixel;
        public static Texture2D NormalPixel
        {
            get
            {
                if (normalPixel != null)
                    return normalPixel;
                else throw new Exception("DrawHelper has not been initialized. Call DrawHelper.Initialize in your LoadContent override.");
            }
            set { normalPixel = value; }
        }

        public static BlendState CopyBlendState(BlendState copy)
		{
			BlendState blendState = new BlendState()
			{
				AlphaBlendFunction = copy.AlphaBlendFunction,
				AlphaDestinationBlend = copy.AlphaDestinationBlend,
				AlphaSourceBlend = copy.AlphaSourceBlend,
				BlendFactor = copy.BlendFactor,
				ColorBlendFunction = copy.ColorBlendFunction,
				ColorDestinationBlend = copy.ColorDestinationBlend,
				ColorSourceBlend = copy.ColorSourceBlend,
				ColorWriteChannels = copy.ColorWriteChannels,
				ColorWriteChannels1 = copy.ColorWriteChannels1,
				ColorWriteChannels2 = copy.ColorWriteChannels2,
				ColorWriteChannels3 = copy.ColorWriteChannels3,
				IndependentBlendEnable = copy.IndependentBlendEnable,
				MultiSampleMask = copy.MultiSampleMask
			};

			return blendState;
		}

		public static void DrawRectangle(this SpriteBatch batch, Rectangle area, Color color, float depth = 0)
        {
            Texture2D whitePixel = DrawHelper.WhitePixel;

            batch.Draw(whitePixel, area, null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void DrawRectangle(this SpriteBatch batch, RectangleF area, Color color, float depth = 0)
        {
            Texture2D whitePixel = DrawHelper.WhitePixel;

            batch.Draw(whitePixel, area.ToRectangle(), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void DrawHollowRectangle(this SpriteBatch batch, Rectangle area, int width, Color color, float depth = 0)
        {
            Texture2D whitePixel = DrawHelper.WhitePixel;

            batch.Draw(whitePixel, new Rectangle(area.X, area.Y, area.Width, width), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y, width, area.Height), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(whitePixel, new Rectangle(area.X + area.Width - width, area.Y, width, area.Height), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y + area.Height - width, area.Width, width), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void DrawHollowRectangle(this SpriteBatch batch, RectangleF area, int width, Color color, float depth = 0)
        {
            Texture2D whitePixel = DrawHelper.WhitePixel;

            batch.Draw(whitePixel, new RectangleF(area.x, area.y, area.width, width).ToRectangle(), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(whitePixel, new RectangleF(area.x, area.y, width, area.height).ToRectangle(), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(whitePixel, new RectangleF(area.x + area.width - width, area.y, width, area.height).ToRectangle(), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(whitePixel, new RectangleF(area.x, area.y + area.height - width, area.width, width).ToRectangle(), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void DrawHollowCircle(this SpriteBatch batch, Vector2 center, float radius, Color color, int lineWidth = 2, int segments = 16, float depth = 0)
        {
            Vector2[] vertex = new Vector2[segments];

            double increment = Math.PI * 2.0 / segments;
            double theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                vertex[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;
            }

            DrawHollowPolygon(batch, vertex, segments, color, lineWidth, depth);
        }
        public static void DrawHollowPolygon(this SpriteBatch batch, Vector2[] vertex, int count, Color color, int lineWidth, float depth = 0)
        {
            Texture2D whitePixel = DrawHelper.WhitePixel;

            if (count > 0)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    DrawLine(batch, vertex[i], vertex[i + 1], color, lineWidth, depth);
                }
                DrawLine(batch, vertex[count - 1], vertex[0], color, lineWidth, depth);
            }
        }

        public static void DrawLine(this SpriteBatch batch, Vector2 begin, Vector2 end, Color color, int width = 1, float depth = 0)
        {
            Texture2D whitePixel = DrawHelper.WhitePixel;

            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            batch.Draw(whitePixel, r, null, color, angle, Vector2.Zero, SpriteEffects.None, depth);
        }

		public static void DrawRotatedRectangle(this SpriteBatch batch, RotatedRectangleF rectangle, Vector2 offset, Color color, float depth)
		{
			RotatedRectangleF offsetRect = rectangle.Offset(offset);
			batch.DrawLine(offsetRect.TopLeft, offsetRect.TopRight, color, 1, depth);
			batch.DrawLine(offsetRect.TopRight, offsetRect.BottomRight, color, 1, depth);
			batch.DrawLine(offsetRect.BottomRight, offsetRect.BottomLeft, color, 1, depth);
			batch.DrawLine(offsetRect.BottomLeft, offsetRect.TopLeft, color, 1, depth);

			batch.DrawHollowCircle((new Vector2(offsetRect.baseRectangle.Left, offsetRect.baseRectangle.Top) + offsetRect.origin), 8, color, 1, 16, 1);
		}
	}
}
