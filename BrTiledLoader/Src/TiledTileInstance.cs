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

		public TiledTileInstance(TiledTile tile, Point position, string layer)
		{
			this.tile = tile;
			this.position = position;
			this.layer = layer;

			valid = true;
		}
	}
}
