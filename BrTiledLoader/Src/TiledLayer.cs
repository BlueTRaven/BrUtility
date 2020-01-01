using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrTiledLoader
{
	public class TiledLayer
	{
		public string name;
		public float drawPriority;
		public TiledTileInstance[,] tiles;

		public Dictionary<string, string> properties;

		public TiledLayer()
		{

		}

		public TiledLayer(string name, float drawPriority, TiledTileInstance[,] tiles, Dictionary<string, string> properties)
		{
			this.name = name;
			this.drawPriority = drawPriority;
			this.tiles = tiles;

			this.properties = new Dictionary<string, string>();

			if (properties != null)
			{
				foreach (string key in properties.Keys)
				{
					this.properties.Add(key, properties[key]);
				}
			}
		}

		public bool IsValidTile(Point pos)
		{
			if (pos.X >= 0 && pos.X < tiles.GetLength(0) && pos.Y >= 0 && pos.Y < tiles.GetLength(1))
			{
				TiledTileInstance tile = tiles[pos.X, pos.Y];
				return tile != null;
			}
			return false;
		}

		/// <summary>
		/// A blank tile instance is:
		/// A tile that is not invalid, and
		/// Does not have a tile attached to it.
		/// </summary>
		/// <returns>False if the tile is invalid or the tile is not blank. True if the tile is valid and blank.</returns>
		public bool IsBlankTile(Point pos)
		{
			return IsValidTile(pos) && GetTile(pos).tile == null;
		}

		public TiledTileInstance GetTile(Point pos)
		{
			return tiles[pos.X, pos.Y];
		}
	}
}
