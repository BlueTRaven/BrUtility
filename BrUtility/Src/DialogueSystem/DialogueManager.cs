using A1r.Input;
using BrAssetsManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility.DialogueSystem
{
	public class DialogueManager
	{
		public bool IsPlaying => currentDialogueSet != null && !currentDialogueSet.done;
		public bool IsPaused => currentDialogueSet != null && !currentDialogueSet.done && currentDialogueSet.preDelay > 0;

		public SpriteFont font;
		
		public DialogueSetInstance currentDialogueSet;
		public string CurrentDialogueName { get; private set; }

		private int letterDelay;
		private string currentText;
		private int currentSubstringIndex;

		private bool currentFinished;

		public string fullText;

		public float pixelScale = 6;

		public Vector2 Position { get; private set; }
		public int Width { get; private set; }
		public Vector2 TextOffset { get; private set; }
		public Vector2 NameplateOffset { get; private set; }

		public float alpha = 1;

		public float stringDrawDepth;
		public float panelDrawDepth;

		private int currentlySelectedDialogueBranch;

		public event Action<DialogueSetInstance> DialogueChanged;
		public event Action DialogueSetStarted;
		public event Action DialogueSetEnded;
		public event Action<char> DialogueCharacterAdded;

		public event Action<SpriteBatch> DrawDialogueSet;

		private AssetsManager assetsManagerWithDialogue;

		private int sineTimer;

		public DialogueManager() { }

		public void Initialize(Vector2 position, int width, SpriteFont font)
		{
			this.Position = position;
			this.Width = width;

			this.font = font;
		}
		
		/// <param name="position">The position to start drawing the entire dialogue box.</param>
		/// <param name="width">The maximum width of the dialogue. Text will wrap if it goes over this width.</param>
		/// <param name="textOffset">The offset of the text box, relative to the position.</param>
		/// <param name="nameplateOffset">The offset of the nameplate, relative to the position.</param>
		public void UpdatePosition(Vector2 position, int width, Vector2 textOffset, Vector2 nameplateOffset)
		{
			this.Position = position;
			this.Width = width;
			this.TextOffset = textOffset;
			this.NameplateOffset = nameplateOffset;
		}

		/// <summary>
		/// Plays the given dialogue set. If another is playing, it is overwritten by this one.
		/// </summary>
		public void PlayDialogueSet(string setName, AssetsManager assetsManagerWithDialogue)
		{
			currentText = "";
			currentSubstringIndex = 0;

			currentDialogueSet = new DialogueSetInstance(assetsManagerWithDialogue.GetAsset<DialogueSet>(setName));
			CurrentDialogueName = setName;
			currentDialogueSet.StartNextDialogue();

			DialogueSetStarted?.Invoke();
			DialogueChanged?.Invoke(currentDialogueSet);

			this.assetsManagerWithDialogue = assetsManagerWithDialogue;

			fullText = CurrentDialogueFullText();
		}

		/// <summary>
		/// if the current dialogue set is not the given dialogue, sets the current dialogue to the given dialog, and starts playing it.
		/// Otherwise, if it is the current dialogue, continues the dialogue on to the next.
		/// </summary>
		/// <param name="setName"></param>
		public void PlayOrContinueDialogueSet(string setName, AssetsManager assetsManagerWithDialogue)
		{
			if (CurrentDialogueName == setName)
				ContinueDialogueSet();
			else
				PlayDialogueSet(setName, assetsManagerWithDialogue);
		}

		/// <summary>
		/// Continues playing the current dialogue set, if any.
		/// If the current dialogue instance is still running, completes it (does not continue.)
		/// Does nothing if no dialogue set is currently playing.
		/// </summary>
		public void ContinueDialogueSet()
		{
			if (currentDialogueSet != null)
			{
				currentText = "";
				currentSubstringIndex = 0;

				if (currentDialogueSet.choosingBranch)
				{
					PlayOrContinueDialogueSet(currentDialogueSet.GetDialogueBranch(currentlySelectedDialogueBranch), assetsManagerWithDialogue);
					currentlySelectedDialogueBranch = 0;
					return;
				}

				if (!currentFinished)
				{	//if the current dialogue set is not finished, finish it
					currentText = fullText;
					currentSubstringIndex = fullText.Length;

					currentFinished = true;
				}
				else
				{
					currentDialogueSet.StartNextDialogue();
					DialogueChanged?.Invoke(currentDialogueSet);

					fullText = CurrentDialogueFullText();

					currentFinished = false;

					if (currentDialogueSet.done)
					{
						if (currentDialogueSet.dialogueSet.information.stringsTo != null && currentDialogueSet.dialogueSet.information.stringsTo != "")
							PlayDialogueSet(currentDialogueSet.dialogueSet.information.stringsTo, assetsManagerWithDialogue);
						else
							ForceStopDialogue();
					}
				}
			}
		}

		public void ForceStopDialogue()
		{
			CurrentDialogueName = "none";
			if (currentDialogueSet != null)
				currentDialogueSet.Reset();
			currentDialogueSet = null;
			currentText = "";
			currentSubstringIndex = 0;

			DialogueSetEnded?.Invoke();
		}

		public void Update(InputManager inputManager, Keys continueKey, Keys upKey, Keys downKey)
		{
			sineTimer++;

			if (currentDialogueSet != null)
				if (inputManager.JustPressed(continueKey))
					ContinueDialogueSet();  //shouldn't do anything if choosing dialogue branch?

			if (currentDialogueSet != null)
			{
				if (currentDialogueSet.preDelay > 0)
					currentDialogueSet.preDelay--;
				else
				{
					currentFinished = false;

					if (!currentDialogueSet.choosingBranch)
						UpdateText();
					else UpdateChooseBranch(inputManager, continueKey, upKey, downKey);
				}

				if (currentFinished)
					currentText = fullText;
			}
		}

		private void UpdateText()
		{
			if (letterDelay <= 0)
			{
				if (currentSubstringIndex < fullText.Length)
				{
					while (letterDelay == 0)
					{
						letterDelay = currentDialogueSet.currentDialogue.letterDelay;
						currentSubstringIndex++;
						currentText = fullText.Substring(0, currentSubstringIndex);

						DialogueCharacterAdded?.Invoke(currentText[currentSubstringIndex - 1]);

						DialogueEffectInstance currentInstance = GetCurrentDialogueEffectInstance(currentText, currentSubstringIndex, false);

						if (currentInstance != null)
						{
							if (currentSubstringIndex == currentInstance.start)
								letterDelay += currentInstance.dialogueEffect.preDelay;
							if (currentSubstringIndex == currentInstance.end)
								letterDelay += currentInstance.dialogueEffect.postDelay;

							if (currentInstance.dialogueEffect.appearInstantly && currentSubstringIndex > currentInstance.start && currentSubstringIndex < currentInstance.end)
								letterDelay = 0;
						}
					}
				}
				else currentFinished = true;
			}
			else letterDelay--;
		}

		private void UpdateChooseBranch(InputManager inputManager, Keys continueKey, Keys upKey, Keys downKey)
		{
			if (inputManager.JustPressed(downKey))
				currentlySelectedDialogueBranch++;

			if (inputManager.JustPressed(upKey))
				currentlySelectedDialogueBranch--;

			int max = currentDialogueSet.dialogueSet.information.branchOptions.Length;

			if (currentlySelectedDialogueBranch < 0)
				currentlySelectedDialogueBranch = max - 1;
			if (currentlySelectedDialogueBranch > max - 1)
				currentlySelectedDialogueBranch = 0;

			//if (inputManager.JustPressed(continueKey))
		}

		public string CurrentDialogueFullText()
		{
			return "";	//TODO fix this. Taylor 11/17/19
			//Added scaling functionality to basically everything in TextHelper, which required swapping over to using FontInfo (a wrapper for SpriteFont) instead of a SpriteFont.
			//This class needs to be updated to use a FontInfo instead of SpriteFont.
			//return TextHelper.WrapText(font, currentDialogueSet.GetDialogue(), Width * pixelScale);
		}

		public void Draw(SpriteBatch batch)
		{
			if (currentDialogueSet != null && currentDialogueSet.preDelay <= 0)
			{
				string speakerName = currentDialogueSet.GetSpeaker();

				//Draw the speaker panel
				RecreateDrawString(batch, (Position + NameplateOffset) * pixelScale, font, speakerName, stringDrawDepth, true);
				
				if (!currentDialogueSet.choosingBranch)
				{
					if (currentText.Length > 0)
						RecreateDrawString(batch, (Position + TextOffset) * pixelScale, font, currentText, stringDrawDepth, false);
				}
				else
				{
					for (int i = 0; i < currentDialogueSet.dialogueSet.information.branchOptions.Length; i++)
					{
						var branchOption = currentDialogueSet.dialogueSet.information.branchOptions[i];

						Vector2 drawPos = Position;

						if (currentlySelectedDialogueBranch == i)
							drawPos += new Vector2(16, 0);

						DrawOutlinedString(batch, drawPos + new Vector2(32, (font.LineSpacing + 2) * (i + 1)) * pixelScale + TextOffset, branchOption.name, stringDrawDepth);
					}
				}

				DrawDialogueSet?.Invoke(batch);
			}
		}

		private void RecreateDrawString(SpriteBatch batch, Vector2 position, SpriteFont font, string text, float drawDepth, bool isNameplate)
		{
			Vector2 offset = Vector2.Zero;
			bool firstGlyphOfLine = true;

			var glyphs = font.GetGlyphs();
			for (int i = 0; i < text.Length; i++)
			{
				char character = text[i];

				if (character == '\r')
					continue;

				if (character == '\n')
				{
					offset.X = 0;
					offset.Y += font.LineSpacing;
					firstGlyphOfLine = true;
					continue;
				}

				DialogueEffectInstance effectInstance = GetCurrentDialogueEffectInstance(text, i, isNameplate);

				if (effectInstance != null && effectInstance.dialogueEffect.effect == "glitch_sml")
				{
					int times = Utility.rand.Next(-3, 3);

					if (times > 0)
					{
						string characters = "abcdefghijklmnopqrstuvwxyz" +
										"ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
										"0123456789-=" +
										"!@#$%^&*()_+" +
										"{}[]\\|'\";:,<.>/?";
						
						character = characters[Utility.rand.Next(0, characters.Length - 1)];
					}
				}

				SpriteFont.Glyph currentGlyph = glyphs[character];

				if (firstGlyphOfLine)
				{
					offset.X = Math.Max(currentGlyph.LeftSideBearing, 0);
					firstGlyphOfLine = false;
				}
				else offset.X += font.Spacing + currentGlyph.LeftSideBearing;

				Vector2 finalPos = offset;
				finalPos.X += currentGlyph.Cropping.X;
				finalPos.Y += currentGlyph.Cropping.Y;
				finalPos += position;

				RectangleF sourceRect = new RectangleF();
				sourceRect.x = currentGlyph.BoundsInTexture.X;
				sourceRect.y = currentGlyph.BoundsInTexture.Y;
				sourceRect.width = currentGlyph.BoundsInTexture.Width;
				sourceRect.height = currentGlyph.BoundsInTexture.Height;

				DrawCharacter(batch, text, i, finalPos, font, sourceRect, drawDepth, isNameplate);

				offset.X += currentGlyph.Width + currentGlyph.RightSideBearing;
			}
		}

		private void DrawCharacter(SpriteBatch batch, string text, int index, Vector2 position, SpriteFont font, RectangleF sourceRect, float drawDepth, bool isNameplate)
		{
			DialogueEffectInstance effectInstance = GetCurrentDialogueEffectInstance(text, index, isNameplate);

			Color outlineColor = new Color(Color.Black, alpha);
			Color color = new Color(Color.White, alpha);

			if (effectInstance != null)
			{
				if (effectInstance.dialogueEffect.effect != null)
				{
					if (effectInstance.dialogueEffect.effect == "shake" || effectInstance.dialogueEffect.effect == "glitch_sml")
						position += new Vector2(Utility.rand.NextFloat(-2, 2), Utility.rand.NextFloat(-2, 2));
					if (effectInstance.dialogueEffect.effect == "sine")
					{
						int rate = 60;
						float percent = (float)((sineTimer + (index * 5)) % rate) / (float)rate;
						float sine = (float)Math.Sin(MathHelper.Pi * 2 * percent) * 4;
						position.Y += sine;
					}
				}

				if (effectInstance.dialogueEffect.color != null)
				{
					string colorRGBA = effectInstance.dialogueEffect.color;
					string[] rgba = colorRGBA.Replace(" ", "").Split(',');

					bool hasR = int.TryParse(rgba[0], out int r);
					bool hasG = int.TryParse(rgba[1], out int g);
					bool hasB = int.TryParse(rgba[2], out int b);
					bool hasA = int.TryParse(rgba[3], out int a);
					
					if (hasR)
						color.R = (byte)r;
					if (hasG)
						color.G = (byte)g;
					if (hasB)
						color.B = (byte)b;
					if (hasA)
						color.A = (byte)a;
				}
			}

			batch.Draw(font.Texture, position + new Vector2(-2, 0), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);
			batch.Draw(font.Texture, position + new Vector2(2, 0), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);
			batch.Draw(font.Texture, position + new Vector2(0, -2), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);
			batch.Draw(font.Texture, position + new Vector2(0, 2), sourceRect.ToRectangle(), outlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);

			batch.Draw(font.Texture, position, sourceRect.ToRectangle(), color, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
		}

		private DialogueEffectInstance GetCurrentDialogueEffectInstance(string text, int index, bool isNameplate)
		{
			int numLeft = 0;

			for (int i = index - 1; i > 0; i--)
			{
				if (text[i] == '\n' || text[i] == '\r')
					numLeft++;
			}

			int realIndex = index - numLeft;

			foreach (var effectInstance in currentDialogueSet.currentDialogueEffects)
			{
				if (effectInstance.isNameplateEffect == isNameplate && realIndex >= effectInstance.start && realIndex < effectInstance.end)
					return effectInstance;
			}

			return null;
		}

		private void DrawOutlinedString(SpriteBatch batch, Vector2 position, string text, float drawDepth)
		{
			batch.DrawString(font, text, position + new Vector2(-2, 0), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);
			batch.DrawString(font, text, position + new Vector2(2, 0), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);
			batch.DrawString(font, text, position + new Vector2(0, -2), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);
			batch.DrawString(font, text, position + new Vector2(0, 2), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth - 0.01f);

			batch.DrawString(font, text, position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, drawDepth);
		}
	}
}
