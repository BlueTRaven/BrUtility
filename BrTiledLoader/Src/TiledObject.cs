using BlueRavenUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace BrTiledLoader
{
	public class TiledObject
	{
		public string layer;
		public Dictionary<string, string> properties;

		public string name;
		public string objType;

		public RectangleF position;

		public TiledObject()
		{
		}

		public void LoadObj(TmxObject obj)
		{
			this.name = obj.Name;
			position = new RectangleF((float)obj.X, (float)obj.Y, (float)obj.Width, (float)obj.Height);

			GetProperties(obj);
		}

		private void GetProperties(TmxObject obj)
		{
			//NOTE: we need to do this this way otherwise the properties dictionary will remain the TiledSharp version (for some reason???)
			//A little inefficient but hey this will only be done content compile time
			properties = new Dictionary<string, string>();
			foreach (string key in obj.Properties.Keys)
				this.properties.Add(key, obj.Properties[key]);

			objType = obj.Type;
		}

		public bool GetPropertyBool(string name, bool def = false)
		{
			if (properties.ContainsKey(name))
				return bool.Parse(properties[name]);
			else return def;
		}

		public float GetPropertyFloat(string name, float def = 0)
		{
			if (properties.ContainsKey(name))
				return float.Parse(properties[name]);
			else return def;
		}

		public int GetPropertyInt(string name, int def = 0)
		{
			if (properties.ContainsKey(name))
				return int.Parse(properties[name]);
			else return def;
		}

		public string GetPropertyString(string name, string def = "")
		{
			if (properties.ContainsKey(name))
				return properties[name];
			else return def;
		}

		public override string ToString()
		{
			return "Tiled Object: " + name + " type: " + objType + " layer: " + layer;
		}
	}
}
