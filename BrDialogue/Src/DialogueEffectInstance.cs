using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue
{
	public class DialogueEffectInstance
	{
		public DialogueEffect dialogueEffect;

		public int start, end;

		public bool isNameplateEffect;

		public DialogueEffectInstance(DialogueEffect effect, int start, int end, bool isNameplateEffect)
		{
			this.dialogueEffect = effect;
			this.start = start;
			this.end = end;
			this.isNameplateEffect = isNameplateEffect;
		}

		public string GetDrawableText(string text)
		{
			string fstr = text.Substring(start, end - start);
			fstr = fstr.PadLeft(start + fstr.Length);
			fstr = fstr.PadRight((text.Length - end) + fstr.Length);
			return fstr;
		}
	}
}
