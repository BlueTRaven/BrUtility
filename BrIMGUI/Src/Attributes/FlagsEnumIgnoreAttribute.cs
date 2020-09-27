using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility.IMGuiSystem.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FlagsEnumIgnoreAttribute : Attribute
	{
		public FlagsEnumIgnoreAttribute()
		{

		}
	}
}
