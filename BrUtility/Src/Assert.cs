using BrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	/// <summary>
	/// An assert object. Other languages have assert usually as a macro... it can be accessed from any namespace without using a namespace of its own. If we were to include some sort of static Assert function,
	/// We would have to use the class name in order to access it. 
	/// Having it be an actual object gets around this!
	/// </summary>
	public struct Assert
	{
		private bool condition;

		public bool Failed => !condition;
		public bool Passed => condition;

		public Assert(bool condition, string message, Logger.LogLevel logLevel)
		{
			if (!condition)
				Logger.GetOrCreate("BrUtility").Log(logLevel, "Assert failed: " + message);

			this.condition = condition;
		}
	}
}
