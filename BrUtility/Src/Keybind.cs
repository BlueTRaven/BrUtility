using A1r.Input;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public class Keybind
	{
		private enum KeybindType
		{
			Keyboard,
			Mouse,
			ControllerDxInput,
			Controller
		}

		[JsonRequired]
		private KeybindType type;

		[JsonRequired]
		private Keys key;

		[JsonRequired]
		private MouseInput mouseInput;

		[JsonRequired]
		private Buttons button;
		[JsonRequired]
		private int buttonsIndex = -1;//buttons index for dxinput

		[JsonRequired]
		private Input input;
		[JsonRequired]
		private int inputIndex = -1;//buttons input for other inputs

		private InputManager inputManager;

		#region Ctors
		public Keybind(Keys key)
		{
			type = KeybindType.Keyboard;
			this.key = key;
		}

		public Keybind(MouseInput mouseInput)
		{
			type = KeybindType.Mouse;
			this.mouseInput = mouseInput;
		}

		public Keybind(Buttons button)
		{
			type = KeybindType.ControllerDxInput;
			this.button = button;
		}

		public Keybind(Buttons button, int index)
		{
			type = KeybindType.ControllerDxInput;
			this.button = button;
			this.buttonsIndex = index;
		}

		public Keybind(Input input)
		{
			type = KeybindType.Controller;
			this.input = input;
		}

		public Keybind(Input input, int index)
		{
			type = KeybindType.Controller;
			this.input = input;
			this.inputIndex = index;
		}
		#endregion

		public void BindInputmanager(InputManager inputManager)
		{
			this.inputManager = inputManager;
		}

		public bool JustPressed()
		{
			switch (type)
			{
				case KeybindType.Controller:
					if (inputIndex != -1)
						return inputManager.JustPressed(input, inputIndex);
					else return inputManager.JustPressed(input);
				case KeybindType.ControllerDxInput:
					if (buttonsIndex != -1)
						return inputManager.JustPressed(button, buttonsIndex);
					else return inputManager.JustPressed(button, 0);
				case KeybindType.Keyboard:
					return inputManager.JustPressed(key);
				case KeybindType.Mouse:
					return inputManager.JustPressed(mouseInput);

				default:
					return false;
			}
		}

		public bool IsPressed()
		{
			switch (type)
			{
				case KeybindType.Controller:
					if (inputIndex != -1)
						return inputManager.IsPressed(input);
					else return inputManager.IsPressed(input, inputIndex);
				case KeybindType.ControllerDxInput:
					if (buttonsIndex != -1)
						return inputManager.IsPressed(button, buttonsIndex);
					else return false;
				case KeybindType.Keyboard:
					return inputManager.IsPressed(key);
				case KeybindType.Mouse:
					return inputManager.IsPressed(mouseInput);

				default:
					return false;
			}
		}

		public bool IsHeld()
		{
			switch (type)
			{
				case KeybindType.Controller:
					if (inputIndex != -1)
						return inputManager.IsHeld(input);
					else return inputManager.IsHeld(input, inputIndex);
				case KeybindType.ControllerDxInput:
					if (buttonsIndex != -1)
						return inputManager.IsHeld(button, buttonsIndex);
					else return false;
				case KeybindType.Keyboard:
					return inputManager.IsHeld(key);
				case KeybindType.Mouse:
					return inputManager.IsHeld(mouseInput);

				default:
					return false;
			}
		}

		public override string ToString()
		{
			return GetStringRepresentation();
		}

		private string GetStringRepresentation()
		{
			if (type == KeybindType.Mouse)
			{
				switch (mouseInput)
				{
					case MouseInput.LeftButton:
						return "lmb";
					case MouseInput.MiddleButton:
						return "mmb";
					case MouseInput.RightButton:
						return "rmb";
					case MouseInput.Button1:
						return "mb4";
					case MouseInput.Button2:
						return "mb5";
					default:
						return "unknown mouse button";
				}
			}
			else if (type == KeybindType.Controller)
				return input.ToString();
			else if (type == KeybindType.ControllerDxInput)
				return button.ToString();
			else if (type == KeybindType.Keyboard)
				return key.ToString();
			else return base.ToString();
		}
	}
}
