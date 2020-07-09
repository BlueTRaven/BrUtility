using A1r.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue.Src
{
	public class DialogueChooserInput : DialogueChooser
	{
		private InputManager inputManager;
		private Keys playKey;

		public DialogueChooserInput(InputManager inputManager, Keys playKey)
		{
			this.inputManager = inputManager;
			this.playKey = playKey;
		}

		public override bool ShouldPlayNext()
		{
			return inputManager.JustPressed(playKey);
		}
	}
}
