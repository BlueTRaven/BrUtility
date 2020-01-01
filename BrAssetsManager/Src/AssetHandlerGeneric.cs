using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace BrAssetsManager
{
	public class AssetHandlerGeneric<T> : AssetHandler
	{
		public AssetHandlerGeneric(string extentionName, string directoryName, ContentManager contentManager, AssetsManager assetManager) : base(extentionName, directoryName, contentManager, assetManager)
		{
		}

		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;
			assets[key].asset = contentManager.Load<T>(assets[key].fileName);
		}
	}
}
