using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BrAssetsManager
{
	public class AssetHandlerTexture2D : AssetHandler
	{
		public AssetHandlerTexture2D(ContentManager contentManager, AssetsManager assetsManager) : base(".xnb", "Textures", contentManager, assetsManager)
		{
		}
		
		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;
			assets[key].asset = contentManager.Load<Texture2D>(assets[key].fileName);
		}

		public override void UnloadAsset(string key)
		{
			if (assets[key] != null)
				(GetAsset(key) as Texture2D).Dispose();
			base.UnloadAsset(key);
		}
	}
}
