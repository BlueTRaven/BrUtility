using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrAssetsManager
{
	public class Asset
	{
		public string key, fileName;
		public bool loadedAsset;
		public object asset;

		public Asset(string key, string fileName)
		{
			this.key = key;
			this.fileName = fileName;
		}
	}

	public abstract class AssetHandler
	{
		public readonly string directoryName;
		public readonly string extentionName;

		public Dictionary<string, Asset> assets;

		protected ContentManager contentManager;
		protected AssetsManager assetManager;

		public AssetHandler(string extentionName, string directoryName, ContentManager contentManager, AssetsManager assetManager)
		{
			this.directoryName = directoryName;
			this.extentionName = extentionName;

			assets = new Dictionary<string, Asset>();
			this.contentManager = contentManager;
			this.assetManager = assetManager;
		}

		public virtual void SetAssetReference(string key, string name, string fullName)
		{
			assets.Add(key, new Asset(key, name));
		}

		public abstract void LoadAsset(string key);

		public virtual void UnloadAsset(string key)
		{
			if (assets[key].loadedAsset && assets[key].asset is IDisposable disposable)
				disposable.Dispose();

			assets[key].loadedAsset = false;
			assets[key].asset = null;
		}

		public virtual object GetAsset(string key)
		{
			if (!assets[key].loadedAsset)
				LoadAsset(key);

			return assets[key].asset;
		}
	}
}
