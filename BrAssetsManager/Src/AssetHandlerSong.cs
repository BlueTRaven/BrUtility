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

		public override object GetAsset(string key)
		{
			
			return base.GetAsset(key);
		}

		public override void LoadAsset(string key)
		{
			//TODO: if audio cannot be loaded - i.e. audio hardware isn't present - handle exception explicitly, don't load.
			base.LoadAsset(key);
		}
	}
}
