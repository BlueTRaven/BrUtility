using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility.DialogueSystem
{
	public struct Dialogue
	{
		public int speaker;
		public string text;
		public int preDelay;
		public int letterDelay;
		
		public Dialogue(int speaker, string text, int preDelay = 0, int letterDelay = 100)
		{
			this.speaker = speaker;
			this.text = text;
			this.preDelay = preDelay;
			this.letterDelay = letterDelay;
		}
	}
}
