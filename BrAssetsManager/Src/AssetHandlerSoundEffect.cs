using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace BrAssetsManager
{
	public class AssetHandlerSoundEffect : AssetHandler
	{
		public AssetHandlerSoundEffect(ContentManager contentManager, AssetsManager assetsManager) : base(".xnb", "Sounds/Effects", contentManager, assetsManager)
		{
		}
		
		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;
			assets[key].asset = contentManager.Load<SoundEffect>(assets[key].fileName);
		}
	}
}
