using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace BrAssetsManager
{
	public class AssetHandlerSong : AssetHandlerGeneric<Song>
	{
		public AssetHandlerSong(ContentManager contentManager, AssetsManager assetsManager) : base(".xnb", "Sounds/Songs", contentManager, assetsManager)
		{
		}
	}
}
