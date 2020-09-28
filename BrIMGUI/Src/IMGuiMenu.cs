using A1r.Input;
using BrUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility.IMGuiSystem
{
	/// <summary>
	/// Note that we don't have an update function, just a draw function. Drawing and updating are done in the same frame.
	/// </summary>
	public abstract class IMGuiMenu
	{
		public virtual void Initialize()
		{
		}

		public virtual void Draw(SpriteBatch batch, InputManager inputManager) {  }
	}
}
