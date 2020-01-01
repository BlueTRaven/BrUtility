using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BrAssetsManager
{
	public class AssetHandlerFont : AssetHandler
	{
		public AssetHandlerFont(ContentManager contentManager, AssetsManager assetManager) : base(".xnb", "Fonts", contentManager, assetManager)
		{
		}
		
		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;
			assets[key].asset = contentManager.Load<SpriteFont>(assets[key].fileName);
			(assets[key].asset as SpriteFont).Texture.Name = key;
		}
	}
}
