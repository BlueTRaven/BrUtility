using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrAssetsManager
{
	/// <summary>
	/// Note: this doesn't handle unloaded assets!
	/// TODO: handle that (perhaps add an OnUnload event in assetHandler?)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AssetReference<T>
	{
		private T cached;
		private string name;

		private AssetsManager assetsManager;

		public AssetReference(AssetsManager assetsManager, string name)
		{
			this.assetsManager = assetsManager;
			this.name = name;
		}

		public T Get()
		{
			if (cached == null)
				cached = assetsManager.GetAsset<T>(name);

			return cached;
		}
	}
}
