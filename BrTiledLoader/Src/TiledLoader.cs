using BlueRavenUtility;
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
	/// <summary>
	/// Tiled Loader is a class that handles the loading of a Tiled Map from TiledSharp.
	/// Usage: Create instance, call Load Map. Discard Loader instance.
	/// </summary>
	public class TiledLoader
	{
		public static bool outputLoadMessage = true;

		private string loadMessage = "";
		public string LoadMessage
		{
			get { return loadMessage; }
			internal set
			{
				if (outputLoadMessage)
					loadMessage = value;
			}
		}

		public TiledLoader()
		{
			
		}

		/// <summary>
		/// Load a tmx map. Loads all related assets as well.
		/// </summary>
		/// <param name="map">The TmxMap asset to import data from.</param>
		public TiledMap LoadMap(TmxMap map)
		{
			TiledMap outMap = new TiledMap();

			LoadMessage = "loading tiles...";
			LoadTiles(map, outMap);
			LoadMessage = "loading objects...";
			LoadObjects(map, outMap);

			outMap.bounds = new Rectangle(0, 0, map.Width * map.TileWidth, map.Height * map.TileHeight);

			outMap.width = map.Width;
			outMap.height = map.Height;

			outMap.tileWidth = map.TileWidth;
			outMap.tileHeight = map.TileHeight;

			return outMap;
		}

		private void LoadTiles(TmxMap map, TiledMap outMap)
		{
			Dictionary<int, TiledTile> allTiles = new Dictionary<int, TiledTile>();

			LoadMessage = "Loading tilesets...";
			foreach (TmxTileset tileset in map.Tilesets)
			{
				foreach (TmxTilesetTile tilesetTile in tileset.Tiles.Values)
				{
					int gid = tilesetTile.Id + tileset.FirstGid;
					allTiles.Add(gid, ParseTile(tileset, tilesetTile));

					LoadMessage = "Loading tilesets...\n" +
						"Tileset " + tileset.Name + ", gid: " + gid;
				}
			}

			int layerIndex = 0;
			foreach (TmxLayer layer in map.Layers)
			{
				if (!layer.Visible)
					continue;   //invisible layers are not loaded.

				float drawPriority = 0;
				if (layer.Properties.ContainsKey("draw_priority"))
					float.TryParse(layer.Properties["draw_priority"], out drawPriority);

				outMap.AddLayer(layer.Name, new TiledLayer(layer.Name, drawPriority, new TiledTileInstance[map.Width, map.Height], layer.Properties));

				LoadMessage = "Blanking tiles...";
				int iter = 0;
				//Create all tiles first
				for (int x = 0; x < map.Width; x++)
				{
					for (int y = 0; y < map.Height; y++)
					{
						iter++;
						outMap.SetTile(layer.Name, new Point(x, y), new TiledTileInstance(null, new Point(x, y), layer.Name));
						LoadMessage = "Blanking tiles...\n(Layer: " + layer.Name + " (" + layerIndex + "/" + (map.Layers.Count - 1) + ") " + iter + "/" + layer.Tiles.Count + ")";
					}
				}

				LoadMessage = "Initializing tiles...";

				Dictionary<int, TiledTile> setTiles = new Dictionary<int, TiledTile>();
				
				iter = 0;
				foreach (TmxLayerTile layerTile in layer.Tiles)
				{
					if (layerTile.Gid != 0 && allTiles.ContainsKey(layerTile.Gid))
					{
						iter++;
						outMap.SetTile(layer.Name, new Point(layerTile.X, layerTile.Y), new TiledTileInstance(allTiles[layerTile.Gid], new Point(layerTile.X, layerTile.Y), layer.Name));
						LoadMessage = "Initializing tiles...\n(Layer: " + layer.Name + " (" + layerIndex + "/" + (map.Layers.Count - 1) + ") " + iter + "/" + layer.Tiles.Count + ")";
						continue;
					}
				}

				layerIndex++;
			}
		}

		private void LoadObjects(TmxMap map, TiledMap outMap)
		{
			foreach (TmxObjectGroup objGroup in map.ObjectGroups)
			{
				foreach (TmxObject obj in objGroup.Objects)
				{
					TiledObject newObj = new TiledObject();
					newObj.LoadObj(obj);
					outMap.tiledObjects.Add(newObj);
				}
			}
		}

		private TmxTileset GetTileset(TmxLayerTile tile, List<TmxTileset> tilesets)
		{
			if (tilesets.Count > 1)
			{
				for (int i = 0; i < tilesets.Count; i++)
				{
					TmxTileset currTileset = tilesets[i];

					if (tile.Gid >= currTileset.FirstGid && tile.Gid < currTileset.FirstGid + currTileset.TileCount && (tile.Gid - currTileset.FirstGid) < currTileset.TileCount)
						return currTileset;
				}
			}
			else
			{   //if the number of tilesets is only one, then that must be the tileset the tile belongs to.
				return tilesets[0];
			}

			return null;
		}

		private TmxTilesetTile GetTilesetTile(TmxLayerTile tile, TmxTileset tileset)
		{
			int id = tile.Gid - tileset.FirstGid;
			//var a = tileset.Tiles.Where(x => x.Value.Id == id).ToList();

			var tilesList = tileset.Tiles.ToList();

			var r = tilesList.Where(x => x.Value.Id == id).FirstOrDefault();

			if (r.Value == null)
				Logger.GetOrCreate("BrTiledLoader").Log(Logger.LogLevel.Info, "Invalid tile at " + tile.X + ", " + tile.Y + ". Id is " + id + ", gid is " + tile.Gid);

			return r.Value;
		}
		
		private string GetTilesetTexture(TmxTileset tileset)
		{
			string source = tileset.Image.Source.Split('/').Last();
			
			return source.Split('.')[0];
		}

		private Rectangle GetTilesetTileSourceRect(TmxTilesetTile tile, TmxTileset tileset)
		{
			int x = tile.Id % tileset.Columns.Value;
			int y = tile.Id / tileset.Columns.Value;

			return new Rectangle(x * tileset.TileWidth, y * tileset.TileHeight, tileset.TileWidth, tileset.TileHeight);
		}

		private TiledTile ParseTile(TmxTileset tileset, TmxTilesetTile tilesetTile)
		{
			string texture = GetTilesetTexture(tileset);
			Rectangle sourceRectangle = GetTilesetTileSourceRect(tilesetTile, tileset);

			TiledTile tiledTile = new TiledTile();
			tiledTile.Initialize(tilesetTile, 0, texture, sourceRectangle);

			return tiledTile;
		}
	}
}
