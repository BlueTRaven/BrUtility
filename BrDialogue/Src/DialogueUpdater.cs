using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrDialogue.Src
{
	/// <summary>
	/// Handles updating the characters of the text
	/// </summary>
	public class DialogueUpdater
	{
		protected enum EffectInstanceStanding
		{
			Start,	//First character of an effect instance
			Middle,	//Any other character of an effect instance
			End		//Last character of an effect instance
		}

		private float timer;

		public virtual void UpdateText(GameTime gt, DialogueSetInstance setInstance, ref int index)
		{
			if (ShouldGetNextChar())
			{
				timer -= (float)gt.ElapsedGameTime.TotalSeconds;

				while (timer <= 0)	//we want to do this in a while loop because of the "appear instantly" property - otherwise one would appear every frame instead of all at once
				{
					timer = GetNextLetterTime(setInstance, index);

					if (!IsAtEnd(setInstance, index))
						index++;
					else break;	//break out if we're at the end (as we can have a high letter delay that is never fixed...)
				}
			}
		}

		public virtual float GetNextLetterTime(DialogueSetInstance setInstance, int index)
		{
			float delay = -1;
			if (index == 0)
				delay = setInstance.currentDialogue.preDelay;
			else delay = setInstance.currentDialogue.letterDelay;

			var effect = GetCurrentDialogueEffectInstance(setInstance, index, false);

			if (effect != null)
			{
				if (effect.dialogueEffect.appearInstantly)
					delay = -1;
				else
				{
					var standing = GetDialogueEffectInstanceStanding(setInstance, index, effect);
					if (standing == EffectInstanceStanding.Start)
						delay = effect.dialogueEffect.preDelay;
					else if (standing == EffectInstanceStanding.End)
						delay = effect.dialogueEffect.postDelay;
				}
			}

			return delay;
		}

		public virtual bool IsAtEnd(DialogueSetInstance setInstance, int index)
		{
			return index - 1 == setInstance.currentDialogueText.Length;
		}

		public virtual bool ShouldGetNextChar()
		{
			return true;
		}

		private DialogueEffectInstance GetCurrentDialogueEffectInstance(DialogueSetInstance setInstance, int index, bool isNameplate)
		{
			int realIndex = GetRealIndex(setInstance, index);

			foreach (var effectInstance in setInstance.currentDialogueEffects)
			{
				if (effectInstance.isNameplateEffect == isNameplate && realIndex >= effectInstance.start && realIndex < effectInstance.end)
					return effectInstance;
			}

			return null;
		}

		protected EffectInstanceStanding GetDialogueEffectInstanceStanding(DialogueSetInstance setInstance, int index, DialogueEffectInstance dialogueEffect)
		{
			int realIndex = GetRealIndex(setInstance, index);

			if (realIndex == dialogueEffect.start)
				return EffectInstanceStanding.Start;
			else if (realIndex == dialogueEffect.end - 1)
				return EffectInstanceStanding.End;
			else return EffectInstanceStanding.Middle;
		}

		protected int GetRealIndex(DialogueSetInstance setInstance, int index)
		{
			int numLeft = 0;

			for (int i = index - 1; i > 0; i--)
			{
				if (CharAt(setInstance, index) == '\n' || CharAt(setInstance, index) == '\r')
					numLeft++;
			}

			return index - numLeft;
		}

		protected char CharAt(DialogueSetInstance setInstance, int index)
		{
			return setInstance.currentDialogueText[index];
		}
	}
}
