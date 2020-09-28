using BrAssetsManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using BrUtility;

namespace BrNineSlice
{
	public class AssetHandlerNineSlice : AssetHandler
	{
		public AssetHandlerNineSlice(ContentManager contentManager, AssetsManager assetManager) : base(".json", "NineSlices", contentManager, assetManager)
		{
		}

		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;

			NineSliceAsset deserialized = Deserialize(assets[key].fileName);
			if (deserialized == null)
				Logger.GetOrCreate("BrUtility").Log(Logger.LogLevel.Error, "Could not load a nine slice asset by name of " + assets[key].fileName + ". Is the file missing, or is the json incorrect?");

			var sizeSer = deserialized.size.Split(',');
			Size size = new Size(float.Parse(sizeSer[0]), float.Parse(sizeSer[1]));
			NineSlice slice = new NineSlice(assetManager.GetAsset<Texture2D>(deserialized.texture), size, 
				deserialized.distLeft, deserialized.distRight, deserialized.distTop, deserialized.distBottom);
			assets[key].asset = slice;
		}

		private NineSliceAsset Deserialize(string name)
		{
			using (StreamReader reader = new StreamReader("Content/" + name + ".json"))
			{
				return JsonConvert.DeserializeObject<NineSliceAsset>(reader.ReadToEnd());
			}
		}

		public override void UnloadAsset(string key)
		{
			base.UnloadAsset(key);

			(assets[key].asset as NineSlice).TexTopLeft.Dispose();
			(assets[key].asset as NineSlice).TexTopRight.Dispose();
			(assets[key].asset as NineSlice).TexBotLeft.Dispose();
			(assets[key].asset as NineSlice).TexBotRight.Dispose();

			(assets[key].asset as NineSlice).TexLeft.Dispose();
			(assets[key].asset as NineSlice).TexRight.Dispose();
			(assets[key].asset as NineSlice).TexTop.Dispose();
			(assets[key].asset as NineSlice).TexBottom.Dispose();

			(assets[key].asset as NineSlice).TexCenter.Dispose();
		}
	}
}
