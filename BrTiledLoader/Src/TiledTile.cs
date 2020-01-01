using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace BrTiledLoader
{
	public class TiledTile
	{
		public int id;
		public string type;

		//public Animation animation;

		public string textureName;
		private Texture2D texture;
		public Rectangle sourceRectangle;

		public float drawPriority;

		public Dictionary<string, string> properties;

		public TiledTile()
		{

		}

		public void Initialize(TmxTilesetTile tile, float drawPriority, string textureName, Rectangle sourceRectangle)
		{
			this.id = tile.Id;

			this.drawPriority = drawPriority;
			this.textureName = textureName;
			this.sourceRectangle = sourceRectangle;

			this.type = tile.Type;
			
			properties = tile.Properties;
		}

		public void Initialize(float drawPriority, Texture2D texture, Rectangle sourceRectangle, Dictionary<string, string> properties)
		{
			this.drawPriority = drawPriority;
			this.texture = texture;
			this.sourceRectangle = sourceRectangle;
			
			this.properties = new Dictionary<string, string>();

			if (properties != null)
			{	//NOTE: we need to do this this way otherwise the properties dictionary will remain the TiledSharp version (for some reason???)
				this.properties = new Dictionary<string, string>();

				foreach (string key in properties.Keys)
				{
					this.properties.Add(key, properties[key]);
				}
			}
		}

		public bool HasTexture()
		{
			return texture != null;
		}

		public void SetTexture(Texture2D texture)
		{
			if (this.texture == null)
				this.texture = texture;
		}

		public Texture2D GetTexture()
		{
			if (texture != null)
				return texture;
			else return null;
		}
	}
}
