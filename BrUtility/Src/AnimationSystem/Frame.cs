using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility.Animations
{
	public struct Frame
	{
		public int time;
		public int nextIndex;
		public SourceRectangle sourceRect;

		public Frame(int time, int nextIndex, SourceRectangle sourceRect)
		{
			this.time = time;
			this.nextIndex = nextIndex;
			this.sourceRect = sourceRect;
		}
	}
}
