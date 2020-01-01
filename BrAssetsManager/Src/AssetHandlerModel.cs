using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BrAssetsManager
{
	public class AssetHandlerModel : AssetHandlerGeneric<Model>
	{
		public AssetHandlerModel(ContentManager contentManager, AssetsManager assetsManager) : base(".xnb", "Models", contentManager, assetsManager)
		{
		}
	}
}
