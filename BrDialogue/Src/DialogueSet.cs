using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue
{
	public struct DialogueSpeaker
	{
		public string name;

		public string portrait;

		public string typingSoundSet;

		public DialogueSpeaker(string name, string portrait, string typingSoundSet)
		{
			this.name = name;
			this.portrait = portrait;
			this.typingSoundSet = typingSoundSet;
		}
	}

	public struct DialogueInformation
	{
		public string type;
		public string stringsTo;    //what this dialogue "strings to" or plays after this one is done.
		public DialogueBranchOption[] branchOptions;
		public DialogueEffect[] dialogueEffects;
	}

	public struct DialogueBranchOption
	{
		public string name;
		public string branchToDialogue;
	}

	public class DialogueSet
	{
		public DialogueInformation information;

		public DialogueSpeaker[] speakers;
		public List<Dialogue> dialogues;

		public DialogueSet()
		{
			speakers = new DialogueSpeaker[8];
			dialogues = new List<Dialogue>();
		}

		public void SetSpeakers(params DialogueSpeaker[] speakers)
		{
			this.speakers = speakers;
		}

		public void SetDialogues(params Dialogue[] dialogues)
		{
			for (int i = 0; i < dialogues.Length; i++)
				this.dialogues.Add(dialogues[i]);
		}
	}
}
