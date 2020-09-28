using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrTiledLoader
{
	public struct TiledTileInstance
	{
		public TiledTile tile;
		public Point position;
		public string layer;

		public bool valid;

		public TiledLayer.TileType.Flip flip;

		public TiledTileInstance(TiledTile tile, Point position, string layer, TiledLayer.TileType.Flip flip)// = TiledLayer.TileType.Flip.FLIP_NONE)
		{
			this.tile = tile;
			this.position = position;
			this.layer = layer;

			this.flip = flip;

			valid = true;
		}
	}
}
