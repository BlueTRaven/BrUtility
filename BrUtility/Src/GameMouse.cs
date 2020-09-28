using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BrUtility
{
    public class GameMouse
    {
        private MouseState currentState, lastState;
        public Vector2 currentPosition, lastPosition;
        private float currentScrollWheelValue;
        public float deltaScrollWheelValue;

        public bool hasFocus;

		private int largestPriority;    //priority system works but only if largest priority is called first...

		private Camera camera;

        public GameMouse()
        {
		}

		public void Initialize(Camera camera)
		{
			this.camera = camera;
		}

        public void PreUpdate()
        {
			largestPriority = 0;
            currentState = Mouse.GetState();
            currentPosition = currentState.Position.ToVector2() * camera.Scale;

            deltaScrollWheelValue = currentState.ScrollWheelValue - currentScrollWheelValue;
            currentScrollWheelValue += deltaScrollWheelValue;
        }

        public void Update()
        {

        }

        public void PostUpdate()
        {
            lastState = currentState;
            lastPosition = currentPosition;
        }

        public Vector2 GetWorldPosition()
        {
            return VectorHelper.ScreenToWorldCoords(currentPosition, camera);
        }

		public Point GetTilePosition(int tileSize)
		{
			return (VectorHelper.ScreenToWorldCoords(currentPosition, camera) / tileSize).ToPoint();
		}

        public float ScrollWheelDirection()
        {
            return deltaScrollWheelValue;
        }

        public bool LeftButtonPressed(int priority = 0)
        {
			if (priority >= largestPriority)
				largestPriority = priority;
			else return false;
            return currentState.LeftButton == ButtonState.Pressed && lastState.LeftButton == ButtonState.Released && hasFocus;
        }

        public bool LeftButtonHeld(int priority = 0)
        {
			if (priority >= largestPriority)
				largestPriority = priority;
			else return false;

			return currentState.LeftButton == ButtonState.Pressed && hasFocus;
        }

		public bool LeftButtonReleased()
		{
			return currentState.LeftButton == ButtonState.Released && lastState.LeftButton == ButtonState.Pressed && hasFocus;
		}
    }
}
