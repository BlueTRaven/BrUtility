using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrAssetsManager
{
    public abstract class AssetsManager
    {
		public Dictionary<Type, AssetHandler> assetTypes;

		public AssetsManager(ContentManager contentManager)
		{
			assetTypes = new Dictionary<Type, AssetHandler>();

			AddAssetTypes(contentManager);
		}

		public abstract void AddAssetTypes(ContentManager contentManager);

		#region get assets
		public virtual void LoadAsset<T>(string name)
		{
			if (assetTypes.ContainsKey(typeof(T)) && assetTypes[typeof(T)].assets.ContainsKey(name))
			{
				assetTypes[typeof(T)].LoadAsset(name);
			}
		}

		/// <summary>
		/// Gets asset of type <typeparamref name="T"/> with name <paramref name="name"/>, if it exists.
		/// </summary>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="name"></param>
		/// <returns>The asset as type T.</returns>
		public virtual T GetAsset<T>(string name)
		{
			if (assetTypes.ContainsKey(typeof(T)) && assetTypes[typeof(T)].assets.ContainsKey(name))
			{
				return (T)assetTypes[typeof(T)].GetAsset(name);
			}

			return default(T);
		}

		/// <summary>
		/// Gets a list of all values (assets) with a given type.
		/// </summary>
		/// <typeparam name="T">The type of asset list to return.</typeparam>
		/// <returns>A list of all values (assets) with the given type.</returns>
		public virtual List<T> GetAssetValuesList<T>()
		{
			if (assetTypes.ContainsKey(typeof(T)))
				return assetTypes[typeof(T)].assets.Values.ToList() as List<T>;
			else return null;
		}

		/// <summary>
		/// Gets a list of all keys with a given type.
		/// </summary>
		/// <typeparam name="T">The type of asset list to return.</typeparam>
		/// <returns>A list of all keys with the given type.</returns>
		public virtual List<string> GetAssetKeysList<T>()
		{
			if (assetTypes.ContainsKey(typeof(T)))
				return assetTypes[typeof(T)].assets.Keys.ToList() as List<string>;
			else return null;
		}

		/// <summary>
		/// Gets a full dictionary of all assets of type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual Dictionary<string, Asset> GetAssetDict<T>()
		{
			if (assetTypes.ContainsKey(typeof(T)))
				return assetTypes[typeof(T)].assets;
			else return null;
		}
		#endregion

		/// <summary>
		/// Unloads asset of type <typeparamref name="T"/> with name <paramref name="assetName"/>.
		/// </summary>
		/// <typeparam name="T">The asset type.</typeparam>
		/// <param name="assetName">The asset name.</param>
		public virtual void UnloadAsset<T>(string assetName)
		{
			assetTypes[typeof(T)].UnloadAsset(assetName);
		}

		protected virtual HashSet<string> GetValidAssetExtentions()
		{
			HashSet<string> validExt = new HashSet<string>();

			foreach (AssetHandler handler in assetTypes.Values.ToList())
			{
				if (!validExt.Contains(handler.extentionName))
					validExt.Add(handler.extentionName);
			}

			return validExt;
		}

		public virtual void LoadContent(string fulldirectoryname)
		{
			int indexAdd = fulldirectoryname.Contains("Global") ? 2 : 1;

			DirectoryInfo d = new DirectoryInfo(fulldirectoryname);
			if (!d.Exists)
				d.Create();
			List<FileInfo> files = d.GetFiles("*", SearchOption.AllDirectories).ToList();

			foreach (FileInfo file in files)
			{
				if (!GetValidAssetExtentions().Contains(file.Extension))
					continue;
				try
				{
					string name = file.Name.Split('.')[0];

					string[] split = file.DirectoryName.Split('\\');
					int index = split.ToList().IndexOf("Content");

					string directory = "";
					for (int i = index + 1; i < split.Length; i++)
					{   //super efficency
						if (i == index + 1)
							directory += split[i];
						else
							directory += "/" + split[i];
					}

					foreach (AssetHandler assetType in assetTypes.Values)
					{
						string assetDirectory = assetType.directoryName;

						if (file.Extension == assetType.extentionName && (directory == assetDirectory || directory.StartsWith(assetDirectory)))
						{
							assetType.SetAssetReference(name, directory + "/" + name, fulldirectoryname);
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
    }
}
