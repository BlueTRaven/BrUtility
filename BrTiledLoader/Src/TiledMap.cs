using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrTiledLoader
{
	public class TiledMap// : IDisposable
	{
		public Rectangle bounds;
		
		public List<TiledObject> tiledObjects = new List<TiledObject>();

		public Dictionary<string, TiledLayer> tiles = new Dictionary<string, TiledLayer>();

		public int width, height;
		public int tileWidth, tileHeight;

		public TiledMap()
		{
		}

		/*public void Dispose()
		{
			tiledObjects.Clear();
			tiledObjects = null;

			tiles.Clear();
			tiles = null;
			bounds = RectangleF.Empty;
			width = 0;
			height = 0;
			tileWidth = 0;
			tileHeight = 0;
		}*/

		public void AddLayer(string name, TiledLayer layer)
		{
			tiles.Add(name, layer);
		}

		public List<TiledLayer> GetLayers()
		{
			return tiles.Values.ToList();
		}

		public void SetTile(string layer, Point pos, TiledTileInstance instance)
		{
			tiles[layer].tiles[pos.X, pos.Y] = instance;
		}

		public TiledTileInstance GetTile(string layer, Point pos)
		{
			return tiles[layer].tiles[pos.X, pos.Y];
		}
	}
}
