using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue.Src
{
	public class DialogueManagerRW
	{
		public readonly DialogueUpdater updater;
		public readonly DialogueChooser chooser;

		public DialogueSetInstance currentDialogueSet;
		public int currentIndex;

		public DialogueManagerRW(DialogueUpdater updater, DialogueChooser chooser)
		{
			this.updater = updater;
			this.chooser = chooser;
		}

		public virtual void Update(GameTime gt)
		{
			updater.UpdateText(gt, currentDialogueSet, ref currentIndex);

			if (chooser.ShouldPlayNext())
				chooser.PlayNext();
		}
	}
}
