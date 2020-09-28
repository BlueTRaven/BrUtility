using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility.Animations
{
	public class Animation
	{
		public int start;
		public Frame current;
		private int frameTime;

		public Frame Next => frames[current.nextIndex];
		public Frame[] frames;

		//serialization purposes only
		public Animation()
		{

		}

		public Animation(int start, params Frame[] frames)
		{
			this.start = start;
			this.frames = frames;

			this.current = frames[start];
		}

		public void Update()
		{
			frameTime--;

			if (frameTime <= 0)
			{
				current = Next;
				frameTime = current.time;
			}
		}
	}
}
