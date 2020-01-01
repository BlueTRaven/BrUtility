using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility.DialogueSystem
{
	public class DialogueSetInstance
	{
		public DialogueSet dialogueSet;

		public Queue<Dialogue> playingDialogues;
		public Dialogue currentDialogue;
		public string currentDialogueText;

		public string currentSpeakerText;

		public List<DialogueEffectInstance> currentDialogueEffects;

		public HashSet<string> hideNames;

		public int preDelay;

		public bool initialized;
		public bool done;
		public bool choosingBranch;

		public int PlayingDialogueIndex { get; private set; }

		public DialogueSetInstance(DialogueSet dialogueSet)
		{
			this.dialogueSet = dialogueSet;

			playingDialogues = new Queue<Dialogue>();

			hideNames = new HashSet<string>();
		}

		public void StartNextDialogue()
		{
			if (!initialized)
				Initialize();

			if (playingDialogues.Count > 0)
				GetNextDialogue();
			else
			{
				currentDialogue = new Dialogue();

				if (!choosingBranch && dialogueSet.information.type == "genericBranching")
					choosingBranch = true;
				else done = true;
			}
		}

		private void Initialize()
		{
			initialized = true;
			playingDialogues = new Queue<Dialogue>(dialogueSet.dialogues);
		}

		private void GetNextDialogue()
		{
			PlayingDialogueIndex++;
			currentDialogue = playingDialogues.Dequeue();
			preDelay = currentDialogue.preDelay;
			initialized = true;

			currentDialogueEffects = new List<DialogueEffectInstance>();
			currentDialogueText = ParseDialogue(currentDialogue.text, false);
			currentSpeakerText = ParseDialogue(GetCurrentSpeaker().name, true);
		}

		private string ParseDialogue(string text, bool isNameplate)
		{
			string full = text;

			string[] effects = full.Split('@');

			full = "";

			int length = 0;

			for (int i = 0; i < effects.Length; i++)
			{   //TODO doesn't support > 10 because that becomes more than one character
				string effect = effects[i];

				if (effect.Length > 0 && int.TryParse(effect[0].ToString(), out int index))
				{   //is a valid effect
					string[] effectFull = effect.Split('}');    //should only ever have 2

					string fullAdd = effectFull[0].Substring(2) + //substring out the {
						effectFull[1];

					full += fullAdd;

					currentDialogueEffects.Add(new DialogueEffectInstance(dialogueSet.information.dialogueEffects[index], length, length + effectFull[0].Substring(2).Length, isNameplate));

					length += fullAdd.Length;
				}
				else
				{
					full += effect;
					length += effect.Length;
				}
			}

			return full;
		}

		public string GetDialogue()
		{
			return currentDialogueText;
		}

		public string GetSpeaker()
		{
			return currentSpeakerText;
		}

		public string GetDialogueBranch(int index)
		{
			return dialogueSet.information.branchOptions[index].branchToDialogue;
		}

		public void Reset()
		{
			hideNames.Clear();
			playingDialogues.Clear();
			initialized = true;

			currentDialogue = new Dialogue();
			preDelay = 0;
		}

		private DialogueSpeaker GetCurrentSpeaker()
		{
			DialogueSpeaker speaker = dialogueSet.speakers[currentDialogue.speaker];
			if (hideNames.Contains(speaker.name))
				return new DialogueSpeaker("???", speaker.portrait, speaker.typingSoundSet);
			else return speaker;
		}
	}
}
