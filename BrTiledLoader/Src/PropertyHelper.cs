using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BrTiledLoader
{
	public static class PropertyHelper
	{
		public static void CopyProperties(Dictionary<string, string> a, TiledSharp.PropertyDict b)
		{
			foreach (string key in b.Keys)
			{
				a.Add(key, b[key]);
			}
		}

		public static bool GetPropertyBool(Dictionary<string, string> properties, string name, bool def = false)
		{
			if (properties.ContainsKey(name))
				return bool.Parse(properties[name]);
			else return def;
		}

		public static float GetPropertyFloat(Dictionary<string, string> properties, string name, float def = 0)
		{
			if (properties.ContainsKey(name))
				return float.Parse(properties[name]);
			else return def;
		}

		public static int GetPropertyInt(Dictionary<string, string> properties, string name, int def = 0)
		{
			if (properties.ContainsKey(name))
				return int.Parse(properties[name]);
			else return def;
		}

		public static string GetPropertyString(Dictionary<string, string> properties, string name, string def = "")
		{
			if (properties.ContainsKey(name))
				return properties[name];
			else return def;
		}
	}
}
