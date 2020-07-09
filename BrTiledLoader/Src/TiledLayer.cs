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
		public struct TileType
		{
			public short id;
		}

		public string name;
		public float drawPriority;
		//public TiledTileInstance[,] tiles;
		public TileType[,] tiles;

		public float offsetVertical;
		public float offsetHorizontal;

		public Dictionary<string, string> properties;

		private Dictionary<int, TiledTile> tileDefinitions;

		public TiledLayer()
		{

		}

		public TiledLayer(string name, float drawPriority, float offsetVertical, float offsetHorizontal, Dictionary<int, TiledTile> tileDefinitions, TileType[,] tiles, Dictionary<string, string> properties)
		{
			this.name = name;
			this.drawPriority = drawPriority;
			this.offsetVertical = offsetVertical;
			this.offsetHorizontal = offsetHorizontal;
			this.tileDefinitions = tileDefinitions;
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
				TiledTileInstance tile = GetTile(pos);
				return tile.valid;
			}
			return false;
		}

		public TiledTileInstance GetTile(Point pos)
		{
			if (pos.X < 0 || pos.X > tiles.GetLength(0) || pos.Y < 0 || pos.Y >= tiles.GetLength(1))
			{
				//TODO log this
				return default;
			}

			int definition = tiles[pos.X, pos.Y].id;

			//return invalid tile if definition is 0
			//we have to manually handle this case since tileDefinitions doesn't keep track of the 0 index tile, since it would be null
			if (definition == 0)
				return default;
			else return new TiledTileInstance(tileDefinitions[definition], pos, name);
		}
	}
}
