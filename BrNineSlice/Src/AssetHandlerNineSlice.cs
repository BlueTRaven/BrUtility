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
using Microsoft.Xna.Framework;

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

			Vector2 position = Vector2.Zero;
			if (deserialized.position.Where(x => x == ',').Count() == 1)
			{
				var posSer = deserialized.size.Split(',');

				float.TryParse(posSer[0], out float px);
				float.TryParse(posSer[1], out float py);

				position = new Vector2(px, py);
			}

			Size size = Size.Zero;
			if (deserialized.size.Where(x => x == ',').Count() == 1)
			{
				var sizeSer = deserialized.size.Split(',');

				float.TryParse(sizeSer[0], out float sx);
				float.TryParse(sizeSer[1], out float sy);

				size = new Size(sx, sy);
			}

			NineSlice slice = new NineSlice(assetManager.GetAsset<Texture2D>(deserialized.texture), new RectangleF(position, size), 
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
