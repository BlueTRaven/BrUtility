using BrAssetsManager;
using TiledSharp;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrTiledLoader
{
	public class AssetHandlerTiledMap : AssetHandler
	{
		public AssetHandlerTiledMap(ContentManager contentManager, AssetsManager assetsManager) : base(".tmx", "Maps", contentManager, assetsManager)
		{
		}

		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;
			assets[key].asset = new TmxMap("Content/" + assets[key].fileName + ".tmx");
		}
	}
}
