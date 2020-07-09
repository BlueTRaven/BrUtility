using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue.Src
{
	public abstract class DialogueChooser
	{
		private DialogueManagerRW manager;

		internal void Start(DialogueManagerRW manager)
		{
			this.manager = manager;
		}

		public void Play(DialogueSetInstance setInstance)
		{
			manager.currentDialogueSet = setInstance;
			manager.currentIndex = 0;	//reset the index, so we're not starting halfway through
		}

		public void PlayNext()
		{
			if (CanFinish() && !manager.updater.IsAtEnd(manager.currentDialogueSet, manager.currentIndex))
			{
				manager.currentIndex = manager.currentDialogueSet.currentDialogueText.Length - 1;
			}
			else
			{
				manager.currentDialogueSet.StartNextDialogue();
				manager.currentIndex = 0;
			}
		}

		public abstract bool ShouldPlayNext();

		public virtual bool CanFinish()
		{
			return true;
		}
	}
}
