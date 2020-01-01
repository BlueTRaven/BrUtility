using BlueRavenUtility;
using BrAssetsManager;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue
{
	public class DialogueHandler : AssetHandler
	{
		public DialogueHandler(ContentManager contentManager, AssetsManager assetsManager) : base(".json", "Dialogue", contentManager, assetsManager)
		{
		}
		
		public override void LoadAsset(string key)
		{
			assets[key].loadedAsset = true;

			DialogueSet deserialized = Deserialize(assets[key].fileName);
			if (deserialized == null)
				Logger.GetOrCreate("BrUtility").Log(Logger.LogLevel.Error, "Could not load a dialogue asset by name of " + assets[key].fileName + ". Is the file missing, or is the json incorrect?");

			assets[key].asset = deserialized;
		}

		private DialogueSet Deserialize(string name)
		{
			using (StreamReader reader = new StreamReader("Content/" + name + ".json"))
			{
				return JsonConvert.DeserializeObject<DialogueSet>(reader.ReadToEnd());
			}
		}
	}
}
