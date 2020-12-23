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
			//for extra packing
			public enum Flip : byte
			{
				FLIP_NONE = 0,
				FLIP_H = 1 << 0,
				FLIP_V = 1 << 1,
				FLIP_D = FLIP_H | FLIP_V
			}
			public ushort id;
			public Flip flip;
		}

		public string name;
		public float drawPriority;
		//public TiledTileInstance[,] tiles;
		public TileType[,] tiles;
		private int tilesWidth;
		private int tilesHeight;

		public float offsetVertical;
		public float offsetHorizontal;

		public Dictionary<string, string> properties;

		//private Dictionary<int, TiledTile> tileDefinitions;
		private TiledTile[] tileDefinitions;

		public TiledLayer()
		{

		}

		public TiledLayer(string name, float drawPriority, float offsetVertical, float offsetHorizontal, Dictionary<int, TiledTile> tileDefinitions, TileType[,] tiles, Dictionary<string, string> properties)
		{
			this.name = name;
			this.drawPriority = drawPriority;
			this.offsetVertical = offsetVertical;
			this.offsetHorizontal = offsetHorizontal;
			//this.tileDefinitions = tileDefinitions;
			this.tiles = tiles;
			this.tilesWidth = tiles.GetLength(0);
			this.tilesHeight = tiles.GetLength(1);

			this.tileDefinitions = new TiledTile[tileDefinitions.Max(x => x.Key)];

			foreach (int definition in tileDefinitions.Keys)
			{
				this.tileDefinitions[definition - 1] = tileDefinitions[definition];
			}

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

		private TiledTile cachedTile;
		private int cachedDefinition;

		public TiledTileInstance GetTile(Point pos)
		{
			if (pos.X < 0 || pos.X >= tilesWidth || pos.Y < 0 || pos.Y >= tilesHeight)
			{
				//TODO log this
				return default;
			}

			//return invalid tile if definition is 0
			//we have to manually handle this case since tileDefinitions doesn't keep track of the 0 index tile, since it would be null
			if (tiles[pos.X, pos.Y].id == 0)
				return default;
			else
			{
				if (cachedTile == null || cachedDefinition != tiles[pos.X, pos.Y].id)
				{
					cachedTile = tileDefinitions[tiles[pos.X, pos.Y].id - 1];
					cachedDefinition = tiles[pos.X, pos.Y].id;
				}

				return new TiledTileInstance(cachedTile, pos, name, tiles[pos.X, pos.Y].flip);
			}
		}
	}
}
