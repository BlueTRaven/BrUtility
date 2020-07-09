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
		
		public List<TiledObject> Objects { get; private set; } = new List<TiledObject>();

		public Dictionary<string, TiledLayer> Layers { get; private set; } = new Dictionary<string, TiledLayer>();

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
			Layers.Add(name, layer);
		}

		public bool HasLayer(string name)
		{
			return Layers.ContainsKey(name);
		}

		public TiledLayer GetLayer(string name)
		{
			if (HasLayer(name))
				return Layers[name];
			else return null;
		}

		public List<TiledLayer> GetLayers()
		{
			return Layers.Values.ToList();
		}

		public void SetTile(string layer, Point pos, TiledTile tileType)
		{
			if (tileType == null)
				Layers[layer].tiles[pos.X, pos.Y] = default;
			else Layers[layer].tiles[pos.X, pos.Y] = new TiledLayer.TileType() { id = (short)tileType.gid };
		}

		public TiledTileInstance GetTile(string layer, Point pos)
		{
			return Layers[layer].GetTile(pos);
		}
	}
}
